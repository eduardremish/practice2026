using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.ExceptionServices;

namespace task17
{
    public class ServerThread
    {
        private readonly BlockingCollection<ICommand> _taskQueue = new BlockingCollection<ICommand>();
        private readonly Thread _workerThread;
        private Action _currentAction;
        private bool _shouldStop = false;

        public Thread Thread => _workerThread;

        public ServerThread()
        {
            _currentAction = DefaultBehavior;
            _workerThread = new Thread(Run);
        }

        public void Add(ICommand cmd)
        {
            try
            {
                _taskQueue.Add(cmd);
            }
            catch (InvalidOperationException)
            {
            }

        }

        public void HardStop()
        {
            _shouldStop = true;
        }

        public void SoftStop()
        {
            _taskQueue.CompleteAdding();
            UpdateBehavior(() =>
            {
                if (_taskQueue.IsCompleted)
                {
                    HardStop();
                    return;
                }
                DefaultBehavior();
            });
        }

        private void DefaultBehavior()
        {
            try
            {
                ICommand cmd = _taskQueue.Take();
                try
                {
                    cmd.Execute();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handler(cmd, ex);
                }
            }
            catch (InvalidOperationException)
            {
                HardStop();
            }

        }

        public void UpdateBehavior(Action nextBehavior)
        {
            _currentAction = nextBehavior ?? throw new ArgumentNullException(nameof(nextBehavior));
        }

        

        public void Join()
        {
            _workerThread.Join();
        }

        private void Run()
        {
            while (!_shouldStop)
            {
                _currentAction();
            }
        }

        public void Start()
        {
            _workerThread.Start();
        }

        
    }
}
