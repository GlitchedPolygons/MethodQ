using System;

namespace MethodQ
{
    /// <summary>
    /// Service interface for scheduling method calls using a method queue.
    /// </summary>
    public interface IMethodQ
    {
        /// <summary>
        /// Schedules the specified action to be executed ONCE at the specified <paramref name="executionUtc"/> <see cref="DateTime"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to schedule.</param>
        /// <param name="executionUtc">The <see cref="DateTime"/> in UTC of when the <paramref name="action"/> should be invoked.</param>
        /// <returns>The scheduled method call's unique identifier (needed for cancellation).</returns>
        int Schedule(Action action, DateTime executionUtc);

        /// <summary>
        /// Schedules the specified <see cref="Action"/> to be executed every <paramref name="interval"/> until manual cancellation.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to schedule.</param>
        /// <param name="interval">Repetition interval <see cref="TimeSpan"/>.</param>
        /// <returns>The scheduled method call's unique identifier (needed for cancellation).</returns>
        int Schedule(Action action, TimeSpan interval);

        /// <summary>
        /// Cancels the specified method call (removes it from the list of scheduled calls). Irreversible!
        /// </summary>
        /// <param name="id">The scheduled action's identifier.</param>
        /// <returns>Whether the cancellation was successful or not (e.g. can't double-cancel or cancel an inexistent call).</returns>
        bool Cancel(int id);
    }
}
