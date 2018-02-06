using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BneDev.TimesheetNinja.Common
{
    public static class AsyncHelper
    {
        /// <summary>
        /// Waits for the task to complete and returns the result.
        /// If an exception occurs within the task execution the underlying exception is extracted flattened
        /// from the <see cref="AggregateException"/> thrown by the Task factory. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public static T WaitAndUnwrapResult<T>(this Task<T> task)
        {
            try
            {
                task.Wait();
                return task.Result;
            }
            catch (AggregateException e)
            {
                ExceptionDispatchInfo.Capture(e.Flatten().InnerExceptions.First()).Throw();
            }
            return default(T);
        }

        /// <summary>
        /// Waits on the task.
        /// If an exception occurs within the task execution the underlying exception is extracted flattened
        /// from the <see cref="AggregateException"/> thrown by the Task factory. 
        /// </summary>
        /// <param name="task">The task.</param>
        public static void WaitAndThrowNestedException(this Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                ExceptionDispatchInfo.Capture(e.Flatten().InnerExceptions.First()).Throw();
            }
        }

        /// <summary>
        /// Runs a TPL Task fire-and-forget style, the right way - in the background, separate from the current thread, with no risk
        /// of it trying to rejoin the current thread.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <remarks>
        /// http://stackoverflow.com/questions/5613951/simplest-way-to-do-a-fire-and-forget-method-in-c-sharp-4-0
        /// </remarks>
        public static void RunInBackground(Func<Task> function)
        {
#pragma warning disable 4014
            Task.Run(function).ConfigureAwait(false);
        }

        public static void RunInBackground(Action action)
        {
#pragma warning disable 4014
            Task.Run(action).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs a TPL Task fire-and-forget style, the right way - in the background, separate from the current thread, with no risk
        /// of it trying to rejoin the current thread.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static void RunInBackground(Func<Task> function, CancellationToken cancellationToken)
        {
#pragma warning disable 4014
            Task.Run(function, cancellationToken).ConfigureAwait(false);
        }
    }
}
