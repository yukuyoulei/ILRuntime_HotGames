using System;
using System.Collections.Generic;
using System.Text;

public class ContaCity : ContaBase
{
	public override bool Immortal => true;
	public static ContaCity Instance { get; private set; }
	public ContaCity(int config_id) : base(config_id)
	{
		Instance = this;
	}
	protected override bool IsSingle => false;
}
