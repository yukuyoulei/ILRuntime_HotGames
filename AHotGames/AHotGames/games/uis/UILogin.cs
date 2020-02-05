using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

public class UILogin : AHotBase
{
	public static string CachedUsernameAndTokenArguments { get { return $"username={CachedUsername}&token={token}"; } }
	public static string CachedUsername
	{
		get
		{
			return PlayerPrefs.GetString("un");
		}
		set
		{
			PlayerPrefs.SetString("un", value);
		}
	}

	public static string token
	{
		get
		{
			return PlayerPrefs.GetString("token");
		}
		set
		{
			PlayerPrefs.SetString("token", value);
		}
	}
	InputField inputUsername;
	InputField inputPassword;
	protected override void InitComponents()
	{
		UICommonWait.Show();
		Task.Run(async () =>
		{
			if (!LibClient.AClientApp.bConnected)
				await LibClient.AClientApp.StartClient();
			UWebSender.Instance.AddProducingAction(() =>
			{
				UICommonWait.Hide();
			});
		});

		inputUsername = FindWidget<InputField>("inputUsername");
		if (!string.IsNullOrEmpty(CachedUsername))
		{
			inputUsername.text = CachedUsername;
		}

		inputPassword = FindWidget<InputField>("inputPassword");
		var btnLogin = FindWidget<Button>("btnLogin");
		btnLogin.onClick.AddListener(() =>
		{
			Task.Run(async () =>
			{
				if (!LibClient.AClientApp.bConnected)
					await LibClient.AClientApp.StartClient();

				if (!LibClient.AClientApp.bConnected)
				{
					AOutput.Log($"连接失败！");
					return;
				}

				UWebSender.Instance.AddProducingAction(OnLogin);
			});
			btnLogin.enabled = false;
		});
		var btnRegister = FindWidget<Button>("btnRegister");
		btnRegister.onClick.AddListener(() =>
		{
			OnUnloadThis();

			LoadAnotherUI<UIRegister>();
		});

		if (!string.IsNullOrEmpty(token))
		{
			inputPassword.text = "******";
			btnLogin.enabled = false;
			UStaticWebRequests.OnWebRequest("Login/CheckToken", "username=" + CachedUsername + "&token=" + token, jobj =>
			{
				OnSelectAvatar();
			},
			jfail =>
			{
				btnLogin.enabled = true;
				inputPassword.text = "";
				token = "";
			});
		}
	}
	private void OnLogin()
	{
		if (string.IsNullOrEmpty(inputUsername.text))
		{
			return;
		}
		if (string.IsNullOrEmpty(inputPassword.text))
		{
			return;
		}
		var username = inputUsername.text;
		var password = inputPassword.text;
		AClientApis.OnLogin(username, MD5String.Hash32(password), LibPacket.PktLoginRequest.EPartnerID.Test);
	}

	private void OnSelectAvatar()
	{
		UStaticWebRequests.DoSelectAvatar(UILogin.CachedUsername, UILogin.token
			, (jsel) =>
			{
				OnUnloadThis();

				URemoteData.OnReceiveAvatarData(jsel["avatar"].ToString());
				LoadAnotherUI<UIMain>();
			}, (err) =>
			{
				if (err == "3")
				{
					OnUnloadThis();

					LoadAnotherUI<UICreateAvatar>();
				}
				else
				{
					UIAlert.Show("选择角色失败，" + err);
				}
			}, (err) =>
			{
				UIAlert.Show("选择角色失败，" + err);
			});
	}
}

