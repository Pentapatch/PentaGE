using PentaGE.GameLoop;
using PentaGE.Viewports;
using System.Drawing;
using System.Numerics;

namespace PentaGE.Core
{
    public class Engine
    {
        private readonly Timing _timing = new();
        private bool isRunning = false;
        private bool isPaused = false;
        private List<Viewport> _viewports = new();

        public delegate void InvalidateHandler(object sender, EventArgs e);

        public event InvalidateHandler Invalidate;

        internal Timing Timing => _timing;

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

        private double x1 = 0;
        private double x2 = 0;
        private double x3 = 0;
        private double x4 = 0;
        private double x5 = 0;

        public void RenderGraphics(Graphics g)
        {
            foreach (var viewport in Viewports)
            {
                if (viewport.ActiveCamera is null) return;

                // Set up the viewport
                g.SetClip(new Rectangle(viewport.Left, viewport.Top, viewport.Width, viewport.Height));
                g.TranslateTransform(viewport.Left, viewport.Top);

                // Clear the viewport
                g.Clear(Color.Black);

                // Render using the active camera
                RenderScene(g, viewport);

                // Reset transformations
                g.ResetTransform();
                g.ResetClip();
            }

            g.DrawString(Timing.Frame.ToString() + $"\nFPS: {Timing.CurrentFps}\nClock: {Timing.Clock}", new Font("Segoe UI", 11, FontStyle.Bold), Brushes.White, new Point(10, 10));

            //g.Clear(Color.FromArgb(24, 24, 24));

            //if (Timing.Frame == 1)
            //{
            //    x1 = 0;
            //    x2 = 0;
            //    x3 = 0;
            //    x4 = 0;
            //    x5 = 0;
            //}
            //x1 += 200 * Timing.Frame.Delta;
            //x2 += 600 * Timing.Frame.Delta;
            //x3 += 1200 * Timing.Frame.Delta;
            //x4 += 1800 * Timing.Frame.Delta;
            //x5 += 2500 * Timing.Frame.Delta;

            //g.FillEllipse(Brushes.Green, 200, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Green, 600, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Green, 1200, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Green, 1800, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Green, 2500, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Red, (int)x1, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Red, (int)x2, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Red, (int)x3, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Red, (int)x4, g.ClipBounds.Height / 2, 10, 10);
            //g.FillEllipse(Brushes.Red, (int)x5, g.ClipBounds.Height / 2, 10, 10);

            ////int x = (int)(gameLoop.Frame.Progress * Form.ClientRectangle.Width);
            ////g.DrawLine(Pens.Yellow, x, 0, x, Form.ClientRectangle.Height);

            //g.DrawString(Timing.Frame.ToString() + $"\nFPS: {Timing.CurrentFps}\nClock: {Timing.Clock}", new Font("Segoe UI", 11, FontStyle.Bold), Brushes.White, new Point(10, 10));
        }

        private static void RenderScene(Graphics g, Viewport viewport)
        {
            if (viewport.ActiveCamera is null) return;

            // Retrieve the frustum planes for culling objects
            Plane[] frustumPlanes = viewport.ActiveCamera.GetFrustumPlanes();

            // Perform your rendering logic here
            // Iterate through your scene objects and render them based on the frustum culling
            // You can use the Graphics object to draw shapes, lines, text, etc.
            // For example:
            g.DrawLine(Pens.Red, 0, 0, viewport.Width, viewport.Height);
        }

    }
}