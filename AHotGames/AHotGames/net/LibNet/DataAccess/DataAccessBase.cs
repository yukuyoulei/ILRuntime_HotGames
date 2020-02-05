#if ENABLE_NETWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LibNet.DataAccess
{
    public abstract class CDataHandlerBase
	{
		public abstract bool InitHandler(String sInitString, string dbName);
	}

    public abstract class CTagBase
    {

    }
    public class CDataRequest
	{
		public CDataHandlerBase DataHandler
        {
            get
            {
                return m_pDataHandler;
            }
            set
            {
                 m_pDataHandler = value;
            }
        }
            
        public CTagBase Tag
        {
            get
            {
                return m_pTag;
            }
            set
            {
                m_pTag = value; 
            }
        }
		public int dbRequestID { get; set; }
		public virtual bool     Execute(){ return false;}
		public virtual void     Finished(){}
	
		private CDataHandlerBase m_pDataHandler;
        private CTagBase m_pTag;
		private int m_dbRequestID;
	};

    public class CDataRequestTpl<TRequestType> : CDataRequest
        where TRequestType : CDataRequest
    {
        public delegate void TRequestCallback(TRequestType request);
        public CDataRequestTpl(TRequestCallback callback )
        {
           m_fncCallback= callback;
        }
        public override void Finished()
		{
            if (null != m_fncCallback)
            {
                m_fncCallback(this as TRequestType);
            }
		}

		private TRequestCallback m_fncCallback;

    }


    public abstract class CDataAccessBase
    {
        public abstract ETaskExecResult ExecuteRequest(CDataRequest pRequest);
        public abstract void FinishTask(CDataRequest pRequest);
    }
    public class CDataAccessTpl<TDataHanderImplType> : CDataAccessBase
        where TDataHanderImplType : CDataHandlerBase, new()
	{
		public bool InitializeHandlers(Int32 iCount, String sInitString, string sDBName)
		{
			AOutput.Log("init-string:" + sInitString + " db-name:" + sDBName);
            for (Int32 ii = 0; ii < iCount; ii++)
			{
                CDataHandlerBase pDataHandler = new TDataHanderImplType();
                
				if(pDataHandler.InitHandler(sInitString, sDBName))
                {
                    m_vAvailableDataHandlers.Enqueue(pDataHandler);
                }
			}
			return m_vAvailableDataHandlers.Count()>0;
		}

        public override  void FinishTask(CDataRequest pRequest)
		{
            lock(m_DBConnetionMutex)
			{
				m_vAvailableDataHandlers.Enqueue( pRequest.DataHandler);
			}
			pRequest.Finished();
		}

		public override ETaskExecResult ExecuteRequest(CDataRequest pRequest)
		{
			CDataHandlerBase pHandler=null;
            lock(m_DBConnetionMutex)
			{
                if (m_vAvailableDataHandlers.Count == 0) return ETaskExecResult.eTaskExecResult_Pedding;

                pHandler = m_vAvailableDataHandlers.Dequeue();
				
			}
            if (pHandler==null) return ETaskExecResult.eTaskExecResult_Pedding;

            pRequest.DataHandler = pHandler;

            if (pRequest.Execute())
                return ETaskExecResult.eTaskExecResult_Ok;
			else
                return ETaskExecResult.eTaskExecResult_Failed;

		}

		protected CDataHandlerBase GetActiveDataHandler()
		{
			CDataHandlerBase pHandler=null;
			while(pHandler==null)
			{
                lock(m_DBConnetionMutex)
                {
                    if(m_vAvailableDataHandlers.Count>0) 
				    {
					   pHandler = m_vAvailableDataHandlers.Dequeue();
					   break;	
				    }
				    else
				    {
					    System.Threading.Thread.Sleep(1);
				    }
                }
			}
			return pHandler;
		}
		protected void RestoreDataHandler(CDataHandlerBase pHandler)
		{
            lock(m_DBConnetionMutex)
            {
                m_vAvailableDataHandlers.Enqueue(pHandler);
            }
			
		}
		private Object m_DBConnetionMutex = new Object();
		private Queue<CDataHandlerBase> m_vAvailableDataHandlers=new Queue<CDataHandlerBase>();
		
	}
	
}

#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;

namespace LibNet.DataAccess
{
	public abstract class CDataHandlerBase
	{
		public abstract bool InitHandler(String p1, String p2, String p3, String p4);
	}

	public abstract class CTagBase
	{

	}
	public class CDataRequest
	{
		public CDataHandlerBase DataHandler { get; set; }
		public CTagBase Tag { get; set; }
		public int dbRequestID { get; set; }
		public virtual bool Execute() { return false; }
		public virtual void Finished() { }
	};

	public class CDataRequestTpl<TRequestType> : CDataRequest
		where TRequestType : CDataRequest
	{
		public delegate void TRequestCallback(TRequestType request);
		public CDataRequestTpl(TRequestCallback callback)
		{
			m_fncCallback = callback;
		}
		public override void Finished()
		{
			if (null != m_fncCallback)
			{
				m_fncCallback(this as TRequestType);
			}
		}

		private TRequestCallback m_fncCallback;

	}


	public abstract class CDataAccessBase
	{
		public abstract ETaskExecResult ExecuteRequest(CDataRequest pRequest);
		public abstract void FinishTask(CDataRequest pRequest);
	}
	public class CDataAccessTpl<TDataHanderImplType> : CDataAccessBase
		where TDataHanderImplType : CDataHandlerBase, new()
	{
		public bool InitializeHandlers(Int32 iCount, String pGameDBHost, String pGameDBName, String pCommonDBHost = "", String pCommonDBName = "")
		{
			AOutput.Log(String.Format("init-string: {0}:{1},{2}:{3}", pGameDBHost, pGameDBName, pCommonDBHost, pCommonDBName));
			for (Int32 ii = 0; ii < iCount; ii++)
			{
				CDataHandlerBase pDataHandler = new TDataHanderImplType();

				if (pDataHandler.InitHandler(pGameDBHost, pGameDBName, pCommonDBHost, pCommonDBName))
				{
					m_vAvailableDataHandlers.Enqueue(pDataHandler);
				}
			}
			return m_vAvailableDataHandlers.Count() > 0;
		}

		public override void FinishTask(CDataRequest pRequest)
		{
			pRequest.Finished();
		}

		public override ETaskExecResult ExecuteRequest(CDataRequest pRequest)
		{
			CDataHandlerBase pHandler = null;
			if (m_vAvailableDataHandlers.Count == 0) return ETaskExecResult.eTaskExecResult_Pedding;
			m_vAvailableDataHandlers.TryDequeue(out pHandler);
			if (pHandler == null) return ETaskExecResult.eTaskExecResult_Pedding;

			pRequest.DataHandler = pHandler;

			bool bSuccess = pRequest.Execute();
			m_vAvailableDataHandlers.Enqueue(pRequest.DataHandler);
			return bSuccess ? ETaskExecResult.eTaskExecResult_Ok : ETaskExecResult.eTaskExecResult_Failed;
		}
		protected void RestoreDataHandler(CDataHandlerBase pHandler)
		{
			m_vAvailableDataHandlers.Enqueue(pHandler);
		}
		private ConcurrentQueue<CDataHandlerBase> m_vAvailableDataHandlers = new ConcurrentQueue<CDataHandlerBase>();

	}

}
#endif