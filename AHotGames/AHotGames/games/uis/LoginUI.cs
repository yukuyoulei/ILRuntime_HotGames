using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class LoginUI : AHotBase
{
    protected override void InitComponents()
    {
        var inputUsername = FindWidget<InputField>("inputUsername");
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
            UWebSender.Instance.OnRequest("http://fscoding.top/common/accountlogin"
                , string.Format("username={0}&password={1}", inputUsername.text, inputPassword.text)
                , (result) =>
                {
                    Debug.Log("web result " + result);
                }
                , (error) =>
                {
                    Debug.Log("web error " + error);
                });
        });
    }
}

