using System;
using Xunit;
using task17;
using System.Threading;
using System.Collections.Generic;

namespace ServerThreadTests
{
    public class ServerThreadTests : IDisposable
    {
        private class Command : ICommand
        {
            public bool Executed { get; private set; }
            public Action OnExecute { get; set; }

            public void Execute()
            {
                Executed = true;
                OnExecute?.Invoke();
            }
        }

        public ServerThreadTests()
        {
            ExceptionHandler.Reset();
        }

        public void Dispose()
        {
            ExceptionHandler.Reset();
        }
        [Fact]
        public void SoftStop_ShouldProcessAllCommands()
        {
            var server = new ServerThread();
            var counter = 0;

            server.Add(new Command { OnExecute = () => counter++ });
            server.Add(new Command { OnExecute = () => counter++ });
            server.Add(new Command { OnExecute = () => counter++ });

            server.Start();
            server.SoftStop();
            server.Join();

            Assert.Equal(3, counter);
        }

        [Fact]
        public void HardStop_ShouldProcessAllCommands()
        {
            var server = new ServerThread();
            server.Start();
            server.HardStop();
            server.Join();

            var cmd = new Command();
            server.Add(cmd);

            Assert.False(cmd.Executed);
        }


        [Fact]
        public void Start_MultipleThreads_ShouldExecuteInParallel()
        {
            var server1 = new ServerThread();
            var server2 = new ServerThread();
            var counter = 0;
            var cmd1 = new Command { OnExecute = () => Interlocked.Increment(ref counter) };
            var cmd2 = new Command { OnExecute = () => Interlocked.Increment(ref counter) };

            server1.Add(cmd1);
            server2.Add(cmd2);
            server1.Start();
            server2.Start();
            server1.HardStop();
            server2.HardStop();
            server1.Join();
            server2.Join();

            Assert.Equal(2, counter);
        }
    
    }
}
