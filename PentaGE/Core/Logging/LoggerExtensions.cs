using Serilog;

namespace PentaGE.Core.Logging
{
    /// <summary>
    /// Provides extension methods for logging with various logger implementations.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Begins a performance logging block for measuring the execution time of a specific operation.
        /// </summary>
        /// <param name="logger">The logger instance used for logging.</param>
        /// <param name="message">The message or description of the operation being logged.</param>
        /// <returns>A disposable object representing the performance logging block. When disposed, the block will log the elapsed time.</returns>
        public static IDisposable BeginPerfLogger(this ILogger logger, string message) =>
            new PerformanceLogger(logger, message);
    }
}