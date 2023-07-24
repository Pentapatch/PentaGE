namespace PentaGE.Core.Events
{
    /// <summary>
    /// Flags to categorize the event.
    /// </summary>
    [Flags]
    internal enum EventCategory
    {
        /// <summary>
        /// Default or undefined category.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Events related to windows.
        /// </summary>
        Window = 1 << 0,

        /// <summary>
        /// General input events.
        /// </summary>
        Input = 1 << 1,

        /// <summary>
        /// Events related to the keyboard.
        /// </summary>
        Keyboard = 1 << 2,

        /// <summary>
        /// Events related to mouse.
        /// </summary>
        Mouse = 1 << 3,

        /// <summary>
        /// Events related to mouse buttons or keyboard keys.
        /// </summary>
        Button = 1 << 4,

        /// <summary>
        /// Events related to positions.
        /// </summary>
        Position = 1 << 5,

        /// <summary>
        /// Events related to hovering (enter or leave).
        /// </summary>
        Hover = 1 << 6,
    }
}