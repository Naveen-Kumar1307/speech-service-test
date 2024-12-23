using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace GlobalEnglish.Utility.Diagnostics
{
    /// <summary>
    /// Spawns a background worker thread.
    /// </summary>
    public class WorkerThread
    {
        #region spawning threads
        /// <summary>
        /// Spawns a background worker thread to perform a deferred task.
        /// </summary>
        /// <typeparam name="StateType">a state type</typeparam>
        /// <param name="taskArgument">a task argument</param>
        /// <param name="DoTask">a deferred task</param>
        public static void SpawnBackground<StateType>(
            StateType taskArgument, Action<StateType> DoTask)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(delegate(object state)
                    { DoTask((StateType)state); }), taskArgument);
        }

        /// <summary>
        /// Spawns a background worker thread to perform a deferred task.
        /// </summary>
        /// <param name="DoTask">a deferred task</param>
        public static void SpawnBackground(Action DoTask)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(delegate(object state) { DoTask(); }));
        }

        /// <summary>
        /// Spawns a foreground worker thread to perform a deferred task.
        /// </summary>
        /// <param name="DoTask">a deferred task</param>
        public static void SpawnForeground(Action DoTask)
        {
            new Thread(new ThreadStart(DoTask)).Start();
        }
        #endregion

    } // WorkerThread
}
