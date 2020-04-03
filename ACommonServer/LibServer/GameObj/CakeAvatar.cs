using LibCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibServer.GameObj
{
	public class CakeAvatar : CakeServer
	{
		public CakeAvatar(string id)
			: base("pinfo", id)
		{
		}
		internal void Create(string unionid, EPartnerID ePartnerID, string avatarName, int sex)
		{
			SetValue(ParamNameDefs.UnionID, unionid);
			SetValue(ParamNameDefs.PartnerID, (int)ePartnerID);
			SetValue(ParamNameDefs.AvatarName, avatarName);
			SetValue(ParamNameDefs.AvatarSex, sex);
			SetValue(ParamNameDefs.AvatarLevel, 1);

			SCommonds.AddItem("create", id, InitValueDefs.gold, 10000);
			SCommonds.AddItem("create", id, InitValueDefs.money, 10);
		}
		public LibPacket.AvatarInfo ToPkt()
		{
			var info = new LibPacket.AvatarInfo();
			info.avatarID = id;
			info.avatarName = GetStringValue(ParamNameDefs.AvatarName);
			return info;
		}

	}
}
