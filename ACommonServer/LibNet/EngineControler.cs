using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibNet
{
	public class EngineControler : Singleton<EngineControler>
	{
		private static List<IProducerConsumerTask> ltasks;
		public void EngineInit()
		{
			ltasks = new List<IProducerConsumerTask>();
			Task.Run(TickMainThread);
		}

		private void TickMainThread()
		{
			while (true)
			{
				var lt = ltasks.ToArray();
				foreach (var t in lt)
				{
					if (t is ExitThreadTask)
					{
						AOutput.LogError("ExitThreadTask!");
						return;         // This signals our exit
					}
					ltasks.Remove(t);
					t.DoWork();
				}
				Thread.Sleep(1);
			}
		}

		internal void QueForTick(ClientConnection client)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(new ClientTickTask(client).DoWork));
		}
		internal void QueForProcessing(ClientConnection client)
		{
			ltasks.Add(new ClientPacketProcessingTask(client));
		}
		internal void QueForProcessing(IProducerConsumerTask task)
		{
			ltasks.Add(task);
		}
		internal void QueForSend(ClientConnection client)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(new ClientPacketSendTask(client).DoWork));
		}

	}
}
