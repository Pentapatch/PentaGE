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
        /// Events related to keys.
        /// </summary>
        Key = 1 << 2,

        /// <summary>
        /// Events related to mouse.
        /// </summary>
        Mouse = 1 << 3,

        /// <summary>
        /// Events related to mouse buttons.
        /// </summary>
        MouseButton = 1 << 4,

        /// <summary>
        /// Events related to mouse position.
        /// </summary>
        MousePosition = 1 << 5,

        /// <summary>
        /// Events related to mouse hover (enter or leave).
        /// </summary>
        MouseHover = 1 << 6,
    }
}