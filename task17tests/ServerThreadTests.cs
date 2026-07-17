using Xunit;
using System;
using System.Linq;
using System.Threading;
using task17;

namespace task17tests
{
    public class BasicCommand : ICommand
    {
        private readonly Action _callback;
        public BasicCommand(Action callback = null) => _callback = callback;
        public void Execute() => _callback?.Invoke();
    }

    public class RepeatingCommand : ILongCommand
    {
        private int _cyclesLeft;
        private readonly Action _callback;
        public bool IsCompleted => _cyclesLeft <= 0;
        public string Label { get; }

        public RepeatingCommand(int totalCycles, string label = "", Action callback = null)
        {
            _cyclesLeft = totalCycles;
            Label = label;
            _callback = callback;
        }

        public void Execute()
        {
            if (!IsCompleted)
            {
                _cyclesLeft--;
                _callback?.Invoke();
            }
        }
    }

    public class QueueScheduler : IScheduler
    {
        private readonly System.Collections.Generic.Queue<ICommand> _pending = new();
        public void Add(ICommand cmd) { if (cmd != null) _pending.Enqueue(cmd); }
        public bool HasCommand() => _pending.Count > 0;
        public ICommand Select() => _pending.Count > 0 ? _pending.Dequeue() : null;
    }

    public class ServerThreadTests : IDisposable
    {
        private readonly QueueScheduler _taskQueue;
        private readonly ServerThread _executor;

        public ServerThreadTests()
        {
            _taskQueue = new QueueScheduler();
            _executor = new ServerThread(_taskQueue);
        }

        public void Dispose()
        {
            if (_executor.Thread.IsAlive)
            {
                _executor.HardStop();
                _executor.Join();
            }
        }

        [Fact]
        public void HardStop_StopsProcessing()
        {
            var counter = 0;
            var cmd = new BasicCommand(() => counter++);

            _executor.Start();
            _executor.Add(cmd);
            Thread.Sleep(100);
            _executor.HardStop();
            _executor.Join();

            Assert.Equal(1, counter);
        }

        [Fact]
        public void SoftStop_ProcessesAllBeforeExit()
        {
            var counter = 0;
            var cmd1 = new BasicCommand(() => counter++);
            var cmd2 = new BasicCommand(() => counter++);
            var cmd3 = new BasicCommand(() => counter++);

            _executor.Start();
            _executor.Add(cmd1);
            _executor.Add(cmd2);
            _executor.Add(cmd3);
            _executor.SoftStop();
            _executor.Join();

            Assert.Equal(3, counter);
        }

        [Fact]
        public void ExceptionHandler_CalledOnError()
        {
            var cmd = new BasicCommand(() => throw new Exception("test"));

            _executor.Start();
            _executor.Add(cmd);
            Thread.Sleep(100);
            _executor.HardStop();
            _executor.Join();

            Assert.NotNull(ExceptionHandler.FailedCommand);
            Assert.NotNull(ExceptionHandler.CapturedError);
        }
    }
}
