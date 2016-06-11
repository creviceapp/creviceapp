using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace CreviceApp.Threading
{
    // http://www.codeguru.com/csharp/article.php/c18931/Understanding-the-NET-Task-Parallel-Library-TaskScheduler.htm
    public class SingleThreadScheduler : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Task> tasks = new BlockingCollection<Task>();
        private readonly Thread thread;

        public SingleThreadScheduler() : this(ThreadPriority.Normal) { }
            
        public SingleThreadScheduler(ThreadPriority priority)
        {
            this.thread = new Thread(new ThreadStart(Main));
            this.thread.Priority = priority;
            this.thread.Start();
        }

        private void Main()
        {
            Debug.Print("SingleThreadScheduler was started; Thread ID: 0x{0:X}", Thread.CurrentThread.ManagedThreadId);
            foreach (var t in tasks.GetConsumingEnumerable())
            {
                TryExecuteTask(t);
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return tasks.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            tasks.CompleteAdding();
        }

        ~SingleThreadScheduler()
        {
            Dispose();
        }
    }
}
