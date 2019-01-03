using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

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
            UWebSender.Instance.OnRequest(Utils.BaseURL + "accountlogin"
                , string.Format("username={0}&password={1}", username, Utils.MD5Hash(password))
                , (result) =>
                {
                    btnLogin.enabled = true;
                    Debug.Log("web result " + result);

                    CachedUsername = username;
                }
                , (error) =>
                {
                    btnLogin.enabled = true;
                    Debug.Log("web error " + error);
                });
        });
        var btnRegister = FindWidget<Button>("btnRegister");
        btnRegister.onClick.AddListener(() =>
        {
            UnloadThisUI();

            LoadAnotherUI<UIRegister>();
        });
    }
}

