namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents different display modes for entities.
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// The entity is displayed at all times (scene is stopped, playing, or paused).
        /// </summary>
        Always = 0,

        /// <summary>
        /// The entity is displayed only during play mode (scene is playing or paused).
        /// </summary>
        WhenPlaying = 1,

        /// <summary>
        /// The entity is displayed only during edit mode (scene is stopped).
        /// </summary>
        WhenEditing = 2,
    }
}