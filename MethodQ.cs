using System;
using System.Timers;
using System.Collections.Generic;

namespace GlitchedPolygons.Services.MethodQ
{
    public class MethodQ : IMethodQ
    {
        private ulong nextId = 0;
        private readonly Dictionary<ulong, Timer> timers = new Dictionary<ulong, Timer>(16);

        public ulong Schedule(Action action, DateTime executionUtc)
        {
            if (action is null)
                throw new ArgumentNullException($"{nameof(MethodQ)}::{nameof(Schedule)}: The provided {nameof(action)} argument is null!");

            var utcNow = DateTime.UtcNow;

            if (executionUtc < utcNow)
                throw new ArgumentException($"{nameof(MethodQ)}::{nameof(Schedule)}: The provided {nameof(executionUtc)} {nameof(DateTime)} lies in the past! The {nameof(action)} wouldn't be executed at all!");

            var deltaTime = executionUtc - utcNow;

            if (deltaTime.TotalMilliseconds >= Int32.MaxValue)
                throw new ArgumentException($"{nameof(MethodQ)}::{nameof(Schedule)}: The provided {nameof(executionUtc)} {nameof(DateTime)} lies more than {{Int32.MaxValue}} milliseconds in the future, which is not allowed and not a good idea anyway (use a scheduled task for anything >3 days in the future).");

            ulong id = nextId++;
            var timer = new Timer(deltaTime.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) =>
            {
                action.Invoke();
                timer.Dispose();
                timers.Remove(id);
            };
            timer.Start();
            timers.Add(id, timer);
            return id;
        }

        public ulong Schedule(Action action, TimeSpan interval)
        {
            if (action is null)
                throw new ArgumentNullException($"{nameof(MethodQ)}::{nameof(Schedule)}: The provided {nameof(action)} argument is null!");

            if (interval.TotalMilliseconds >= Int32.MaxValue)
                throw new ArgumentException($"{nameof(MethodQ)}::{nameof(Schedule)}: The provided {nameof(interval)} {nameof(TimeSpan)} is longer than {{Int32.MaxValue}} milliseconds, which is not allowed and not a good idea anyway (use a scheduled task for anything >3 days in the future).");

            ulong id = nextId++;
            var timer = new Timer(interval.TotalMilliseconds) { AutoReset = true };
            timer.Elapsed += (_, __) => action.Invoke();
            timer.Start();
            timers.Add(id, timer);
            return id;
        }

        public bool Cancel(ulong id)
        {
            if (!timers.TryGetValue(id, out var timer))
                return false;

            if (timer is null || !timer.Enabled)
                return false;

            timer.Stop();
            timer.Dispose();
            return timers.Remove(id);
        }
    }
}
