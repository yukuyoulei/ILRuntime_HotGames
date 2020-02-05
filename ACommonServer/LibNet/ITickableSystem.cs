using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNet
{
    public interface ITickableSystem
    {
        void Tick(double fDeltaSec);
    }
}
