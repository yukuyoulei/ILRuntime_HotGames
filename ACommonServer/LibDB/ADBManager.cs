using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using System.Threading;

public class ADBManager : Singleton<ADBManager>
{
	private static Dictionary<string, ADBAccessor> dDbPool = new Dictionary<string, ADBAccessor>();
	public static ADBAccessor Get(string dbConnect, string dbName)
	{
		if (!dDbPool.ContainsKey(dbConnect + "." + dbName))
		{
			var _DB = new ADBAccessor(dbConnect, dbName);
			dDbPool.Add(dbConnect + "." + dbName, _DB);
		}
		return dDbPool[dbConnect + "." + dbName];
	}

	private Dictionary<string, MongoClient> m_vMongoServer = new Dictionary<string, MongoClient>();
	private MongoClient GetMongoServer(String sInitString)
	{
		if (!m_vMongoServer.ContainsKey(sInitString))
		{
			MongoClient mc = new MongoClient(sInitString);
			m_vMongoServer.Add(sInitString, mc);
		}
		return m_vMongoServer[sInitString];
	}
	public IMongoDatabase GetDB(string sConnect, string sDbName)
	{
		try
		{
			return GetMongoServer(sConnect).GetDatabase(sDbName);
		}
		catch
		{
			return null;
		}
	}
	public void RemoteRequestDB(Action<object> remoteAction)
	{
		ThreadPool.QueueUserWorkItem(new WaitCallback(remoteAction));
	}
}