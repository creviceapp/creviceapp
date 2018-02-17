using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Threading
{
    using System.Threading;
    using System.Collections.Concurrent;
    
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
                var thread = new Thread(() =>
                {
                    foreach (Task task in _tasks.GetConsumingEnumerable())
                    {
                        TryExecuteTask(task);
                    }

                });
                thread.Name = $"{Name}(Priority={Priority}, PoolSize={PoolSize}): {i}";
                thread.Priority = Priority;
                thread.IsBackground = true;
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
            GC.SuppressFinalize(this);
            _tasks.CompleteAdding();
        }

        ~LowLatencyScheduler()
            => Dispose();

        public static TaskFactory CreateTaskFactory(string name, ThreadPriority priority, int poolSize)
            => new TaskFactory(new LowLatencyScheduler(name, priority, poolSize));
    }
}
