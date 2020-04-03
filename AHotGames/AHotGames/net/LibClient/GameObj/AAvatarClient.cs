using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;

namespace LibClient.GameObj
{
	public class AAvatarClient : AAvatarCommon
	{
		public AAvatarClient()
		{
			RegisterAllComponents();
			RegisterAllParams();

			InitAllComponents();
		}

		public string AvatarID
		{
			get { return objID; }
			set { objID = value; }
		}
		public override void RegisterAllComponents()
		{
			RegisterComponent(new AComponentBagCommon(this));
			RegisterComponent(new AComponentParamCommon(this));
		}

		public void FromPkt(AvatarInfo info)
		{
			AvatarID = info.avatarID;
			AvatarName = info.avatarName;
			foreach (var pu in info.lInfos)
			{
				OnSetParamValue(pu.paramName, pu.paramValue);
			}
		}
	}
}
