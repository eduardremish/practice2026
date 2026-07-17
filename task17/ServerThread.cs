using System;
using System.Collections.Concurrent;
using System.Threading;
using task17;

namespace task17
{
    public class ServerThread
    {
        private readonly BlockingCollection<ICommand> _buffer = new();
        private readonly IScheduler _taskScheduler;
        private readonly Thread _worker;
        private readonly CancellationTokenSource _tokenSource = new();
        private Action _currentStrategy;
        private volatile bool _forceStop = false;

        public Thread Thread => _worker;

        public ServerThread(IScheduler scheduler)
        {
            _taskScheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _currentStrategy = DefaultBehavior;
            _worker = new Thread(Run);
        }

        public void Start() => _worker.Start();

        public void Join() => _worker.Join();

        public void HardStop()
        {
            _forceStop = true;
            _tokenSource.Cancel();
        }

        public void UpdateBehavior(Action strategy)
        {
            _currentStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public void Add(ICommand cmd)
        {
            try
            {
                _buffer.Add(cmd, _tokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void SoftStop()
        {
            _buffer.CompleteAdding();

            UpdateBehavior(() =>
            {
                // 1. Сначала планировщик
                if (_taskScheduler.HasCommand())
                {
                    ICommand cmd = _taskScheduler.Select();
                    if (cmd != null)
                    {
                        ProcessCommand(cmd);
                        return;
                    }
                }
                
                // 2. Потом очередь
                if (_buffer.TryTake(out ICommand queueCmd))
                {
                    ProcessCommand(queueCmd);
                    return;
                }
                
                // 3. Всё пусто — останавливаемся
                HardStop();
            });
        }

        private void Run()
        {
            while (!_forceStop)
            {
                _currentStrategy();
            }
            
            // Добиваем очередь
            while (_buffer.TryTake(out ICommand remaining))
            {
                ProcessCommand(remaining);
            }
        }

        private void ProcessCommand(ICommand cmd)
        {
            try
            {
                cmd.Execute();
                if (cmd is ILongCommand longCmd && !longCmd.IsCompleted)
                {
                    _taskScheduler.Add(longCmd);
                }
            }
            catch (Exception error)
            {
                ExceptionHandler.Handle(cmd, error);
            }
        }

        private void DefaultBehavior()
        {
            // 1. СНАЧАЛА планировщик — чтобы команды чередовались строго
            if (_taskScheduler.HasCommand())
            {
                ICommand scheduledCmd = _taskScheduler.Select();
                if (scheduledCmd != null)
                {
                    ProcessCommand(scheduledCmd);
                    return;
                }
            }

            // 2. Потом очередь
            if (_buffer.TryTake(out ICommand incomingCmd))
            {
                ProcessCommand(incomingCmd);
                return;
            }

            // 3. Блокируемся на очереди
            try
            {
                ICommand nextCmd = _buffer.Take(_tokenSource.Token);
                ProcessCommand(nextCmd);
            }
            catch (OperationCanceledException)
            {
                // просто выходим
            }
            catch (InvalidOperationException)
            {
                // очередь закрыта
            }
        }
    }
}
