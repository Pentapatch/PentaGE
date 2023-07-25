namespace PentaGE.Core
{
    /// <summary>
    /// A manager class for handling custom timings in the game engine.
    /// </summary>
    public sealed class CustomTimingsManager
    {
        private readonly List<CustomTiming> _customTimings = new();
        private double lastElapsed = 0d;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTimingsManager"/> class.
        /// </summary>
        internal CustomTimingsManager() { }

        /// <summary>
        /// Gets the custom timing instance associated with the specified interval in seconds.
        /// If the timing doesn't exist, a new one is created.
        /// </summary>
        /// <param name="interval">The interval at which the custom timing should occur in seconds.</param>
        public CustomTiming this[double interval] =>
            CreateOrReturnEntry(interval);

        /// <summary>
        /// Adds a new custom timing with the specified interval and an action to perform on each tick.
        /// </summary>
        /// <param name="intervalInSeconds">The interval at which the custom timing should occur in seconds.</param>
        /// <param name="action">The action to be executed on each tick of the custom timing.</param>
        /// <returns>The newly added <see cref="CustomTiming"/> instance.</returns>
        public CustomTiming Add(double intervalInSeconds, Action<double> action)
        {
            var customTiming = new CustomTiming(intervalInSeconds, action);
            _customTimings.Add(customTiming);
            return customTiming;
        }

        /// <summary>
        /// Adds a new custom timing with the specified interval.
        /// </summary>
        /// <param name="intervalInSeconds">The interval at which the custom timing should occur in seconds.</param>
        /// <returns>The newly added <see cref="CustomTiming"/> instance.</returns>
        public CustomTiming Add(double intervalInSeconds)
        {
            var customTiming = new CustomTiming(intervalInSeconds);
            _customTimings.Add(customTiming);
            return customTiming;
        }

        /// <summary>
        /// Removes the specified custom timing.
        /// </summary>
        /// <param name="customTiming">The custom timing instance to remove.</param>
        /// <returns><c>true</c> if the custom timing is successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(CustomTiming customTiming) =>
            _customTimings.Remove(customTiming);

        /// <summary>
        /// Removes all custom timings from the manager.
        /// </summary>
        public void Clear() => _customTimings.Clear();

        /// <summary>
        /// Updates the custom timings based on the elapsed time since the last update.
        /// </summary>
        /// <param name="currentTime">The current time in seconds.</param>
        internal void UpdateTimings(double currentTime)
        {
            double elapsedTime = currentTime - lastElapsed;
            lastElapsed = currentTime;

            foreach (var customTiming in _customTimings)
            {
                customTiming.Update(elapsedTime);
            }
        }

        /// <summary>
        /// Creates or returns an existing custom timing instance associated with the specified interval in seconds.
        /// If a custom timing with the specified interval already exists, it is returned; otherwise, a new custom timing is created and added.
        /// </summary>
        /// <param name="intervalInSeconds">The interval at which the custom timing should occur in seconds.</param>
        /// <returns>The existing or newly created <see cref="CustomTiming"/> instance.</returns>
        private CustomTiming CreateOrReturnEntry(double intervalInSeconds)
        {
            CustomTiming? customTiming = _customTimings.FirstOrDefault(x => x.Interval == intervalInSeconds);
            if (customTiming is not null) return customTiming;
            return Add(intervalInSeconds);
        }
    }
}