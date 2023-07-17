using PentaGE.MathExtensions;
using PentaGE.Structs;
using System.Numerics;

namespace PentaGE.Camera
{
    public class Camera : IPositionable, IOrientable
    {
        public Placement Placement { get; set; } = new();

        public Vector3 Position => Placement.Position;

        public EulerAngles Orientation => Placement.Orientation;

        public float FieldOfView { get; set; } = 60;

        public float NearClipPlane { get; set; } = 0.1f;

        public float FarClipPlane { get; set; } = 1000f;

        public float AspectRatio { get; set; } = 4f / 3;

        public Plane[] GetFrustumPlanes()
        {
            Matrix4x4 viewMatrix = CalculateViewMatrix();
            Matrix4x4 projectionMatrix = CalculateProjectionMatrix();
            Matrix4x4 viewProjectionMatrix = viewMatrix * projectionMatrix;

            // Extract the frustum planes from the combined view-projection matrix
            Plane[] frustumPlanes = new Plane[6];
            frustumPlanes[0] = new Plane(
                viewProjectionMatrix.M14 + viewProjectionMatrix.M11,
                viewProjectionMatrix.M24 + viewProjectionMatrix.M21,
                viewProjectionMatrix.M34 + viewProjectionMatrix.M31,
                viewProjectionMatrix.M44 + viewProjectionMatrix.M41
            );
            // Extract the other frustum planes in a similar way

            return frustumPlanes;
        }

        private Matrix4x4 CalculateViewMatrix()
        {
            // Calculate the view matrix based on the camera's position, target, and up vector
            return Matrix4x4.CreateLookAt(Position, Position + Orientation.GetForwardVector(), Orientation.GetUpVector());
        }

        private Matrix4x4 CalculateProjectionMatrix()
        {
            // Calculate the projection matrix based on the camera's field of view, aspect ratio, near and far clip planes
            return Matrix4x4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(FieldOfView),
                (float)AspectRatio,
                (float)NearClipPlane,
                (float)FarClipPlane
            );
        }

    }
}