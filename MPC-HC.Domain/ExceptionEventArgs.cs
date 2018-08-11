using System;

namespace MPC_HC.Domain
{
    public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);

    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get;}

        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }

    }
}