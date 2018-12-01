#if ENABLE_WWW
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
//using Encrypter;
public class MD5String
{
//	public static String Hash32(String sInput)
//	{
//		return MD5.Encrypt (sInput, 32);
//	}

	public static String Hash32(string sInput)
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
	public static string GetMD5HashFromFile(string fileName)
	{
		try
		{
			var file = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
		}
	}

}

public class ByteToHexStr
{
	public static String DoConvert(byte[] bytes)
	{
		string sResult = "";
		if (null != bytes)
		{
			for (int ii = 0; ii < bytes.Length; ++ii)
			{
				sResult += bytes[ii].ToString("X2");
			}
		}
		return sResult;
	}
}
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
//using Encrypter;
public class MD5String
{
	public static String Hash32(string sInput)
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

    public static String Hash32UTF8(string sInput)
    {
        MD5 md5Hasher = MD5.Create();
        byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(sInput));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }
}

public class ByteToHexStr
{
	public static String DoConvert(byte[] bytes)
	{
		string sResult = "";
		if (null != bytes)
		{
			for (int ii = 0; ii < bytes.Length; ++ii)
			{
				sResult += bytes[ii].ToString("X2");
			}
		}
		return sResult;
	}
}
#endif