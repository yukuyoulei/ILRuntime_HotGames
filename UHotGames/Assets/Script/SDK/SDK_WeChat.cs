using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SDK_WeChat : MonoBehaviour
{
#if UNITY_ANDROID
	AndroidJavaClass _wechatSDK;
	AndroidJavaClass wechatSDK
	{
		get
		{
			if (_wechatSDK == null)
			{
				_wechatSDK = new AndroidJavaClass("com.gelunjiaoyu.readbook.WeChatEntry");
			}
			return _wechatSDK;
		}
	}
	static AndroidJavaObject WXApi
	{
		get
		{
			if (_wxApi == null)
			{

				AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaClass apiFactory = new AndroidJavaClass("com.tencent.mm.opensdk.openapi.WXAPIFactory");

				_wxApi = apiFactory.CallStatic<AndroidJavaObject>("createWXAPI", activity, appid, false);
			}
			return _wxApi;
		}
	}
	static AndroidJavaObject _wxApi;
#elif UNITY_IOS
    [DllImport("__Internal")]
    private extern static string WechatPay(string appid, string partnerid, string prepayid, string package, string noncestr, string timestamp, string sign);
    /*[DllImport("__Internal")]
    private extern static string WechatPayWeChatInitialize(string appid);*/
#endif

	public const string appid = "wxb244f08e7ca02c21";
	private const string appsecret = "62a0584a3ffc03f9a08e782bc75d5918";
	private const string partnerid = "1504873861";
	void Start()
	{
#if UNITY_ANDROID
		wechatSDK.CallStatic("Initialize", appid, partnerid);
#elif UNITY_IOS
        /*var sappid = appid;
        WechatPayWeChatInitialize(sappid);*/
#endif
	}

	void OpenWechatPay(string args)
	{
		var aargs = args.Split(',');
		var prepayid = aargs[0];
		var nonceStr = aargs[1];
		var timeStamp = aargs[2];
		var sign = aargs[3];
		var sappid = appid;
		var spartnerid = partnerid;

#if UNITY_EDITOR
		return;
#endif
#if UNITY_ANDROID
		Dictionary<string, string> paras = new Dictionary<string, string>();
		//BY CG:由于wechat android sdk中的变量与签名标志不一致，所以分开写，#前面是签名属性，后面对应的SDK中变量名
		paras.Add("appid#appId", appid);
		paras.Add("partnerid#partnerId", partnerid); //商户号
		paras.Add("prepayid#prepayId", prepayid);
		paras.Add("package#packageValue", "Sign=WXPay"); //暂填写固定值Sign=WXPay
		paras.Add("noncestr#nonceStr", nonceStr); //随机字符串
		paras.Add("timestamp#timeStamp", timeStamp);
		paras.Add("sign#sign", sign); //应用签名，预下单的签名不能用，要重新签名

		var request = new AndroidJavaObject("com.tencent.mm.opensdk.modelpay.PayReq");
		foreach (var kv in paras)
		{
			request.Set(kv.Key.Split('#')[1], kv.Value);
		}
		bool ret = WXApi.Call<bool>("sendReq", request);
		Debug.Log($"OpenWechatPay {ret}");
#elif UNITY_IOS
        WechatPay(sappid, spartnerid, prepayid, "Sign=WXPay", nonceStr, timeStamp, sign);
#endif
	}

	void WXPayCallback(string code)
	{
		Debug.Log($"WXPayCallback {code}");
	}
}
