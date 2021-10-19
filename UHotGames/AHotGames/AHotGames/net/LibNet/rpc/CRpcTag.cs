using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNet.DataAccess;
using LibNet.rpc;
namespace LibNet
{
    public  class CRpcTag : CTagBase
    {
        public CRpcTag(IResponer responser)
        {
            m_responser = responser;
        }
        public IResponer Responser
        {
            get
            {
                return m_responser;
            }
        }
        private IResponer m_responser;
    }
}
