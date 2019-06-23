using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMMORPG : AHotBase
{
    protected override void InitComponents()
    {
        UICommonWait.Show();
        WebSocketConnector.Instance.OnInit(Utils.WebSocketURL + "username=" + UILogin.CachedUsername + "&token=" + UILogin.token
            , (openEvt) =>
            {
                UICommonWait.Hide();
                LoadAnotherUI<UMUIMain>();
            }, (msgEvt) =>
            {
                
            }, (errorEvt) =>
            {
                UIAlert.Show("WSError:" + errorEvt.Message);
            }, (closeMsg) =>
            {
                UICommonWait.Hide();
            });
    }
}

