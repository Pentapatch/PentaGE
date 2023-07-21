using PentaGE.Core;

namespace Sandbox
{
    internal class Application : PentaGameEngine
    {
        protected override void Initialize()
        {
            // Do initialization work
            Timing.CustomTimings[1].Tick += Application_Tick;
        }

        private void Application_Tick(object? sender, CustomTimingTickEventArgs e)
        {
            Windows[0].Title = $"{Timing.CurrentFps}FPS : {e.ElapsedTime}s";
        }

        protected override void Shutdown()
        {
            // Unload resources
        }

        protected override void Update()
        {
            // Update game state
        }

    }
}