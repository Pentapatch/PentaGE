using PentaGE.Viewports;
using System.Drawing;
using System.Numerics;

namespace PentaGE.Rendering
{
    internal class Renderer
    {

        internal static void RenderScene(Graphics g, Viewport viewport)
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

        internal static void RenderViewport(Viewport viewport, Graphics g)
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
    }
}