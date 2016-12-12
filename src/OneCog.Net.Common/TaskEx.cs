using System;
using System.Threading.Tasks;

namespace OneCog.Net
{
    public static class TaskEx
    {
        private static readonly Lazy<Task> _completedTask = new Lazy<Task>(ConstructCompletedTask);

        private static Task ConstructCompletedTask()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            tcs.SetResult(tcs);
            return tcs.Task;
        }

        public static Task Completed
        {
            get { return _completedTask.Value; }
        }
    }
}
