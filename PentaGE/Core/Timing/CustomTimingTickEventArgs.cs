namespace PentaGE.Core
{
    /// <summary>
    /// Custom event arguments class for the "Tick" event in the <see cref="CustomTiming"/> class.
    /// </summary>
    public class CustomTimingTickEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the elapsed time since the last tick event in seconds.
        /// </summary>
        public double ElapsedTime { get; }

        /// <summary>
        /// Gets the interval at which the "Tick" event occurs in seconds.
        /// </summary>
        public double Interval { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTimingTickEventArgs"/> class
        /// with the specified interval and elapsed time.
        /// </summary>
        /// <param name="interval">The interval at which the "Tick" event occurs in seconds.</param>
        /// <param name="elapsedTime">The elapsed time since the last tick event in seconds.</param>
        public CustomTimingTickEventArgs(double interval, double elapsedTime)
        {
            Interval = interval;
            ElapsedTime = elapsedTime;
        }
    }
}