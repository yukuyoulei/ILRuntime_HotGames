using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public static class Utils
{
    public static String MD5Hash(string sInput)
    {
        MD5 md5Hasher = MD5.Create();
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(sInput));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }

    public static string BaseURL = "http://www.xhthink.com/hotgame/";

    private static Dictionary<string, string> dErrors;
    public static string ErrorFormat(string error)
    {
        if (dErrors == null)
        {
            dErrors = new Dictionary<string, string>();
			dErrors.Add("2", "无效的token。");
            dErrors.Add("1", "账号密码错误。");
            dErrors.Add("-1", "系统错误。");
            dErrors.Add("-2", "重复的用户名。");
            dErrors.Add("-3", "无效的用户名长度。");
            dErrors.Add("-4", "无效的邮箱地址。");
        }
        if (dErrors.ContainsKey(error))
            return dErrors[error];
        return error;
    }
}

