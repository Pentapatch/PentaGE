using PentaGE.Common;
using System.Numerics;

namespace PentaGE.Core.Rendering
{
    public sealed class Camera2d : Camera
    {
        public float Zoom { get; set; } = 1.0f;

        public bool IsTopDown { get; set; } = false;

        public Camera2d() : base() { }

        public Camera2d(float zoom, bool isTopDown) : base()
        {
            Zoom = zoom;
            IsTopDown = isTopDown;
        } 

        public override Matrix4x4 GetProjectionMatrix(int viewportWidth, int viewportHeight)
        {
            var halfWidth = viewportWidth / 2.0f;
            var halfHeight = viewportHeight / 2.0f;

            var left = -halfWidth / Zoom;
            var right = halfWidth / Zoom;
            var top = halfHeight / Zoom;
            var bottom = -halfHeight / Zoom;

            return Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, NearPlaneClipping, FarPlaneClipping);
        }

        public override Matrix4x4 GetViewMatrix()
        {
            // Invert the camera's position to move the scene in the opposite direction
            var cameraPosition = new Vector3(-Position.X, -Position.Y, -1);
            var zTarget = IsTopDown ? Position.Z - 1.0f : 0;
            var target = new Vector3(-Position.X, -Position.Y, zTarget);

            // Create the view matrix using the inverted camera position
            Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(cameraPosition, target, World.UpVector);

            // Flip the view matrix horizontally to move the scene to the left
            viewMatrix = Matrix4x4.CreateScale(-1.0f, 1.0f, 1.0f) * viewMatrix;

            return viewMatrix;
        }
    }
}