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

    public static string BaseURL = "https://cn1.ydvrgame.com/felement/";
}

