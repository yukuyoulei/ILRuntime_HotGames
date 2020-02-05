using System;
using System.Collections.Generic;
using System.IO;
using LibPacket;

public class AFactoryPacket : Singleton<AFactoryPacket>
{
	private Dictionary<int, CallBase> dCallers = new Dictionary<int, CallBase>();
	public void RegistPackets(CallBase caller)
	{
		dCallers.Add(caller.GetSign(), caller);
	}
	public CallBase GetCaller(int ePacket)
	{
		if (!dCallers.ContainsKey(ePacket))
		{
			Console.WriteLine("Not registed caller " + ePacket);
			return null;
		}
		return dCallers[ePacket];
	}
}
public abstract class CallBase
{
	public abstract int GetSign();
	public abstract void Call(IResponer responer, byte[] bytes);
}
public class Caller<T> : CallBase
	where T : PktBase, new()
{
	public override int GetSign()
	{
		return CRC.CRC16(typeof(T).FullName);
	}
	public Caller(Action<IResponer, T> delCaller)
	{
		this.delCaller = delCaller;
	}

	public Action<IResponer, T> delCaller;
	public IResponer responer;
	public override void Call(IResponer responer, byte[] bytes)
	{
		try
		{
			delCaller(responer, PktBase.Deserialize<T>(bytes));
		}
		catch (Exception ex)
		{
			Console.WriteLine("Stack trace:" + ex.StackTrace);
			Console.WriteLine("Message:" + ex.Message);
		}
	}
}
