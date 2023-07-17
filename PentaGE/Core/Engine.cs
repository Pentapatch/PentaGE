using PentaGE.GameLoop;
using System.Drawing;

namespace PentaGE.Core
{
    public class Engine
    {
        private readonly Timing _timing = new();
        private bool isRunning = false;
        private bool isPaused = false;

        public delegate void RenderHandler(object sender, EventArgs e);

        public event RenderHandler Render;

        internal Timing Timing => _timing;

        protected virtual void OnRender(EventArgs e)
        {
            RenderHandler handler = Render;
            handler?.Invoke(this, e);
        }

        public void Run()
        {
            isRunning = true;
            while (isRunning)
            {
                // Update game state
                // Do work here

                // Render game
                OnRender(EventArgs.Empty);

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
            g.Clear(Color.FromArgb(24, 24, 24));

            if (Timing.Frame == 1)
            {
                x1 = 0;
                x2 = 0;
                x3 = 0;
                x4 = 0;
                x5 = 0;
            }
            x1 += 200 * Timing.Frame.Delta;
            x2 += 600 * Timing.Frame.Delta;
            x3 += 1200 * Timing.Frame.Delta;
            x4 += 1800 * Timing.Frame.Delta;
            x5 += 2500 * Timing.Frame.Delta;

            g.FillEllipse(Brushes.Green, 200, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Green, 600, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Green, 1200, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Green, 1800, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Green, 2500, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Red, (int)x1, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Red, (int)x2, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Red, (int)x3, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Red, (int)x4, g.ClipBounds.Height / 2, 10, 10);
            g.FillEllipse(Brushes.Red, (int)x5, g.ClipBounds.Height / 2, 10, 10);

            //int x = (int)(gameLoop.Frame.Progress * Form.ClientRectangle.Width);
            //g.DrawLine(Pens.Yellow, x, 0, x, Form.ClientRectangle.Height);

            g.DrawString(Timing.Frame.ToString() + $"\nFPS: {Timing.CurrentFps}\nClock: {Timing.Clock}", new Font("Segoe UI", 11, FontStyle.Bold), Brushes.White, new Point(10, 10));
        }

    }
}