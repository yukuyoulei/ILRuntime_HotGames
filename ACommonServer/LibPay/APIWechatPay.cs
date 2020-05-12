using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

public static class APIWechatPay
{
	private static string WeiXinPayUrl = "https://api.mch.weixin.qq.com/pay/unifiedorder";
	private static string packageStr = "Sign=WXPay";

	/// <summary>
	/// 微信支付商户号（从微信发给你的邮件中获得的）
	/// </summary>
	private static string MchId = "";

	/// <summary>
	/// 应用的APPID（微信发给你的邮件中也有这项内容，一般以wx开头，微信开放平台-管理中心-应用详情也可以看到这项内容）
	/// </summary>
	private static string AppId = "";//appid
	private static string ApiSecret = "";//appsecret
	/// <summary>
	/// 这里是API密钥，不是Appsecret，这里最容易出错了！请务必注意！
	/// 设置方法：微信商户平台(pay.weixin.qq.com)-->账户设置-->API安全-->密钥设置
	/// </summary>
	public static string ApiKey = "";

	/// <summary>
	/// 支付成功后，微信会请求这个路径，
	/// </summary>
	private static string NotifyUrl = "https://www.wechatpay.com/classroom/ordercallback";

	/// <summary>
	/// 开发发起支付
	/// </summary>
	/// <param name="TotalFee">总金额，单位：分，不能为空</param>
	/// <param name="TradeNo">订单号，你自己定就好了，不要重复，不能为空</param>
	/// <param name="Des">订单描述，不能为空</param>
	/// <param name="ClientIp">客户端的IP地址，不能为空</param>
	/// <param name="FeeType">货币类型，默认是CNY，人民币</param>
	/// <returns></returns>
	public static string Pay(Int64 TotalFee, string TradeNo, string Des, string ClientIp = "127.0.0.1", string trade_type = "APP", string FeeType = "CNY")
	{
		//为发送请求给微信服务器准备数据
		var nstr = MakeNonceStr();
		Hashtable packageParameter = new Hashtable();
		packageParameter.Add("appid", AppId);
		packageParameter.Add("body", Des);
		packageParameter.Add("mch_id", MchId);
		packageParameter.Add("notify_url", NotifyUrl);
		packageParameter.Add("nonce_str", nstr);
		packageParameter.Add("out_trade_no", TradeNo);
		packageParameter.Add("total_fee", TotalFee.ToString());
		packageParameter.Add("spbill_create_ip", ClientIp);
		packageParameter.Add("trade_type", trade_type);
		packageParameter.Add("fee_type", FeeType);
		var sign = CreateMd5Sign(packageParameter);
		packageParameter.Add("sign", sign);
		var xe = PostDataToWeiXin(packageParameter);
		//为响应客户端的请求准备数据 
		var timeStamp = GetTimeStamp();
		var pv = xe.Element("prepay_id");
		if (pv == null)
		{
			return xe.ToString();
		}
		var prepayId = pv.Value;
		nstr = xe.Element("nonce_str").Value;
		Hashtable paySignReqHandler = new Hashtable();
		paySignReqHandler.Add("appid", AppId);
		paySignReqHandler.Add("partnerid", MchId);
		paySignReqHandler.Add("prepayid", prepayId);
		paySignReqHandler.Add("package", "Sign=WXPay");
		paySignReqHandler.Add("noncestr", nstr);
		paySignReqHandler.Add("timestamp", timeStamp);
		var paySign = CreateMd5Sign(paySignReqHandler);
		var obj = new
		{
			appid = AppId,
			partnerid = MchId,
			prepayid = prepayId,
			package = packageStr,
			noncestr = nstr,
			timestamp = timeStamp,
			sign = paySign
		};
		return JsonConvert.SerializeObject(obj);
	}
	public static string GetTimeStamp()
	{
		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return Convert.ToInt64(ts.TotalSeconds).ToString();
	}
	private static XElement PostDataToWeiXin(Hashtable parameters)
	{
		var xmlStr = getXmlStr(parameters);
		var data = Encoding.UTF8.GetBytes(xmlStr);
		Stream responseStream;
		HttpWebRequest request = WebRequest.Create(WeiXinPayUrl) as HttpWebRequest;
		request.ContentType = "application/x-www-form-urlencoded";
		request.Method = "POST";
		request.ContentLength = data.Length;
		Stream requestStream = request.GetRequestStream();
		requestStream.Write(data, 0, data.Length);
		requestStream.Close();
		try
		{
			responseStream = request.GetResponse().GetResponseStream();
		}
		catch (Exception exception)
		{
			throw exception;
		}
		string str = string.Empty;
		using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
		{
			str = reader.ReadToEnd();
		}
		responseStream.Close();
		var xe = XElement.Parse(str);
		return xe;
	}


	private static string getXmlStr(Hashtable parameters)
	{
		var sb = new StringBuilder();
		sb.Append("<xml>");
		foreach (string k in parameters.Keys)
		{
			var v = (string)parameters[k];
			if (Regex.IsMatch(v, @"^[0-9.]$"))
			{
				sb.Append("<" + k + ">" + v + "</" + k + ">");
			}
			else
			{
				sb.Append("<" + k + "><![CDATA[" + v + "]]></" + k + ">");
			}
		}
		sb.Append("</xml>");
		return sb.ToString();
	}
	public static string CreateMd5Sign(Hashtable parameters)
	{
		var sb = new StringBuilder();
		var akeys = new ArrayList(parameters.Keys);
		akeys.Sort();//排序，这是微信要求的
		foreach (string k in akeys)
		{
			var v = (string)parameters[k];
			sb.Append(k + "=" + v + "&");
		}
		sb.Append("key=" + ApiKey);
		string sign = GetMD5(sb.ToString());
		return sign;
	}


	public static string MakeNonceStr()
	{
		var timestap = DateTime.Now.ToString("yyyyMMddhhmmssffff");
		return GetMD5(timestap);
	}
	public static string GetMD5(string src)
	{
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] data = Encoding.UTF8.GetBytes(src);
		byte[] md5data = md5.ComputeHash(data);
		md5.Clear();
		var retStr = BitConverter.ToString(md5data);
		retStr = retStr.Replace("-", "").ToUpper();
		return retStr;
	}
}