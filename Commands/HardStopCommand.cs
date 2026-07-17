using System;
using System.Threading;
using task17;


namespace Commands
{
    public class HardStopCommand: ICommand
    {
        private readonly ServerThread _serverThread;
        public HardStopCommand(ServerThread serverThread)
        {
            _serverThread = serverThread;
        }

        public void Execute()
        {
            if (Thread.CurrentThread != _serverThread.Thread)
            {
                throw new InvalidOperationException("Эта команда не может быть выполнена в текущем потоке");
            }
            _serverThread.HardStop();
        }
    }
}
