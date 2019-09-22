using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class ADatabaseConfigsManager
{
	public const string tUserData = "tuser";
	public static string userDBConnect = System.Configuration.ConfigurationManager.AppSettings["userdbconnect"];
	public static string userDBName = System.Configuration.ConfigurationManager.AppSettings["userdbname"];
	public static ADBAccessor userDB
	{
		get
		{
			return ADBManager.Get(userDBConnect, userDBName);
		}
	}

	public const string tAvatarData = "tavatar";
	public static string avatarDBConnect = System.Configuration.ConfigurationManager.AppSettings["avatardbconnect"];
	public static string avatarDBName = System.Configuration.ConfigurationManager.AppSettings["avatardbname"];
	public static ADBAccessor avatarDB
	{
		get
		{
			return ADBManager.Get(avatarDBConnect, avatarDBName);
		}
	}

}
