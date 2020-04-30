using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;
using LibCommon;

namespace LibServer.Managers
{
	public class AContaManager : Singleton<AContaManager>, LibNet.ITickableSystem
	{
		Dictionary<int, List<ContaBase>> dAllContas = new Dictionary<int, List<ContaBase>>();
		Dictionary<string, ContaBase> dPlayerInContas = new Dictionary<string, ContaBase>();
		public Entity OnEnterConta(string psid, int pid)
		{
			if (string.IsNullOrEmpty(psid)) return null;
			OnLeaveConta(psid);
			if (!dAllContas.ContainsKey(pid))
				dAllContas.Add(pid, new List<ContaBase>());
			var conta = GetNewConta(pid, psid);
			dPlayerInContas.Add(psid, conta);
			return conta.PlayerSystem.Create(psid);
		}

		private ContaBase GetNewConta(int pid, string psid)
		{
			foreach (var c in dAllContas[pid])
			{
				if (c.CanEnter(psid)) return c;
			}
			var conta = ContaFactory(pid);
			dAllContas[pid].Add(conta);
			return conta;
		}

		public ContaBase OnGetConta(string psid)
		{
			if (dPlayerInContas.ContainsKey(psid))
				return dPlayerInContas[psid];
			return null;
		}

		private ContaBase ContaFactory(int pid)
		{
			switch (pid)
			{
				case 1:
					return new ContaCity(pid);
				default:
					var data = MapLoader.Instance.OnGetData(pid);
					return new ContaSingle(pid, data.wuxing);
			}
			throw new Exception("Invalid conta type {pid}");
		}

		public void OnLeaveConta(string psid)
		{
			if (!dPlayerInContas.ContainsKey(psid)) return;
			dPlayerInContas[psid].leave(psid);
			dPlayerInContas.Remove(psid);
		}
		internal void OnRemove(ContaBase contaBase)
		{
			contaBase.bRemove = true;
		}

		internal void OnOffline(string psid)
		{
			OnLeaveConta(psid);
		}

		public void Tick(double fDeltaSec)
		{
			var lremove = new List<ContaBase>();
			foreach (var contas in dAllContas)
			{
				lremove.Clear();
				foreach (var c in contas.Value)
				{
					if (c.bRemove)
					{
						lremove.Add(c);
						continue;
					}
					c.Tick(fDeltaSec);
				}
				foreach (var r in lremove)
				{
					r.Dispose();
					contas.Value.Remove(r);
				}
			}
		}
	}
}
