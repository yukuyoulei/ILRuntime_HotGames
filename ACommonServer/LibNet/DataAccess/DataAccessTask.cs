using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNet.DataAccess
{
    public class CDataAccessTask : CLongRunningTask
	{
        public static CLongRunningTask Create(CDataAccessBase dataAccess,CDataRequest pRequest)
		{
            return new CDataAccessTask(dataAccess,pRequest);
        }
		public override ETaskExecResult Execute()
		{
            return m_DataAccessBase.ExecuteRequest(m_pRequest);
		}

        public override void Finished()
		{
            m_DataAccessBase.FinishTask(m_pRequest);
		}
        private CDataAccessTask(CDataAccessBase dataAccess ,CDataRequest pRequest)
		{
            m_pRequest=pRequest;
            m_DataAccessBase = dataAccess;

		}

        public CDataRequest m_pRequest;
        CDataAccessBase m_DataAccessBase;
	};
}
