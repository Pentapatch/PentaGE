using PentaGE.Common;
using PentaGE.Maths;
using System.Numerics;

namespace PentaGE.Core.Rendering
{
    public sealed class Camera3d : Camera
    {
        public float FieldOfView { get; set; }

        public float AspectRatio { get; private set; }

        public Rotation  Rotation { get; set; }

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
            var targetPosition = Position - Rotation.GetForwardVector();
            var viewMatrix = Matrix4x4.CreateLookAt(Position, targetPosition, Rotation.GetUpVector());

            return viewMatrix;
        }
    }
}