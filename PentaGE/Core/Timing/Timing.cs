using GLFW;

namespace PentaGE.Core
{
    /// <summary>
    /// Provides timing and frame rate management for the game engine.
    /// </summary>
    public sealed class Timing
    {
        private readonly CustomTimingsManager _timingManager;
        private double _gameSpeedFactor;
        private double previousTime = 0d;
        private double lastFPSTime = 0d;
        private int fpsFrameCount = 0;

        public const double NORMAL_GAME_SPEED = 1d;
        public const double DOUBLE_GAME_SPEED = 2d;
        public const double HALF_GAME_SPEED = 0.5d;

        /// <summary>
        /// Gets the total elapsed run time as a TimeSpan.
        /// </summary>
        public TimeSpan RunTime => TimeSpan.FromSeconds(TotalElapsedTime);

        /// <summary>
        /// Gets the current frame information such as the frame number and delta time.
        /// </summary>
        public Frame CurrentFrame { get; private set; } = new();

        /// <summary>
        /// Gets the current frames per second (FPS) value.
        /// </summary>
        public int CurrentFps { get; private set; }

        /// <summary>
        /// Gets or sets the target frames per second (FPS) for the game engine.
        /// </summary>
        public int TargetFps { get; set; }

        /// <summary>
        /// Gets the total elapsed time since the start of the game engine, measured in seconds.
        /// </summary>
        public double TotalElapsedTime { get; private set; }

        public CustomTimingsManager CustomTimings => _timingManager;

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
        /// Initializes a new instance of the Timing class with default settings.
        /// The default target frames per second (FPS) is 120, and the game speed factor is set to 1.0 (normal speed).
        /// </summary>
        internal Timing()
        {
            _timingManager = new();
            TargetFps = 120;
            _gameSpeedFactor = NORMAL_GAME_SPEED;
        }

        /// <summary>
        /// Advances to the next frame, updating the timing and FPS information.
        /// </summary>
        internal void NextFrame()
        {
            // Update the timing manager
            _timingManager.UpdateTimings(Glfw.Time);

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

            // Optional FPS limiting
            if (TargetFps > 0)
            {
                double targetTime = previousTime + (1.0 / TargetFps);
                while (Glfw.Time < targetTime)
                {
                    // Do nothing or process unimportant pipelines here
                    _timingManager.UpdateTimings(Glfw.Time);
                }
                currentTime = Glfw.Time;
            }

            // Calculate the delta time between the current and previous frame
            double deltaTime = currentTime - previousTime; // In seconds

            // Apply game speed to deltaTime
            deltaTime *= GameSpeedFactor;

            CurrentFrame = new Frame(CurrentFrame + 1, deltaTime);
            TotalElapsedTime += deltaTime;
            previousTime = currentTime;
        }
    }
}