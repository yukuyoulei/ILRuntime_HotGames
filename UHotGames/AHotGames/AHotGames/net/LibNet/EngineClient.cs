using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNet
{
    using rpc;
    public class EngineClient : Engine
    {
		public static long packetStatistic = 0;

        static public IRPCClient Connect(String sIP,Int32 iPort)
        {
			return RPCClientManager.Instance.CreateRPCClient(sIP, iPort);
        }
        static public void EngineInit()
        {
            EngineControler.Instance.EngineInit();
        }

    }
}
