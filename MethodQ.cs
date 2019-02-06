using System;
using System.Timers;
using System.Collections.Generic;

namespace MethodQ
{
    public class MethodQ : IMethodQ
    {
        private readonly List<Timer> timers = new List<Timer>(16);

        public int Schedule(Action action, DateTime executionUtc)
        {
            if (action is null)
            {
                throw new ArgumentNullException($"{nameof(MethodQ)}::{nameof(Schedule)}: The provided {nameof(action)} argument is null!");
            }

            var utcNow = DateTime.UtcNow;
            var deltaTime = executionUtc - utcNow;

            if (executionUtc < utcNow)
            {
                throw new ArgumentException($"{nameof(MethodQ)}::{nameof(Schedule)}: The provided {nameof(executionUtc)} {nameof(DateTime)} lies in the past! The {nameof(action)} wouldn't be executed at all!");
            }

            var timer = new Timer(deltaTime.TotalMilliseconds);
            timer.Elapsed += (_, __) => action.Invoke();
            timer.AutoReset = false;
            timer.Start();

            timers.Add(timer);
            return timers.Count - 1;
        }

        public int Schedule(Action action, TimeSpan interval)
        {
            if (action is null)
            {
                throw new ArgumentNullException($"{nameof(MethodQ)}::{nameof(Schedule)}: The provided {nameof(action)} argument is null!");
            }

            var timer = new Timer(interval.TotalMilliseconds);
            timer.Elapsed += (_, __) => action.Invoke();
            timer.AutoReset = true;
            timer.Start();

            timers.Add(timer);
            return timers.Count - 1;
        }

        public bool Cancel(int id)
        {
            if (id < 0 || id >= timers.Count)
            {
                return false;
            }

            var timer = timers[id];
            if (timer is null || !timer.Enabled)
            {
                return false;
            }

            timer.Stop();
            return true;
        }
    }
}
