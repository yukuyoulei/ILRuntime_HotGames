using LibNet;
using System;
using System.Collections.Generic;
using System.Text;

public class CtSystem : SystemBase
{
	public CtSystem(ContaBase conta) : base(conta) { }

	List<DelayFunction> lDelayFunctions = new List<DelayFunction>();
	public DelayFunction AddDelayFunction(double delay, Action action)
	{
		var d = new DelayFunction(delay, action);
		return d;
	}
	List<DelayFunction> lremove = new List<DelayFunction>();
	public override void Tick(double fDeltaSec)
	{
		foreach (var d in lDelayFunctions)
		{
			if (!d.IsTimeOut) continue;
			lremove.Add(d);
		}
		foreach (var r in lremove)
		{
			r.action();
			lDelayFunctions.Remove(r);
		}
	}
}

public class DelayFunction
{
	public DelayFunction(double delay, Action action)
	{
		this.delay = delay;
		this.action = action;
		startTime = DateTime.Now;
	}
	public double delay { get; private set; }
	public Action action { get; private set; }
	public bool bCanceled;
	public DateTime startTime { get; private set; }
	public bool IsTimeOut { get { return (DateTime.Now - startTime).TotalSeconds >= delay; } }
}