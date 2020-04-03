using LibCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClient.GameObj
{
	public class CakeAvatar : CakeClient
	{
		public static string myID;
		public CakeAvatar(string id, LibPacket.AvatarInfo info)
			: base("pinfo", id, "")
		{
			SetValue(ParamNameDefs.AvatarName, info.avatarName);
		}
	}
}
