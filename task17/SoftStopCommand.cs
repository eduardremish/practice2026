using System;
using System.Threading;

namespace task17
{
    public class SoftStopCommand : ICommand
    {
        private readonly ServerThread _thread;
        public SoftStopCommand(ServerThread serverThread)
        {
            _thread = serverThread;
        }

        public void Execute()
        {
            if (Thread.CurrentThread != _thread.Thread)
            {
                throw new InvalidOperationException("Эта команда не может быть выполнена в текущем потоке");
            }
            _thread.SoftStop();
        }
    }
}
