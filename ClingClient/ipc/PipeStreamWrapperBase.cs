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
    public abstract class PipeStreamWrapperBase<T> where T : PipeStream
    {
        protected const int BUFFER_SIZE = 4096;

        /// <summary>
        /// This event fires when a message is received from the pipe.
        /// </summary>
        public event EventHandler<ReceivedMessageEventArgs> OnReceivedMessage;

        
        public PipeStreamWrapperBase(string pipeName)
        {
            if (pipeName == null)
            {
                throw new ArgumentNullException("pipeName", "Argument cannot be null.");
            }

            PipeName = pipeName;
        }

        /// <summary>
        /// The instance of PipeStream.
        /// </summary>
        protected T Pipe { get; set; }

        /// <summary>
        /// StreamWriter for writing messages to the pipe.
        /// </summary>
        protected StreamWriter PipeWriter { get; set; }

        /// <summary>
        /// Creates an instance of T.
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateStream();

        /// <summary>
        /// Should calls to write to the stream through PipeWriter automatically call Flush().
        /// </summary>
        protected abstract bool AutoFlushPipeWriter { get; }
        
        /// <summary>
        /// Method that runs on the ThreadPool for reading messages from the pipe.
        /// </summary>
        /// <param name="state"></param>
        protected abstract void ReadFromPipe(object state);

        public string PipeName
        {
            get;
            private set;
        }

        /// <summary>
        /// Create the named pipe stream and starts the reader thread.
        /// </summary>
        public void Start()
        {
            Pipe = CreateStream();

            ThreadPool.QueueUserWorkItem(new WaitCallback(ReadFromPipe));
        }

        /// <summary>
        /// Requests the reader thread stop and disposes the named pipe.
        /// </summary>
        public void Stop()
        {
            m_stopRequested = true;
            Pipe.Close();
            Pipe.Dispose();
        }

        protected bool m_stopRequested = false;

        /// <summary>
        /// Reads a message from the pipe.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected static byte[] ReadMessage(PipeStream stream)
        {
            MemoryStream memoryStream = new MemoryStream();

            byte[] buffer = new byte[BUFFER_SIZE];

            do
            {
                memoryStream.Write(buffer, 0, stream.Read(buffer, 0, buffer.Length));

            } while (stream.IsMessageComplete == false);

            return memoryStream.ToArray();
        }        

        /// <summary>
        /// Write a message to the pipe if the pipe is connected.
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            if (Pipe.IsConnected == true && Pipe.CanWrite == true)
            {
                if (PipeWriter == null)
                {
                    PipeWriter = new StreamWriter(Pipe);

                    PipeWriter.AutoFlush = AutoFlushPipeWriter;
                }

                WriteToStream(message);
            }
        }

        /// <summary>
        /// Write a message to the pipe's stream.
        /// </summary>
        /// <param name="message"></param>
        protected virtual void WriteToStream(string message)
        {
            PipeWriter.WriteLine(message);
        }

        /// <summary>
        /// Fire the OnReceivedMessage event.
        /// </summary>
        /// <param name="message">The message sent with the event.</param>
        protected void ThrowOnReceivedMessage(byte[] message)
        {
            if (OnReceivedMessage != null)
            {
                OnReceivedMessage(this, new ReceivedMessageEventArgs(Encoding.Default.GetString(message)));
            }
        }
    }
}
