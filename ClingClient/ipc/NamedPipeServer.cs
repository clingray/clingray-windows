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
using System.Diagnostics;

namespace ClingClient.ipc
{
    public class NamedPipeServer : PipeStreamWrapperBase<NamedPipeServerStream>
    {
        public NamedPipeServer(string pipeName)
            : base(pipeName)
        {

        }

        ~NamedPipeServer()
        {
            if (Pipe != null) Pipe.Dispose();
        }

        protected override bool AutoFlushPipeWriter
        {
            get { return true; }
        }

        protected override NamedPipeServerStream CreateStream()
        {
            return new NamedPipeServerStream(PipeName,
                       PipeDirection.InOut,
                       NamedPipeServerStream.MaxAllowedServerInstances,
                       PipeTransmissionMode.Message,
                       PipeOptions.Asynchronous,
                       BUFFER_SIZE,
                       BUFFER_SIZE);
        }

        protected override void ReadFromPipe(object state)
        {
            try
            {
                while (Pipe != null && m_stopRequested == false)
                {
                    if (Pipe.IsConnected == false) Pipe.WaitForConnection();

                    byte[] msg = ReadMessage(Pipe);

                    ThrowOnReceivedMessage(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (!Pipe.IsConnected)
                    Start();
            }
        }
    }
}
