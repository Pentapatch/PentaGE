namespace PentaGE.Core.Events
{
    /// <summary>
    /// Flags to categorize the event.
    /// </summary>
    [Flags]
    internal enum EventCategory
    {
        /// <summary>
        /// No category.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents all categories of events.
        /// </summary>
        All = ~0,

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

        /// <summary>
        /// Events related to scrolling.
        /// </summary>
        Scroll = 1 << 7,

        /// <summary>
        /// Events related to focus.
        /// </summary>
        Focus = 1 << 8,

        /// <summary>
        /// Events related to closing.
        /// </summary>
        Closing = 1 << 9,

        /// <summary>
        /// Events related to iconifying (minimizing or restoring).
        /// </summary>
        Iconify = 1 << 10,

        /// <summary>
        /// Events related to maximizing or restoring from maximized.
        /// </summary>
        Maximize = 1 << 11,

        /// <summary>
        /// Events related to resizing.
        /// </summary>
        Resize = 1 << 12,

        /// <summary>
        /// Events related to errors.
        /// </summary>
        Error = 1 << 13,

        /// <summary>
        /// Events related to hotkeys.
        /// </summary>
        HotKey = 1 << 14,
    }
}