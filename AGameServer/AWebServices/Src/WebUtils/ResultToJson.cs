using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

public class ResultToJson
{
	public static string JsonFormat(params string[] astr)
	{
		if (astr.Length % 2 == 0)
		{
			var o = new JObject();
			o["err"] = "0";
			for (var i = 0; i < astr.Length; i += 2)
			{
				o[astr[i]] = astr[i + 1];
			}
			return o.ToString();
		}
		else
		{
			return $"Invalid array length. {string.Join("-", astr)}";
		}

	}
	public static string JsonFormat(JObject obj)
	{
		return obj.ToString();
	}
	public static HttpResponseMessage GetErrorJsonResponse(ErrorDefs err)
	{
		return GetErrorJsonResponse((int)err);
	}
	public static HttpResponseMessage GetErrorJsonResponse(int err)
	{
		return GetErrorJsonResponse(err.ToString());
	}
	public static HttpResponseMessage GetErrorJsonResponse(string err = "0")
	{
		String str = JsonFormat(new string[] { "err", err });
		HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
		return result;
	}
	public static HttpResponseMessage GetJsonResponse(params string[] astr)
	{
		String str = JsonFormat(astr);
		HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
		return result;
	}
	public static HttpResponseMessage ToJsonResponse(JObject obj)
	{
		return new HttpResponseMessage { Content = new StringContent(obj.ToString(), Encoding.GetEncoding("UTF-8"), "application/json") };
	}
	public static HttpResponseMessage ToNormalResponse(string content)
	{
		return new HttpResponseMessage { Content = new StringContent(content) };
	}
}