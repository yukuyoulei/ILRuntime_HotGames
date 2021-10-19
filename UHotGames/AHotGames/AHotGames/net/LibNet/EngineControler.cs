using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibNet
{
	public class EngineControler : Singleton<EngineControler>
	{
		private static ConcurrentQueue<IProducerConsumerTask> ltasks;
		public void EngineInit()
		{
			ltasks = new ConcurrentQueue<IProducerConsumerTask>();
			Task.Run(() => { TickMainThread(); });
		}

		private void TickMainThread()
		{
			while (true)
			{
				IProducerConsumerTask t;
				while(ltasks.TryDequeue(out t))
				{
					if (t is ExitThreadTask)
					{
						AOutput.LogError("ExitThreadTask!");
						return;         // This signals our exit
					}
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
			ltasks.Enqueue(new ClientPacketProcessingTask(client));
		}
		internal void QueForProcessing(IProducerConsumerTask task)
		{
			ltasks.Enqueue(task);
		}
		internal void QueForSend(ClientConnection client)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(new ClientPacketSendTask(client).DoWork));
		}

	}
}
