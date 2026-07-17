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

      

        [Fact]
        public void UpdateBehavior_ShouldReplaceDefaultProcessing()
        {
            var server = new ServerThread();
            var executedByDefault = false;
            var executedByCustom = false;

            var cmd1 = new Command { OnExecute = () => executedByDefault = true };
            var cmd2 = new Command { OnExecute = () => executedByCustom = true };

            server.UpdateBehavior(() =>
            {
                executedByCustom = true;
                server.HardStop();
            });

            server.Add(cmd1);
            server.Add(cmd2);
            server.Start();
            server.Join();

            Assert.False(executedByDefault);
            Assert.True(executedByCustom);
        }

        [Fact]
        public void Add_AfterHardStop_ShouldNotExecuteCommands()
        {
            var server = new ServerThread();
            server.Start();
            server.HardStop();
            server.Join();

            var cmd = new Command();
            server.Add(cmd);

            Assert.False(cmd.Executed);
        }

       
    }
}