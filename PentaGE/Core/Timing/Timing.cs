using GLFW;

namespace PentaGE.Core
{
    /// <summary>
    /// Provides timing and frame rate management for the game engine.
    /// </summary>
    public sealed class Timing
    {
        private readonly TimingManager _timingManager = new();
        private double _gameSpeedFactor = 1d;
        private double previousTime = 0d;
        private double lastFPSTime = 0d;
        private int fpsFrameCount = 0;

        /// <summary>
        /// Gets the current frame information such as the frame number and delta time.
        /// </summary>
        public Frame CurrentFrame { get; private set; } = new();

        /// <summary>
        /// Gets the current frames per second (FPS) value.
        /// </summary>
        public int CurrentFps { get; private set; } = 0;

        /// <summary>
        /// Gets or sets the target frames per second (FPS) for the game engine.
        /// </summary>
        public int TargetFps { get; set; } = 60;

        /// <summary>
        /// Gets the total elapsed time since the start of the game engine, measured in seconds.
        /// </summary>
        public double TotalElapsedTime { get; private set; } = 0d;

        public TimingManager CustomTimings => _timingManager;

        /// <summary>
        /// Gets or sets the factor to scale the game speed. Default is 1.0 (normal speed).
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the value is zero.</exception>
        public double GameSpeedFactor
        {
            get => _gameSpeedFactor;
            set
            {
                if (value == 0)
                    throw new ArgumentException("The value can not be zero.", nameof(value));
                _gameSpeedFactor = value;
            }
        }

        /// <summary>
        /// Advances to the next frame, updating the timing and FPS information.
        /// </summary>
        internal void NextFrame()
        {
            // Calculate the target frame time based on the TargetFps
            double targetFrameTime = 1.0 / TargetFps;

            // Get the current time using Glfw.Time
            double currentTime = Glfw.Time;
            double fpsDeltaTime = currentTime - lastFPSTime;
            fpsFrameCount++;

            if (fpsDeltaTime >= 1.0) // Calculate FPS every 1 second
            {
                CurrentFps = fpsFrameCount;
                fpsFrameCount = 0;
                lastFPSTime = currentTime;
            }

            // Calculate the delta time between the current and previous frame
            double deltaTime = currentTime - previousTime;

            // Apply game speed to deltaTime
            deltaTime *= GameSpeedFactor;

            // Limit the frame rate if necessary
            if (TargetFps > 0 && deltaTime < targetFrameTime)
            {
                double sleepTime = (targetFrameTime - deltaTime) * 1000d; // Convert to milliseconds
                if (sleepTime > 0)
                {
                    Thread.Sleep((int)sleepTime);
                }
            }

            _timingManager.UpdateTimings(Glfw.Time);

            CurrentFrame = new Frame(CurrentFrame + 1, deltaTime);
            TotalElapsedTime += deltaTime;
            previousTime = currentTime;
        }
    }
}