using System;
using System.Collections.Generic;
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
			return o.ToString().Replace("\r","").Replace("\n", "");
		}
		else
		{
			return $"Invalid array length. {string.Join("-", astr)}";
		}

	}
	public static JObject JsonFormatArray(string tag, List<List<string>> astrs)
	{
		var obj = new JObject();
		var a = new JArray();
		foreach (var astr in astrs)
		{
			if (astr.Count % 2 == 0)
			{
				var o = new JObject();
				for (var i = 0; i < astr.Count; i += 2)
				{
					o[astr[i]] = astr[i + 1];
				}
				a.Add(o);
			}
			else
			{
				return null;
			}
		}
		obj[tag] = a;
		return obj;
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
		var str = JsonFormat(new string[] { "err", err });
		HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
		return result;
	}
	public static HttpResponseMessage GetJsonResponse(JObject jobj)
	{
		jobj["err"] = 0;
		HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(jobj.ToString(), Encoding.GetEncoding("UTF-8"), "application/json") };
		return result;
	}
	public static HttpResponseMessage GetJsonResponse(params string[] astr)
	{
		var str = JsonFormat(astr);
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