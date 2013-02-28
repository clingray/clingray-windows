using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClingClient.ipc
{
    [Serializable()]
    public class ClingStartupCommand
    {
        public ClingStartupCommand()
        {
        }

        public ClingStartupCommand(int action, string parameter)
        {
            this.action = action;
            this.parameter = parameter;
        }

        public int action { get; set; }
        public string parameter { get; set; }

        public override string ToString()
        {
            return string.Format("action: {0}, parameter: {1}", action, parameter);
        }
    }
}
