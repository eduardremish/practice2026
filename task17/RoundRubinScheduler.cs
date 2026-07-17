using System;
using System.Collections.Generic;
using task17;

namespace task17
{
    public class RoundRobinScheduler : IScheduler
    {
        private readonly Queue<ICommand> _pending = new();
        private readonly object _syncRoot = new();

        public bool HasCommand()
        {
            lock (_syncRoot)
            {
                return _pending.Count > 0;
            }
        }

        public ICommand Select()
        {
            lock (_syncRoot)
            {
                return _pending.Count > 0 ? _pending.Dequeue() : null;
            }
        }

        public void Add(ICommand cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));

            lock (_syncRoot)
            {
                _pending.Enqueue(cmd);
            }
        }
    }
}
