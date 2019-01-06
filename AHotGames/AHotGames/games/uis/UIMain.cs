using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class UIMain : AHotBase
{
    protected override void InitComponents()
    {
        var textUsername = FindWidget<Text>("textUsername");
        textUsername.text = UILogin.CachedUsername;

        var btnLogout = FindWidget<Button>("btnLogout");
        btnLogout.onClick.AddListener(() =>
        {
            UWebSender.Instance.OnRequest(Utils.BaseURL + "accountlogout"
                , "username=" + UILogin.CachedUsername + "&token=" + UILogin.token, (res) =>
               {
                   var jres = (JObject)JsonConvert.DeserializeObject(res);
                   var err = jres["err"].ToString();
                   if (err == "0")
                   {
                       UnloadThisUI();
                       LoadAnotherUI<UILogin>();
                   }
                   else
                   {
                       UIAlert.Show("注销失败：" + Utils.ErrorFormat(err));
                   }
               }, (fail) =>
               {
                   UIAlert.Show("web error:" + fail);
               });

        });
    }
}

