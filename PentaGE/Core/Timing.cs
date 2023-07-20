using System.Diagnostics;

namespace PentaGE.Core
{
    internal class Timing
    {
        private readonly Stopwatch stopwatch = new();
        private int totalElapsedMilliseconds = 0;

        private const int GAME_SPEED_BASE_VALUE = 1000;

        public Frame Frame { get; private set; } = new();

        public int TargetFps { get; set; } = 60;

        public int CurrentFps { get; private set; } = 0;

        public int SimulateThrottle { get; set; } = 0;

        public double GameSpeed { get; set; } = 1d;

        public int Clock { get; private set; } = 0;

        private int GetGameSpeed() => (int)(GAME_SPEED_BASE_VALUE / GameSpeed);

        public void NextFrame()
        {
            if (!stopwatch.IsRunning) stopwatch.Start();

            int targetFrameTime = (int)(1d / TargetFps * GAME_SPEED_BASE_VALUE);

            // Check if we should restart the frame count
            int overshootMilliseconds = 0;
            if (totalElapsedMilliseconds >= GAME_SPEED_BASE_VALUE)
            {
                CurrentFps = Frame;
                overshootMilliseconds = totalElapsedMilliseconds - GAME_SPEED_BASE_VALUE;
                totalElapsedMilliseconds = overshootMilliseconds;
                Frame = new(0, overshootMilliseconds, totalElapsedMilliseconds, GetGameSpeed());
                Clock += 1;
            }

            // Random trottling
            SimulateThrottling();

            // Limit the frame rate
            int elapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds + overshootMilliseconds;
            if (TargetFps > 0 && elapsedMilliseconds < targetFrameTime)
            {
                while (stopwatch.ElapsedMilliseconds < targetFrameTime + 1)
                {
                    // Do nothing while waiting out the time
                }
            }

            // Update the current frame
            elapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds;
            totalElapsedMilliseconds += elapsedMilliseconds;
            Frame = new(Frame + 1, elapsedMilliseconds, totalElapsedMilliseconds, GetGameSpeed());

            stopwatch.Restart();
        }

        private void SimulateThrottling()
        {
            if (SimulateThrottle <= 0) return;

            if (Random.Shared.NextDouble() <= 0.15)
            {
                Thread.Sleep(Random.Shared.Next(0, SimulateThrottle));
            }
        }
    }
}