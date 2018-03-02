using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Threading
{
    using System.Threading;
    using System.Collections.Concurrent;
    using Crevice.Logging;
    
    public class LowLatencyScheduler
        : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Task> _tasks = new BlockingCollection<Task>();
        private readonly Thread[] _threads;

        public string Name { get; }
        public ThreadPriority Priority { get; }
        public int PoolSize { get; }

        public LowLatencyScheduler(string name, ThreadPriority priority, int poolSize)
        {
            Name = name;
            Priority = priority;
            PoolSize = poolSize;
            _threads = InitializeThreadPool();
        }

        private Thread[] InitializeThreadPool()
        {
            var threads = new Thread[PoolSize];
            for (int i = 0; i < PoolSize; i++)
            {
                var name = $"{Name}(Priority={Priority}, PoolSize={PoolSize}): [{i}/{PoolSize - 1}]";
                var thread = new Thread(() =>
                {
                    Verbose.Print($"Start {name}");
                    foreach (Task task in _tasks.GetConsumingEnumerable())
                    {
                        TryExecuteTask(task);
                    }
                    Verbose.Print($"End {name}");
                });
                thread.Name = name;
                thread.Priority = Priority;
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                threads[i] = thread;
            }
            return threads;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
            => _tasks;

        protected override void QueueTask(Task task)
            => _tasks.Add(task);

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            => false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tasks.CompleteAdding();
            }
        }

        ~LowLatencyScheduler() => Dispose(false);
    }
}
