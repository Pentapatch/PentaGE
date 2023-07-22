using Serilog;

namespace PentaGE.Core.Logging
{
    /// <summary>
    /// Provides a performance logging block to measure the execution time of a specific operation.
    /// </summary>
    internal class PerformanceLogger : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _message;
        private readonly DateTimeOffset _startTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceLogger"/> class, starting the performance logging block.
        /// </summary>
        /// <param name="logger">The logger instance used for logging.</param>
        /// <param name="message">The message or description of the operation being logged.</param>
        public PerformanceLogger(ILogger logger, string message)
        {
            _logger = logger;
            _message = message;
            _startTime = DateTimeOffset.UtcNow;

            _logger.Information($"{_message}..");
        }

        /// <summary>
        /// Stops the performance logging block and logs the elapsed time.
        /// </summary>
        public void Dispose()
        {
            var elapsedMilliseconds = (DateTimeOffset.UtcNow - _startTime).TotalMilliseconds;
            _logger.Information($"{_message} done [{elapsedMilliseconds}ms].");
        }
    }
}