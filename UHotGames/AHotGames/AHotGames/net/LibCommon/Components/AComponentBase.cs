using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public abstract class AComponentBase
{
	public AGameObj owner;
	public AComponentBase(AGameObj owner)
	{
		this.owner = owner;
	}
	public virtual void OnDispose()
	{
		owner = null;
	}
	public abstract void InitComponent();
}
