using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClingClientEngine
{
    public interface ProgressIF
    {
        void sendMsg(string msg, double val);
    }
}
