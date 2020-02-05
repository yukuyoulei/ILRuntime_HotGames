using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LibNet
{
	public class EngineControler : Singleton<EngineControler>
	{
		public void EngineInit()
		{
		}
		internal void QueForTick(ClientConnection client)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(new ClientTickTask(client).DoWork));
		}
		internal void QueForProcessing(ClientConnection client)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(new ClientPacketProcessingTask(client).DoWork));
		}
		internal void QueForProcessing(IProducerConsumerTask task)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(task.DoWork));
		}
		internal void QueForSend(ClientConnection client)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(new ClientPacketSendTask(client).DoWork));
		}

	}
}
