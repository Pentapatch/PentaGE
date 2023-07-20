namespace PentaGE.Core
{
    /// <summary>
    /// Represents a frame in the game engine with information about its number and delta time.
    /// </summary>
    public record Frame
    {
        /// <summary>
        /// Gets the number of the frame.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Gets the time duration between this frame and the previous one, in seconds.
        /// The delta time is scaled by the game speed factor.
        /// </summary>
        public double DeltaTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class with the specified frame number and delta time.
        /// </summary>
        /// <param name="number">The number of the frame.</param>
        /// <param name="deltaTime">The time duration between this frame and the previous one, in seconds.</param>
        public Frame(int number, double deltaTime)
        {
            Number = number;
            DeltaTime = deltaTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class with default values.
        /// </summary>
        public Frame() { }

        /// <summary>
        /// Implicitly converts a <see cref="Frame"/> to its frame number.
        /// </summary>
        /// <param name="frame">The <see cref="Frame"/> to convert.</param>
        public static implicit operator int(Frame frame) => frame.Number;
    }
}