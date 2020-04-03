using System;
using System.Collections.Generic;
using System.Text;

namespace LibServer.GameObj
{
	public class CakeItems : CakeServer
	{
		protected override bool IsMulti => true;
		public CakeItems(string id)
			: base("items", id)
		{
		}
	}
}
