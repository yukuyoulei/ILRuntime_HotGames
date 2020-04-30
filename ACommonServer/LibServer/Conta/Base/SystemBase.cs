using System;
using System.Collections.Generic;
using System.Text;

public abstract class SystemBase : LibNet.ITickableSystem
{
	public ContaBase conta { get; private set; }
	public SystemBase(ContaBase conta)
	{
		this.conta = conta;
	}
	public abstract void Tick(double fDeltaSec);
}

