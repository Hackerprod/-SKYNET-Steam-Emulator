using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.IPC
{
    /// <summary>
    /// Handles new connections.
    /// </summary>
    /// <typeparam name="T">Reference type</typeparam>
    public class ConnectionEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Connection
        /// </summary>
        public PipeConnection<T> Connection { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public ConnectionEventArgs(PipeConnection<T> connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
    }

    public class ConnectionExceptionEventArgs<T> : ConnectionEventArgs<T>
    {
        /// <summary>
        /// The exception that was thrown
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="exception"></param>
        public ConnectionExceptionEventArgs(PipeConnection<T> connection, Exception exception) : base(connection)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }

    /// <summary>
    /// Handles messages received from a named pipe.
    /// </summary>
    /// <typeparam name="T">Reference type</typeparam>
    public class ConnectionMessageEventArgs<T> : ConnectionEventArgs<T>
    {
        /// <summary>
        /// Message sent by the other end of the pipe
        /// </summary>
        public T Message { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        public ConnectionMessageEventArgs(PipeConnection<T> connection, T message) : base(connection)
        {
            Message = message;
        }
    }

    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        //public Exception Exception { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionEventArgs(/*Exception exception*/)
        {
            //Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
    }
}

