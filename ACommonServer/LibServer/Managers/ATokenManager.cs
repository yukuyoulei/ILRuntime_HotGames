using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;

namespace LibServer.Managers
{
	public class ATokenManager : Singleton<ATokenManager>
	{
		private Dictionary<KeyValuePair<PktLoginRequest.EPartnerID, string>, string> dTokens = new Dictionary<KeyValuePair<PktLoginRequest.EPartnerID, string>, string>();
		public string AddToken(PktLoginRequest.EPartnerID ePartnerID, string uid)
		{
			var kv = new KeyValuePair<PktLoginRequest.EPartnerID, string>(ePartnerID, uid);
			if (dTokens.ContainsKey(kv))
				dTokens.Remove(kv);
			var token = Guid.NewGuid().ToString();
			dTokens.Add(kv, token);
			return token;
		}
		public string GetToken(PktLoginRequest.EPartnerID ePartnerID, string uid)
		{
			var kv = new KeyValuePair<PktLoginRequest.EPartnerID, string>(ePartnerID, uid);
			if (dTokens.ContainsKey(kv))
			{
				var token = dTokens[kv];
				dTokens.Remove(kv);
				return token;
			}
			return "";
		}
	}
}
