using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibPacket;

namespace LibClient
{
	public abstract class AClientComm
	{
		internal virtual void resultServerDisconnected() { AOutput.Log($"NotImplemented resultServerDisconnected"); }
		internal virtual void rcvItemNotify(PktItemNotify itemNotify) { AOutput.Log($"NotImplemented rcvItemNotify"); }
		internal virtual void rcvLoginCb(bool bSuccess, string uid, PktLoginRequest.EPartnerID ePartnerID) { AOutput.Log($"NotImplemented rcvLoginCb"); }
		internal virtual void rcvEnterGameCb(AvatarInfo info) { AOutput.Log($"NotImplemented rcvItemNotify"); }


	}
}
