using System;
using System.Timers;
using System.Collections.Generic;

namespace GlitchedPolygons.Services.MethodQ
{
    /// <summary>
    /// Schedule your method calls easily and add the fourth dimension to your projects. Time is the most precious resource in life.
    /// </summary>
    public class MethodQ : IMethodQ
    {
        private ulong nextId = 0;
        private readonly Dictionary<ulong, Timer> timers = new Dictionary<ulong, Timer>(16);

        /// <summary>
        /// Schedules the specified action to be executed ONCE at the specified <paramref name="executionUtc"/> <see cref="DateTime"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to schedule.</param>
        /// <param name="executionUtc">The <see cref="DateTime"/> in UTC of when the <paramref name="action"/> should be invoked.</param>
        /// <returns>The scheduled method call's unique identifier (needed for cancellation).</returns>
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

        /// <summary>
        /// Schedules the specified <see cref="Action"/> to be executed every <paramref name="interval"/> until manual cancellation.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to schedule.</param>
        /// <param name="interval">Repetition interval <see cref="TimeSpan"/>.</param>
        /// <returns>The scheduled method call's unique identifier (needed for cancellation).</returns>
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

        /// <summary>
        /// Cancels the specified method call (removes it from the list of scheduled calls). Irreversible!
        /// </summary>
        /// <param name="id">The scheduled action's identifier.</param>
        /// <returns>Whether the cancellation was successful or not (e.g. can't double-cancel or cancel an inexistent call).</returns>
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
