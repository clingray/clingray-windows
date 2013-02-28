using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Pipes;
using System.IO;
using System.Threading;

namespace ClingClient.ipc
{
    public class ReceivedMessageEventArgs : EventArgs
    {

        public ReceivedMessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
