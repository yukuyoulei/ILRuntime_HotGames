using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UMUICreateAvatar : AHotBase
{
    protected override void InitComponents()
    {
        var inputNickname = FindWidget<InputField>("inputNickname");
        var bfemale = false;
        var btnMale = FindWidget<Button>("btnMale");
        var maleSel = FindWidget<Image>(btnMale.transform, "sel");
        var btnFemale = FindWidget<Button>("btnFemale");
        var femaleSel = FindWidget<Image>(btnFemale.transform, "sel");
        btnMale.onClick.AddListener(() =>
        {
            bfemale = false;
            femaleSel.enabled = bfemale;
            maleSel.enabled = !bfemale;
        });
        btnFemale.onClick.AddListener(() =>
        {
            bfemale = true;
            femaleSel.enabled = bfemale;
            maleSel.enabled = !bfemale;
        });
        var btnCreate = FindWidget<Button>("btnCreate");
        btnCreate.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(inputNickname.text))
            {
                return;
            }
            UWebSender.Instance.OnRequest(Utils.BaseURL + "avatarcreate", "username=" + UILogin.CachedUsername + "&token=" + UILogin.token + "&avatarname=" + inputNickname.text, (res) =>
                  {
                      var jres = (JObject)JsonConvert.DeserializeObject(res);
                      if (jres["err"].ToString() == "0")
                      {
                          UIAlert.Show("创建角色成功。");
                      }
                      else
                      {
                          UIAlert.Show("创建角色失败，" + jres["err"]);
                      }
                  }, (err) => { });
        });
        var btnReturn = FindWidget<Button>("btnReturn");
        btnReturn.onClick.AddListener(() =>
        {
            UnloadThis();
            LoadAnother<UIMain>();
        });
    }
}

