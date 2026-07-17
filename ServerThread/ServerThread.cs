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
        private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
        private readonly Thread _thread;
        private Action _behavior;
        private bool _stop = false;

        public Thread Thread => _thread;

        public void HardStop()
        {
            _stop = true;
        }

        private void DefaultBehavior()
        {
            try
            {
                ICommand command = _queue.Take();
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(command, ex);
                }
            }
            catch (InvalidOperationException)
            {
                HardStop();
            }

        }

        public void UpdateBehavior(Action newBehavior)
        {
            _behavior = newBehavior ?? throw new ArgumentNullException(nameof(newBehavior));
        }

        public void SoftStop()
        {
            _queue.CompleteAdding();
            UpdateBehavior(() =>
            {
                if (_queue.IsCompleted)
                {
                    HardStop();
                    return;
                }
                DefaultBehavior();
            });
        }

        public void Join()
        {
            _thread.Join();
        }

        private void Run()
        {
            while (!_stop)
            {
                _behavior();
            }
        }

        public ServerThread()
        {
            _behavior = DefaultBehavior;
            _thread = new Thread(Run);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Add( ICommand command)
        {
            try
            {
                _queue.Add(command);
            }
            catch (InvalidOperationException)
            {
            }
            
        }
    }
}
