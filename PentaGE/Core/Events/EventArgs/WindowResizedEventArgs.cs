using System.Drawing;

namespace PentaGE.Core.Events
{
    /// <summary>
    /// Event arguments for a window resized event.
    /// </summary>
    public sealed class WindowResizedEventArgs : EngineEventArgs
    {
        /// <summary>
        /// Gets the event category.
        /// </summary>
        internal override EventCategory Category => 
            EventCategory.Window | EventCategory.Resize;

        /// <summary>
        /// Gets the event type.
        /// </summary>
        internal override EventType Type => 
            EventType.WindowResized;


        /// <summary>
        /// Gets the size of the window after resizing.
        /// </summary>
        public Size Size { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowResizedEventArgs"/> class with the associated window.
        /// </summary>
        /// <param name="onEvent">The event handler to be invoked when the event is raised.</param>
        /// <param name="window">The window associated with the event.</param>
        /// <param name="size">The new size of the window after resizing.</param>
        public WindowResizedEventArgs(Action<EngineEventArgs> onEvent, Window window, Size size) : base(onEvent, window)
        {
            Size = size;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="WindowResizedEventArgs"/> object, including the size of the window.
        /// </summary>
        /// <returns>A string representing the window resized event with its associated data.</returns>
        public override string ToString() =>
            $"{{Size={Size}}}";
    }
}