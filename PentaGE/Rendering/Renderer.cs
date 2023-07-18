using PentaGE.Cameras;
using PentaGE.GameObjects;
using PentaGE.Structs;
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

            List<TestGameObject> gameObjects = new()
            {
                new(),
            };

            foreach (var gameObject in gameObjects)
            {
                // Check if the object is within the camera's frustum
                if (IsObjectInFrustum(gameObject.Transform, frustumPlanes))
                {
                    // Calculate the position of the object in screen space
                    Point screenPosition = WorldToScreen(gameObject.Transform.Position, viewport.ActiveCamera);

                    // Render the object
                    RenderGameObject(g, gameObject, screenPosition);
                }
            }

            RenderAxes(g, viewport.ActiveCamera);

            // Perform your rendering logic here
            // Iterate through your scene objects and render them based on the frustum culling

            // You can also use the camera's view and projection matrices as needed
            //Matrix4x4 viewMatrix = ActiveCamera.CalculateViewMatrix();
            //Matrix4x4 projectionMatrix = ActiveCamera.CalculateProjectionMatrix();

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

        private static bool IsObjectInFrustum(Transform transform, Plane[] frustumPlanes)
        {
            // Implement frustum culling logic here to determine if the object is within the frustum
            // You can use the frustum planes to perform the necessary checks

            return true; // For now, assume all objects are within the frustum
        }

        private static Point WorldToScreen(Vector3 worldPosition, Camera camera)
        {
            // Apply the camera's view and projection matrices to the world position
            Vector3 transformedPosition = Vector3.Transform(worldPosition, camera.CalculateViewMatrix() * camera.CalculateProjectionMatrix());

            // Perform any additional transformations or calculations here

            // Map the transformed position to screen coordinates
            int screenX = (int)transformedPosition.X;
            int screenY = (int)transformedPosition.Y;

            // Return the screen coordinates as a Point
            return new Point(screenX, screenY);
        }

        private static void RenderGameObject(Graphics g, TestGameObject gameObject, Point screenPosition)
        {
            // Render the game object using a circle
            g.FillEllipse(new SolidBrush(gameObject.Color), screenPosition.X, screenPosition.Y, gameObject.Size.Width, gameObject.Size.Height);
        }

        private static void RenderAxes(Graphics g, Camera camera)
        {
            // Retrieve the camera's view matrix
            Matrix4x4 viewMatrix = camera.CalculateViewMatrix();

            // Get the camera's position and orientation vectors
            Vector3 cameraPosition = camera.Position;
            Vector3 cameraForward = camera.Orientation.GetForwardVector();
            Vector3 cameraUp = camera.Orientation.GetUpVector();
            Vector3 cameraRight = camera.Orientation.GetRightVector();

            // Calculate the screen position of the camera
            Point cameraScreenPosition = WorldToScreen(cameraPosition, camera);

            // Calculate the lengths of the axis arrows
            float arrowLength = 100f;
            float arrowHeadLength = 10f;

            // Draw the X-axis arrow in green
            Point xArrowStart = cameraScreenPosition;
            Point xArrowEnd = WorldToScreen(cameraPosition + arrowLength * cameraRight, camera);
            DrawArrow(g, xArrowStart, xArrowEnd, Color.Green, arrowHeadLength);

            // Draw the Y-axis arrow in red
            Point yArrowStart = cameraScreenPosition;
            Point yArrowEnd = WorldToScreen(cameraPosition + arrowLength * cameraUp, camera);
            DrawArrow(g, yArrowStart, yArrowEnd, Color.Red, arrowHeadLength);

            // Draw the Z-axis arrow in blue
            Point zArrowStart = cameraScreenPosition;
            Point zArrowEnd = WorldToScreen(cameraPosition + arrowLength * cameraForward, camera);
            DrawArrow(g, zArrowStart, zArrowEnd, Color.Blue, arrowHeadLength);
        }

        private static void DrawArrow(Graphics g, Point start, Point end, Color color, float arrowHeadLength)
        {
            using (Pen pen = new(color, 2f))
            {
                // Draw the line segment
                g.DrawLine(pen, start, end);

                // Calculate the direction vector of the arrow
                PointF direction = new(end.X - start.X, end.Y - start.Y);
                float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                direction.X /= length;
                direction.Y /= length;

                // Calculate the arrowhead points
                PointF arrowPoint1 = new PointF(end.X + arrowHeadLength * (-direction.Y), end.Y + arrowHeadLength * direction.X);
                PointF arrowPoint2 = new PointF(end.X + arrowHeadLength * direction.Y, end.Y + arrowHeadLength * (-direction.X));

                // Draw the arrowhead
                g.DrawLine(pen, end, arrowPoint1);
                g.DrawLine(pen, end, arrowPoint2);
            }
        }
    }
}