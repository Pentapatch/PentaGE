namespace PentaGE.Core
{
    /// <summary>
    /// Represents a custom timing mechanism that triggers an event or executes an action at a specified interval of time.
    /// </summary>
    public sealed class CustomTiming
    {
        private double totalElapsedSeconds = 0d;

        /// <summary>
        /// Occurs when the custom timing interval has elapsed.
        /// </summary>
        public event EventHandler<CustomTimingTickEventArgs>? Tick;

        /// <summary>
        /// Gets or sets the interval in seconds at which the custom timing occurs.
        /// </summary>
        public double Interval { get; set; }

        /// <summary>
        /// Gets or sets the action to be executed when the custom timing interval has elapsed.
        /// </summary>
        public Action<double>? Action { get; set; } = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTiming"/> class with the specified interval.
        /// </summary>
        /// <param name="intervalInSeconds">The interval in seconds at which the custom timing occurs.</param>
        public CustomTiming(double intervalInSeconds)
        {
            Interval = intervalInSeconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTiming"/> class with the specified interval and action to be executed.
        /// </summary>
        /// <param name="intervalInSeconds">The interval in seconds at which the custom timing occurs.</param>
        /// <param name="action">The action to be executed when the custom timing interval has elapsed.</param>
        public CustomTiming(double intervalInSeconds, Action<double> action)
        {
            Interval = intervalInSeconds;
            Action = action;
        }

        /// <summary>
        /// Updates the custom timing by a specified elapsed time and triggers the Tick event or executes the associated action if the interval has elapsed.
        /// </summary>
        /// <param name="elapsedTime">The time elapsed since the last update in seconds.</param>
        public void Update(double elapsedTime)
        {
            totalElapsedSeconds += elapsedTime;

            if (totalElapsedSeconds >= Interval)
            {
                OnTick();
                totalElapsedSeconds = 0d;
            }
        }

        /// <summary>
        /// Triggers the Tick event or executes the associated action when the custom timing interval has elapsed.
        /// </summary>
        private void OnTick()
        {
            if (Action is not null) Action(totalElapsedSeconds);
            var eventArgs = new CustomTimingTickEventArgs(Interval, totalElapsedSeconds);
            Tick?.Invoke(this, eventArgs);
        }
    }
}