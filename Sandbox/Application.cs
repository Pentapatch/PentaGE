using PentaGE.Core;
using PentaGE.Core.Events;
using Serilog;

namespace Sandbox
{
    internal class Application : PentaGameEngine
    {
        protected override bool Initialize()
        {
            // Do initialization work
            Timing.CustomTimings[1].Tick += Application_Tick;

            Events.KeyDown += Events_KeyDown;
            Events.KeyUp += Events_KeyUp;
            Events.KeyRepeat += Events_KeyRepeat;

            Events.MouseDown += Events_MouseDown;
            Events.MouseUp += Events_MouseUp;
            Events.MouseMoved += Events_MouseMoved;

            Events.MouseEntered += Events_MouseEntered;
            Events.MouseLeft += Events_MouseLeft;

            return true;
        }

        private void Events_MouseLeft(object? sender, MouseLeftEventArgs e)
        {
            Log.Information($"Mouse left the window.");
        }

        private void Events_MouseEntered(object? sender, MouseEnteredEventArgs e)
        {
            Log.Information($"Mouse entered the window.");
        }

        private void Events_MouseMoved(object? sender, MouseMovedEventArgs e)
        {
            Log.Information($"Mouse position was changed: {e.Position}.");
        }

        private void Events_MouseUp(object? sender, MouseUpEventArgs e)
        {
            Log.Information($"Mouse '{e.Button}' was released.");
        }

        private void Events_MouseDown(object? sender, MouseDownEventArgs e)
        {
            Log.Information($"Key '{e.Button}' was pressed.");
        }

        private void Events_KeyRepeat(object? sender, KeyDownEventArgs e)
        {
            Log.Information($"Key '{e.Key}' was held.");
        }

        private void Events_KeyUp(object? sender, KeyUpEventArgs e)
        {
            Log.Information($"Key '{e.Key}' was released.");
        }

        private void Events_KeyDown(object? sender, KeyDownEventArgs e)
        {
            Log.Information($"Key '{e.Key}' was pressed.");
        }

        private void Application_Tick(object? sender, CustomTimingTickEventArgs e)
        {
            Windows[0].Title = $"{Timing.CurrentFps}FPS : {e.ElapsedTime}s : {Timing.RunTime:h\\:mm\\:ss}";
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