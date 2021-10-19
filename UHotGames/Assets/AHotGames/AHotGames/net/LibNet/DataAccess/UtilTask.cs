#if ENABLE_NETWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace LibNet.DataAccess
{
    public enum ETaskExecResult
    {
        eTaskExecResult_Ok,
        eTaskExecResult_Failed,
        eTaskExecResult_Pedding,
    };

    public abstract class CLongRunningTask
    {
        public CLongRunningTask() { }
        public virtual bool IsShutdownTask() { return false; }

        public abstract ETaskExecResult Execute();
        public abstract void Finished();
    };

    public class CShutdownTask : CLongRunningTask
    {
        public override bool IsShutdownTask() { return true; }
        public override ETaskExecResult Execute() { return ETaskExecResult.eTaskExecResult_Ok; }
        public override void Finished() { }


        public static CLongRunningTask Create()
        {
            return new CShutdownTask();
        }
    };

    public class CLongRunningTaskMgr : ITickableSystem
    {
        private static CLongRunningTaskMgr s_Instance = null;
        internal CLongRunningTaskMgr() { }
        static public CLongRunningTaskMgr Instance
        {
            get
            {
                if (null == s_Instance)
                {
                    s_Instance = new CLongRunningTaskMgr();
                }
                return s_Instance;
            }
        }

		public static int maxProcessPerWork = 100;
        public static void WorkThreadProc()
        {
			int iresult = 1;
			int count = 0;
			do
			{
				iresult = CLongRunningTaskMgr.Instance.Process();
				count++;
				if (iresult == 0 || count > maxProcessPerWork)
				{
					count = 0;
					Thread.Sleep(1);
				}
			}
            while (iresult > -1);
        }

        public void Initialize(Int32 uiNumWorkers)
        {
            Shutdown();
            m_uiMaxSimulaniousTasks = uiNumWorkers;
            for (Int32 ii = 0; ii < m_uiMaxSimulaniousTasks; ++ii)
            {
                Thread newThread = new Thread(new ThreadStart(WorkThreadProc));
                m_vThreads.Add(newThread);
                newThread.Start();
            }
        }

        public void Shutdown()
        {
            if (m_vThreads.Count == 0)
            {
                return;
            }
            foreach (Thread thread in m_vThreads)
            {
                AddTask(CShutdownTask.Create());
            }
            foreach (Thread thread in m_vThreads)
            {
                thread.Join();
            }
            m_vThreads.Clear();

            Tick(0);// let callbacks execute for task that remained
        }

        public void AddTask(CLongRunningTask pTask)
        {
            lock (m_TaskLockObj)
            {
                m_vTasks.Enqueue(pTask);
            }
            // - Process some pending finished tasks
            //Tick(0); 
        }

        public void Tick(double fDeltaSec)
        {
            List<CLongRunningTask> vFinishedTasks = new List<CLongRunningTask>();
            lock (m_FinishedLockObj)
            {
                vFinishedTasks.AddRange(m_vFinishedTasks);
                m_vFinishedTasks.Clear();
            }
            foreach (CLongRunningTask pTask in vFinishedTasks)
            {
                pTask.Finished();
            }

            vFinishedTasks.Clear();
        }

		public void OnPrint()
		{
			Console.WriteLine("task count " + m_vTasks.Count);
		}

        private int Process()
        {
            CLongRunningTask task = null;
            lock (m_TaskLockObj)
            {
                if (m_vTasks.Count <= 0)
                {
                    return 0;
                }
				task = m_vTasks.Dequeue();
            }
            if (task == null) return 1;
            if (task.IsShutdownTask()) return -1;

            ETaskExecResult eResult = task.Execute();
            switch (eResult)
            {
                case ETaskExecResult.eTaskExecResult_Ok:
                    {
                        lock (m_FinishedLockObj)
                        {
                            m_vFinishedTasks.Add(task);
                        }
                        break;
                    }
                case ETaskExecResult.eTaskExecResult_Failed:
                    {
                        //SystemLogger.Log.Error("Task Execute Failed!Finished it! ");
                        lock (m_FinishedLockObj)
                        {
                            m_vFinishedTasks.Add(task);
                        }
                        break;
                    }
                case ETaskExecResult.eTaskExecResult_Pedding:
                    {
                        lock (m_TaskLockObj)
                        {
							m_vTasks.Enqueue(task);
                        }
                        break;
                    }
                default:
                    return 1;
            }
            return 1;
        }

        private Int32 m_uiMaxSimulaniousTasks;
        private Queue<CLongRunningTask> m_vTasks = new Queue<CLongRunningTask>();
        private List<CLongRunningTask> m_vFinishedTasks = new List<CLongRunningTask>();
        List<Thread> m_vThreads = new List<Thread>();

        private object m_TaskLockObj = new object();
        private object m_FinishedLockObj = new object();

    };

}

