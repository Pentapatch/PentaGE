using System.Numerics;

namespace PentaGE.Core.Rendering
{
    public sealed class Camera3d : Camera
    {
        public float FieldOfView { get; set; }

        public float AspectRatio { get; private set; }

        public Camera3d() : base() { }

        public Camera3d(float fieldOfView, float aspectRatio) : base()
        {
            FieldOfView = fieldOfView;
            AspectRatio = aspectRatio;
        }

        public override Matrix4x4 GetProjectionMatrix(int viewportWidth, int viewportHeight)
        {
            // Calculate the aspect ratio based on the viewport dimensions
            AspectRatio = (float)viewportWidth / viewportHeight;

            // Create the perspective projection matrix
            Matrix4x4 projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI * FieldOfView / 180.0f, // Convert field of view from degrees to radians
                AspectRatio,
                NearPlaneClipping,
                FarPlaneClipping
            );

            return projectionMatrix;
        }

        public override Matrix4x4 GetViewMatrix()
        {
            // Extract the camera's position, target, and up vector
            Vector3 cameraPosition = Transform.Position;
            Vector3 target = cameraPosition + Rotation.GetForwardVector();
            Vector3 up = Rotation.GetUpVector();

            // Create the view matrix using the camera's position, target, and up vector
            Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(cameraPosition, target, up);

            return viewMatrix;
        }
    }
}