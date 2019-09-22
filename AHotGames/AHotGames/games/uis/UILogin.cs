using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UILogin : AHotBase
{
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
	protected override void InitComponents()
	{
		var inputUsername = FindWidget<InputField>("inputUsername");
		if (!string.IsNullOrEmpty(CachedUsername))
		{
			inputUsername.text = CachedUsername;
		}

		var inputPassword = FindWidget<InputField>("inputPassword");
		var btnLogin = FindWidget<Button>("btnLogin");
		btnLogin.onClick.AddListener(() =>
		{
			if (string.IsNullOrEmpty(inputUsername.text))
			{
				return;
			}
			if (string.IsNullOrEmpty(inputPassword.text))
			{
				return;
			}
			btnLogin.enabled = false;
			var username = inputUsername.text;
			var password = inputPassword.text;

			UStaticWebRequests.DoLogin(username, Utils.MD5Hash(password),
				(jres) =>
				{
					btnLogin.enabled = true;
					CachedUsername = username;
					token = jres["token"].ToString();

					OnSelectAvatar();
				}
				, (err) =>
				{
					btnLogin.enabled = true;
					UIAlert.Show("登录失败，" + err);
				}
				, (error) =>
				{
					btnLogin.enabled = true;
					UIAlert.Show("登录失败，网络错误：" + error);
				});
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