#else
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace LibNet.DataAccess
{
	public enum ETaskExecResult
	{
		eTaskExecResult_Ok,
		eTaskExecResult_Failed,
		eTaskExecResult_Pedding,
	};

	public abstract class CLongRunningTask
	{
		public CLongRunningTask() { }
		public virtual bool IsShutdownTask() { return false; }

		public abstract ETaskExecResult Execute();
		public abstract void Finished();
	};

	public class CShutdownTask : CLongRunningTask
	{
		public override bool IsShutdownTask() { return true; }
		public override ETaskExecResult Execute() { return ETaskExecResult.eTaskExecResult_Ok; }
		public override void Finished() { }

		public static CLongRunningTask Create()
		{
			return new CShutdownTask();
		}
	};

	public class CLongRunDataRequestTask : CLongRunningTask
	{
		public override bool IsShutdownTask() { return false; }
		public override ETaskExecResult Execute() { return ETaskExecResult.eTaskExecResult_Ok; }
		public void Process(object o)
		{
			CLongRunningTaskMgr.Instance.Process(m_pRequest);
		}
		public override void Finished() { }

		public CLongRunDataRequestTask(CDataAccessTask pRequest)
		{
			m_pRequest = pRequest;
		}
		CDataAccessTask m_pRequest;
	};

	public class CLongRunningTaskMgr : ITickableSystem
	{
		private static CLongRunningTaskMgr s_Instance = null;
		internal CLongRunningTaskMgr() { }
		static public CLongRunningTaskMgr Instance
		{
			get
			{
				if (null == s_Instance)
				{
					s_Instance = new CLongRunningTaskMgr();
				}
				return s_Instance;
			}
		}

		public void Initialize()
		{
		}

		private ConcurrentDictionary<string, int> dTaskCount = new ConcurrentDictionary<string, int>();
		public void AddTask(CLongRunningTask pTask, bool bPending = false)
		{
			if (pTask is CDataAccessTask)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(new CLongRunDataRequestTask(pTask as CDataAccessTask).Process));
			}
		}

		public void Tick(double fDeltaSec)
		{
		}

		public void OnPrint()
		{
			int count = 0;
			foreach (string str in dTaskCount.Keys)
			{
				if (dTaskCount[str] == 0)
				{
					continue;
				}
				count += dTaskCount[str];
				AOutput.Log(str + ":" + dTaskCount[str]);
			}
			AOutput.Log("total tasks:" + count);
		}

		public static int[] taskExcuteCount = new int[] { 0, 0, 0 };
		public static int[] maxTaskExcuteCount = new int[] { 0, 0, 0 };
		static DateTime[] oldTime = new DateTime[] { DateTime.Now, DateTime.Now, DateTime.Now };
		public void Process(CLongRunningTask task)
		{
			ETaskExecResult eResult = task.Execute();
			switch (eResult)
			{
				case ETaskExecResult.eTaskExecResult_Ok:
					{
						EngineControler.Instance.QueForProcessing(new LongRunTaskFinishTask(task));

						if ((DateTime.Now - oldTime[0]).TotalMilliseconds > 1000)
						{
							if (maxTaskExcuteCount[0] < taskExcuteCount[0])
							{
								maxTaskExcuteCount[0] = taskExcuteCount[0];
							}
							taskExcuteCount[0] = 0;
							oldTime[0] = DateTime.Now;
						}
						taskExcuteCount[0]++;
						break;
					}
				case ETaskExecResult.eTaskExecResult_Failed:
					{
						EngineControler.Instance.QueForProcessing(new LongRunTaskFinishTask(task));

						if ((DateTime.Now - oldTime[1]).TotalMilliseconds > 1000)
						{
							if (maxTaskExcuteCount[1] < taskExcuteCount[1])
							{
								maxTaskExcuteCount[1] = taskExcuteCount[1];
							}
							taskExcuteCount[1] = 0;
							oldTime[1] = DateTime.Now;
						}
						taskExcuteCount[1]++;
						break;
					}
				case ETaskExecResult.eTaskExecResult_Pedding:
					{
						AddTask(task, true);

						if ((DateTime.Now - oldTime[2]).TotalMilliseconds > 1000)
						{
							if (maxTaskExcuteCount[2] < taskExcuteCount[2])
							{
								maxTaskExcuteCount[2] = taskExcuteCount[2];
							}
							taskExcuteCount[2] = 0;
							oldTime[2] = DateTime.Now;
						}
						taskExcuteCount[2]++;
						break;
					}
				default:
					return;
			}
		}
		private void Process(object o)
		{
			CLongRunningTask task = null;
			if (m_vTasks.Count > 0)
			{
				m_vTasks.TryDequeue(out task);
			}
			if (task == null) return;
			if (task.IsShutdownTask()) return;

			Process(task);
		}

		private ConcurrentQueue<CLongRunningTask> m_vTasks = new ConcurrentQueue<CLongRunningTask>();
	};

}
#endif