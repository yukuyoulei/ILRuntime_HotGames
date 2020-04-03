using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;

namespace LibServer.Managers
{
	public class AContaManager : Singleton<AContaManager>
	{
		Dictionary<string, ContaBase> dPlayerInContas = new Dictionary<string, ContaBase>();
		List<ContaBase> lContas = new List<ContaBase>();

		public void OnEnterConta(string psid, ContaBase conta)
		{
			if (string.IsNullOrEmpty(psid)) return;
			OnLeaveConta(psid);
			dPlayerInContas.Add(psid, conta);
		}
		public void OnLeaveConta(string psid)
		{
			if (!dPlayerInContas.ContainsKey(psid)) return;
			dPlayerInContas[psid].leave(psid);
			dPlayerInContas.Remove(psid);
		}
		internal void OnRemove(ContaBase contaBase)
		{
			lContas.Remove(contaBase);
		}

		internal void OnOffline(string psid)
		{
			OnLeaveConta(psid); //todo
		}
	}
}
