using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using LibCommon;
using System.Threading.Tasks;

public class UIMinerLogin : AHotBase
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
	Button btnLogin;
	protected override void InitComponents()
	{
		UICommonWait.Show();
		Task.Run(async () =>
		{
			if (!LibClient.AClientApp.bConnected)
				await LibClient.AClientApp.StartClient();
			UEventListener.Instance.AddProducingAction(() =>
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
		btnLogin = FindWidget<Button>("btnLogin");
		btnLogin.onClick.AddListener(() =>
		{
			Task.Run(async () =>
			{
				if (!LibClient.AClientApp.bConnected)
					await LibClient.AClientApp.StartClient();

				if (!LibClient.AClientApp.bConnected)
				{
					UEventListener.Instance.AddProducingAction(() =>
					{
						btnLogin.enabled = true;

					});
					AOutput.Log($"连接失败！");
					return;
				}

				UEventListener.Instance.AddProducingAction(OnLogin);
			});
			btnLogin.enabled = false;
		});
		var btnRegister = FindWidget<Button>("btnRegister");
		btnRegister.onClick.AddListener(() =>
		{
			OnUnloadThis();

			LoadAnotherUI<UIRegister>();
		});

		UEventListener.Instance.OnRegisterEvent(UEvents.LoginFailed, OnLoginFailedCb);
		UEventListener.Instance.OnRegisterEvent(UEvents.EnterGame, OnEnterGameCb);
		UEventListener.Instance.OnRegisterEvent(UEvents.ServerDisconnected, OnServerDisconnectedCb);
	}

	private static void OnServerDisconnectedCb(UEventBase obj)
	{
		UnloadAllClasses();
		LoadAnotherUI<UIMinerLogin>();
	}

	private void OnEnterGameCb(UEventBase obj)
	{
		var eb = obj as EventEnterGame;
		if (eb.info == null)
		{
			LoadAnotherUI<UIMinerCreateAvatar>();
		}
		else
		{
			LoadAnotherUI<UIMinerMain>();
		}
		OnUnloadThis();
	}

	private void OnLoginFailedCb(UEventBase obj)
	{
		btnLogin.enabled = true;
		UICommonTips.AddTip($"登录失败！");
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
		AClientApis.OnLogin(username, MD5String.Hash32(password), EPartnerID.Test);
	}

}

