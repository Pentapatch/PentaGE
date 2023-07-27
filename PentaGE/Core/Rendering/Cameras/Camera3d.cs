using PentaGE.Common;
using PentaGE.Maths;
using System.Numerics;

namespace PentaGE.Core.Rendering
{
    public sealed class Camera3d : Camera
    {
        public float FieldOfView { get; set; }

        public float AspectRatio { get; private set; }

        public Rotation Rotation { get; set; }

        public Camera3d() : base() { }

        public Camera3d(float fieldOfView) : base()
        {
            FieldOfView = fieldOfView;
        }

        public override Matrix4x4 GetProjectionMatrix(int viewportWidth, int viewportHeight)
        {
            // Calculate the aspect ratio based on the viewport dimensions
            AspectRatio = (float)viewportWidth / viewportHeight;

            // Create the perspective projection matrix
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(FieldOfView),
                AspectRatio,
                NearPlaneClipping,
                FarPlaneClipping
            );

            return projectionMatrix;
        }

        public override Matrix4x4 GetViewMatrix()
        {
            // Extract the camera's position, target, and up vector
            var cameraPosition = new Vector3(-Position.X, -Position.Y, Position.Z);
            var target = cameraPosition + Rotation.GetForwardVector();
            var up = Rotation.GetUpVector();

            // Create the view matrix using the camera's position, target, and up vector
            var viewMatrix = Matrix4x4.CreateLookAt(cameraPosition, target, up);

            // Flip the view matrix horizontally to move the scene to the left
            viewMatrix = Matrix4x4.CreateScale(-1.0f, 1.0f, 1.0f) * viewMatrix;

            return viewMatrix;
        }
    }
}