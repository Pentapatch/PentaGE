namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents different update modes for entities.
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// The entity receives update events both when the scene is playing and stopped.
        /// </summary>
        Always = 0,

        /// <summary>
        /// The entity receives update events only when the scene is playing.
        /// </summary>
        WhenPlaying = 1,

        /// <summary>
        /// The entity receives update events only when the scene is stopped.
        /// </summary>
        WhenEditing = 2,
    }
}