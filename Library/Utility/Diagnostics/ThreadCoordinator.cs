using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Common.Logging;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Diagnostics
{
    /// <summary>
    /// Coordinates several threads, typically for stress tests.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows the performer for each thread</item>
    /// <item>knows the repetitions and wait limit for each thread</item>
    /// </list>
    /// </remarks>
    public class ThreadCoordinator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ThreadCoordinator));
        private static readonly int ShortDelay = 500;

        /// <summary>
        /// An upper bound on the (random) thread wait interval.
        /// </summary>
        public int ThreadWaitLimit { get; private set; }

        /// <summary>
        /// Indicates how many times to perform each action.
        /// </summary>
        public int ActionRepetitions { get; private set; }

        /// <summary>
        /// The actions on which the spawned threads perform.
        /// </summary>
        public IPerformance[] Performers { get; set; }

        private DateTime SpawnTime { get; set; }
        private DateTime FinalTime { get; set; }

        private double MaximumSpan { get; set; }
        private double TotalSpan { get; set; }

        private int TotalCount { get; set; }
        private int TotalWait { get; set; }

        private int ThreadCount { get; set; }
        private Thread[] Threads { get; set; }
        private Random RandomTime = new Random();
        private AutoResetEvent Coordinator { get; set; }

        /// <summary>
        /// Signals that all threads have been started.
        /// </summary>
        public event EventHandler AllThreadsStarted;

        /// <summary>
        /// Signals that all threads have completed.
        /// </summary>
        public event EventHandler AllThreadsCompleted;

        #region creating instances
        /// <summary>
        /// Returns a new ThreadCoordinator.
        /// </summary>
        /// <param name="performer"></param>
        /// <returns></returns>
        public static ThreadCoordinator With(IPerformance performer)
        {
            Argument.Check("performer", performer);
            IPerformance[] performers = { performer };
            return With(performers);
        }

        /// <summary>
        /// Returns a new ThreadCoordinator.
        /// </summary>
        /// <param name="performers"></param>
        /// <returns></returns>
        public static ThreadCoordinator With(ICollection<IPerformance> performers)
        {
            Argument.CheckAny("performers", performers);
            ThreadCoordinator result = new ThreadCoordinator();
            result.Performers = performers.ToArray();
            return result;
        }

        /// <summary>
        /// Constructs a new ThreadCoordinator.
        /// </summary>
        private ThreadCoordinator()
        {
            ActionRepetitions = 1;
            ThreadWaitLimit = 0;
        }

        /// <summary>
        /// Establishes the thread repetition and wait limits.
        /// </summary>
        /// <param name="repetitions">the number of times to perform each action</param>
        /// <param name="waitLimit">the limit from which to choose a random wait interval</param>
        /// <returns>this ThreadCoordinator</returns>
        public ThreadCoordinator With(int repetitions, int waitLimit)
        {
            ActionRepetitions = repetitions;
            ThreadWaitLimit = waitLimit;
            return this;
        }
        #endregion

        #region accessing values
        /// <summary>
        /// The average time threads spent waiting.
        /// </summary>
        public double AverageWaitTime
        {
            get { return TotalCount > 0 ? TotalWait / TotalCount : 0; }
        }

        /// <summary>
        /// The average time threads spent performing actions.
        /// </summary>
        public double AveragePerformanceTime
        {
            get
            {
                if (TotalCount < 1) return 0;
                if (TotalCount > 5)
                {
                    return (TotalSpan - MaximumSpan) / (TotalCount - 1);
                }
                else
                {
                    return TotalSpan / TotalCount;
                }
            }
        }

        /// <summary>
        /// The maximum time a thread spent performing an action.
        /// </summary>
        public double MaximumPerformanceTime
        {
            get { return MaximumSpan; }
        }

        /// <summary>
        /// The average action performance rate.
        /// </summary>
        public double AveragePerformanceRate
        {
            get { return TotalCount > 0 ? TotalCount / TotalTestTime : 0; }
        }

        /// <summary>
        /// The action performance count.
        /// </summary>
        public int TotalPerformanceCount
        {
            get { return TotalCount; }
        }

        /// <summary>
        /// The total time of the test.
        /// </summary>
        private double TotalTestTime
        {
            get { return (FinalTime - SpawnTime).TotalSeconds; }
        }
        #endregion

        #region coordinating threads
        /// <summary>
        /// Indicates whether this coordinator has active threads.
        /// </summary>
        public bool HasActiveThreads
        {
            get { return ThreadCount > 0; }
        }

        /// <summary>
        /// Spawns the foreground thread(s) and waits for them to finish.
        /// </summary>
        /// <returns></returns>
        public ThreadCoordinator SpawnThreadsWaitForCompletion()
        {
            return SpawnForegroundThreads().WaitForCompletion();
        }

        /// <summary>
        /// Spawns the foreground thread(s) that repeatedly perform the action(s).
        /// </summary>
        public ThreadCoordinator SpawnForegroundThreads()
        {
            if (!HasActiveThreads) SpawnNewThreads();

            return this;
        }

        /// <summary>
        /// Waits for the completion of the spawned threads.
        /// </summary>
        public ThreadCoordinator WaitForCompletion()
        {
            if (HasActiveThreads)
            {
                Coordinator = new AutoResetEvent(false);
                Coordinator.WaitOne();
                FinalTime = DateTime.Now;
            }

            return this;
        }

        /// <summary>
        /// Waits for the completion of any background threads.
        /// </summary>
        public ThreadCoordinator WaitForBackgroundCompletion()
        {
            if (ThreadWaits)
            {
                WaitForBackgroundCompletion(ThreadWaitLimit);
            }

            return this;
        }

        /// <summary>
        /// Stops all threads spawned by this coordinator (if any).
        /// </summary>
        public ThreadCoordinator StopAllThreads()
        {
            if (HasActiveThreads)
            {
                ActionRepetitions = 0;

                foreach (Thread thread in Threads)
                {
                    thread.Abort();
                }

                SignalCompletion();
            }

            return this;
        }

        /// <summary>
        /// Spawns threads on the configured action(s).
        /// </summary>
        private void SpawnNewThreads()
        {
            MaximumSpan = 0;
            TotalSpan = 0;
            TotalWait = 0;
            TotalCount = 0;

            ThreadCount = Performers.Length;
            Threads = new Thread[ThreadCount];

            for (int index = 0; index < ThreadCount; index++)
            {
                Threads[index] = CreatePerformer(index).CreateThread();
            }

            SpawnTime = DateTime.Now;
            for (int index = 0; index < ThreadCount; index++)
            {
                Threads[index].Start();
            }

            SignalStarted();
        }

        /// <summary>
        /// Creates a performer for a given index.
        /// </summary>
        private IPerformance CreatePerformer(int index)
        {
            return Performance<int>.With(PerformAction, index);
        }

        /// <summary>
        /// Performs an indicated action repeatedly until complete or interrupted.
        /// </summary>
        private void PerformAction(int index)
        {
            int repetitions = ActionRepetitions;

            // repeat until either done or interrupted
            while (ActionRepetitions > 0 && repetitions > 0)
            {
                try
                {
                    WaitUntilStartTime();

                    // perform action unless interrupted
                    if (ActionRepetitions > 0)
                    {
                        TimeSpan span = DateTime.Now.TimeToRun(delegate()
                        {
                            Performers[index].Perform();
                        });

                        TotalSpan += span.TotalMilliseconds;
                        if (span.TotalMilliseconds > MaximumSpan)
                        {
                            MaximumSpan = span.TotalMilliseconds;
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    // ignore this
                }
                catch (ThreadInterruptedException)
                {
                    // ignore this
                }
                catch (Exception ex)
                {
                    Logger.Error(ThreadName(index) + " failed", ex);
                }
                finally
                {
                    if (ActionRepetitions > 0)
                    {
                        TotalCount++;

                        // count down cycles unless interrupted
                        repetitions--;
                        if (repetitions < 1)
                        {
                            ThreadCount--;
                            if (ThreadCount < 1)
                            {
                                SignalCompletion();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Waits until the configured start time.
        /// </summary>
        private void WaitUntilStartTime()
        {
            if (ThreadWaits)
            {
                int waitTime = RandomTime.Next(ThreadWaitLimit);
                TotalWait += waitTime;
                Thread.Sleep(waitTime);
            }
        }

        /// <summary>
        /// Waits for any background threads to complete.
        /// </summary>
        /// <param name="waitLimit">a wait limit (in msecs)</param>
        public static void WaitForBackgroundCompletion(int waitLimit)
        {
            while (HasPendingWorkerThreads()) Thread.Sleep(waitLimit);
        }

        /// <summary>
        /// Indicates whether any pending threads are still working.
        /// </summary>
        public static bool HasPendingWorkerThreads()
        {
            int workerThreads = 0;
            int portThreads = 0;
            ThreadPool.GetAvailableThreads(out workerThreads, out portThreads);

            int maxWorkerThreads = 0;
            int maxPortThreads = 0;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxPortThreads);

            return (workerThreads < maxWorkerThreads);
        }

        /// <summary>
        /// Indicates whether each thread waits at the beginning of each cycle.
        /// </summary>
        private bool ThreadWaits
        {
            get { return ThreadWaitLimit >= 5; }
        }

        /// <summary>
        /// Signals that all threads have been spawned.
        /// </summary>
        private void SignalStarted()
        {
            try
            {
                if (AllThreadsStarted != null)
                    AllThreadsStarted(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Logger.Warn(SignalFailure + "AllThreadsStarted", ex);
            }
        }

        /// <summary>
        /// Signals that all threads have completed.
        /// </summary>
        private void SignalCompletion()
        {
            try
            {
                Threads = null;
                ThreadCount = 0;

                if (AllThreadsCompleted != null)
                    AllThreadsCompleted(this, new EventArgs());

                if (Coordinator != null)
                {
                    Coordinator.Set();
                    Thread.Sleep(ShortDelay);

                    Coordinator.Close();
                    Coordinator = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(SignalFailure + "AllThreadsCompleted", ex);
            }
        }

        /// <summary>
        /// A thread name.
        /// </summary>
        private string ThreadName(int index)
        {
            return "ThreadCoordinator.Threads[" + index + "]";
        }

        private static readonly string SignalFailure =
                                "Detected failure during processing of signal ThreadCoordinator.";
        #endregion

    } // ThreadCoordinator


    /// <summary>
    /// A thread source (factory).
    /// </summary>
    public interface IThreadSource
    {
        Thread CreateThread();
    }

    /// <summary>
    /// Performs an action.
    /// </summary>
    public interface IPerformance : IThreadSource
    {
        void Perform();
    }

    /// <summary>
    /// Performs an action (without any argument).
    /// </summary>
    public class Performance : IPerformance
    {
        /// <summary>
        /// An action to perform.
        /// </summary>
        public Action PerformedAction { get; set; }

        /// <summary>
        /// Returns a new performance.
        /// </summary>
        /// <param name="action">an action</param>
        /// <param name="boundValue">a bound value</param>
        /// <returns>a new performance</returns>
        public static IPerformance With<T>(Action<T> action, T boundValue)
        {
            return Performance<T>.With(action, boundValue);
        }

        /// <summary>
        /// Returns a new performance.
        /// </summary>
        /// <param name="action">an action</param>
        /// <returns>a new performance</returns>
        public static IPerformance With(Action action)
        {
            Argument.Check("action", action);
            Performance result = new Performance();
            result.PerformedAction = action;
            return result;
        }

        /// <summary>
        /// Creates a new thread that performs an action.
        /// </summary>
        /// <returns>a new Thread</returns>
        public Thread CreateThread()
        {
            return new Thread(new ThreadStart(PerformedAction));
        }

        /// <summary>
        /// Performs an action.
        /// </summary>
        public void Perform()
        {
            PerformedAction();
        }
    }

    /// <summary>
    /// Performs an action with a bound value.
    /// </summary>
    /// <typeparam name="T">a bound value type</typeparam>
    public class Performance<T> : IPerformance
    {
        /// <summary>
        /// An action to perform.
        /// </summary>
        public Action<T> PerformedAction { get; set; }

        /// <summary>
        /// A bound value with which to perform the action.
        /// </summary>
        public T BoundValue { get; set; }

        /// <summary>
        /// Returns a new performance.
        /// </summary>
        /// <param name="action">an action</param>
        /// <param name="boundValue">a bound value</param>
        /// <returns>a new performance</returns>
        public static IPerformance With(Action<T> action, T boundValue)
        {
            Argument.Check("action", action);
            Performance<T> result = new Performance<T>();
            result.BoundValue = boundValue;
            result.PerformedAction = action;
            return result;
        }

        /// <summary>
        /// Creates a new thread that performs an action with a bound value.
        /// </summary>
        /// <returns>a new Thread</returns>
        public Thread CreateThread()
        {
            return new Thread(new ThreadStart(Perform));
        }

        /// <summary>
        /// Performs an action with a bound value.
        /// </summary>
        public void Perform()
        {
            PerformedAction(BoundValue);
        }

    } // ActionPerformance<T>
}
