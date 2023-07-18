using PentaGE.GameLoop;
using PentaGE.Rendering;
using PentaGE.Viewports;
using System.Drawing;

namespace PentaGE.Core
{
    public class Engine
    {
        private readonly Timing _timing = new();
        private readonly Renderer _renderer = new();

        private bool isRunning = false;
        private bool isPaused = false;
        private List<Viewport> _viewports = new();

        public delegate void InvalidateHandler(object sender, EventArgs e);

        public event InvalidateHandler Invalidate;

        internal Timing Timing => _timing;

        internal Renderer Renderer => _renderer;

        public List<Viewport> Viewports => _viewports;

        protected virtual void OnInvalidate(EventArgs e)
        {
            InvalidateHandler handler = Invalidate;
            handler?.Invoke(this, e);
        }

        public void Run()
        {
            isRunning = true;
            while (isRunning)
            {
                // Update game state
                // TODO: Do work here

                // Ask the owner form to invalidate itself
                // The owner form should subscribe to the Form_Paint event
                // and call Engine.RenderGraphics(e.Graphics) to trigger the render.
                OnInvalidate(EventArgs.Empty);

                // Get the next frame
                Timing.NextFrame();
            }
        }

        public void Stop()
        {
            isRunning = false;
            isPaused = false;
        }

        public void Render(Graphics g)
        {
            if (isPaused) return;

            foreach (var viewport in Viewports)
            {
                Renderer.RenderViewport(viewport, g);
            }

            // Temporary solution: Draw stats
            //g.DrawString(Timing.Frame.ToString() + $"\nFPS: {Timing.CurrentFps}\nClock: {Timing.Clock}", new Font("Segoe UI", 11, FontStyle.Bold), Brushes.White, new Point(10, 10));
        }

    }
}