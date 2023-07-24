namespace PentaGE.Common
{
    /// <summary>
    /// Specifies the modifier keys used in combination with other input events.
    /// </summary>
    [Flags]
    public enum ModifierKey
    {
        /// <summary>
        /// Undefined or no modifier key.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Shift key.
        /// </summary>
        Shift = 1 << 0,

        /// <summary>
        /// The Control key.
        /// </summary>
        Control = 1 << 1,

        /// <summary>
        /// The Alt key.
        /// </summary>
        Alt = 1 << 2,

        /// <summary>
        /// The Super key (Windows key on Windows).
        /// </summary>
        Super = 1 << 3,

        /// <summary>
        /// The CapsLock key.
        /// </summary>
        CapsLock = 1 << 4,

        /// <summary>
        /// The NumLock key.
        /// </summary>
        NumLock = 1 << 5
    }
}