using LibClient;
using LibCommon;
using LibPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AClient
{
	public class AOnlineSubsystem : AClientComm
	{
		public override void resultServerDisconnected()
		{
			AOutput.Log($"resultServerDisconnected");
		}
		public override void rcvLoginCb(bool bSuccess, string uid, EPartnerID ePartnerID)
		{
			AOutput.Log($"rcvLoginCb {bSuccess}, {uid}, {ePartnerID}");
		}
		public override void rcvEnterGameCb(AvatarInfo info)
		{
			AOutput.Log($"rcvEnterGameCb [{info.avatarID}]{info.avatarName}");
		}
	}
}
