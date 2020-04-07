using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibPacket;
using LibCommon;

namespace LibClient
{
    public abstract class AClientComm
    {
        public virtual void resultServerDisconnected() { AOutput.Log($"NotImplemented resultServerDisconnected"); }
        public virtual void rcvLoginCb(bool bSuccess, string uid, EPartnerID ePartnerID) { AOutput.Log($"NotImplemented rcvLoginCb {bSuccess} {uid} {ePartnerID}"); }
        public virtual void rcvEnterGameCb(AvatarInfo info) { AOutput.Log($"NotImplemented rcvEnterGameCb"); }
        public virtual void rcvCreateAvatarCb(PktCreateAvatarResult.EResult eResult, AvatarInfo info) { AOutput.Log($"NotImplemented rcvCreateAvatarCb"); }
        public virtual void rcvContaData(int id) { AOutput.Log($"NotImplemented rcvContaData"); }
        public virtual void rcvParamUpdate() { AOutput.Log($"NotImplemented rcvParamUpdate"); }
        public virtual void rcvEasy(string name, List<int> ints, List<string> strs) { AOutput.Log($"NotImplemented rcvEasy"); }
        public virtual void rcvDailyCheckCb(PktDailyCheckResult.EResult eResult, List<Int2> lItems) { AOutput.Log($"NotImplemented rcvDailyCheckCb"); }
        public virtual void rcvExchangeCb(bool bSuccess) { AOutput.Log($"NotImplemented ExchangeCb"); }
        public virtual void rcvCreateOrderCb(PktCreateOrderResult.EResult eResult, string orderID, string extraInfo) { AOutput.Log($"NotImplemented CreateOrderCb"); }

    }
}
