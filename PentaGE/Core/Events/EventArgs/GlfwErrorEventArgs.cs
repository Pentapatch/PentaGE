namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a GLFW error event.
    /// </summary>
    public sealed class GlfwErrorEventArgs : EngineEventArgs
    {
        /// <summary>
        /// Gets the event category.
        /// </summary>
        internal override EventCategory Category =>
            EventCategory.Error;

        /// <summary>
        /// Gets the event type.
        /// </summary>
        internal override EventType Type =>
            EventType.Error;

        /// <summary>
        /// Gets the error message associated with the event.
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Gets the GLFW error code associated with the event.
        /// </summary>
        public GLFW.ErrorCode ErrorCode { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlfwErrorEventArgs"/> class with the associated error message and error code.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="message">The error message associated with the event.</param>
        /// <param name="errorCode">The GLFW error code associated with the event.</param>
        public GlfwErrorEventArgs(Action<EngineEventArgs> onEvent, string message, GLFW.ErrorCode errorCode) : base(onEvent, null!)
        {
            Message = message;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="GlfwErrorEventArgs"/> object, including the error message and error code.
        /// </summary>
        /// <returns>A string representing the error event with its associated data.</returns>
        public override string ToString() =>
            $"{{Message={Message}, ErrorCode={ErrorCode}}}";
    }
}