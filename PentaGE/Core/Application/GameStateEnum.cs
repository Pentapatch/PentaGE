namespace PentaGE.Core
{
    /// <summary>
    /// Represents the different states of the game engine.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game engine is initializing.
        /// </summary>
        Initializing = 0,

        /// <summary>
        /// The game engine is running and actively updating and rendering the game.
        /// </summary>
        Running = 1,

        /// <summary>
        /// The game engine is paused, and no updates or rendering occur.
        /// </summary>
        Paused = 2,

        /// <summary>
        /// The game engine is terminating, shutting down, or unloading resources.
        /// </summary>
        Terminating = 3,

        /// <summary>
        /// The game engine has terminated.
        /// </summary>
        Terminated = 4
    }
}