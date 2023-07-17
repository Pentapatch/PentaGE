using PentaGE.MathExtensions;
using PentaGE.Structs;
using PentaGE.WorldGrid;
using System.Numerics;

namespace PentaGE.Cameras
{
    public class Camera : IPlacable, IPositionable, IOrientable
    {
        private Placement _placement = new();

        public Placement Placement { get => _placement; set => _placement = value; }
        
        public Vector3 Position { get => _placement.Position; set => _placement.Position = value; }

        public EulerAngles Orientation { get => _placement.Orientation; set => _placement.Orientation = value; }

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

        #region Factory methods

        public static Camera CreateCamera(Vector3 orientationVector, Vector3? position = null, float? fieldOfView = null, float? aspectRatio = null)
        {
            var camera = new Camera() { Orientation = EulerAngles.FromVector3(orientationVector) };

            if (position is not null) camera.Position = position.Value;
            if (fieldOfView is not null) camera.FieldOfView = fieldOfView.Value;
            if (aspectRatio is not null) camera.AspectRatio = aspectRatio.Value;

            return camera;
        }

        public static Camera CreateTopDownCamera(Vector3? position = null, float? fieldOfView = null, float? aspectRatio = null) =>
            CreateCamera(World.DownVector, position, fieldOfView, aspectRatio);

        public static Camera CreateSideScrollerCamera(Vector3? position = null, float? fieldOfView = null, float? aspectRatio = null) => 
            CreateCamera(World.ForwardVector, position, fieldOfView, aspectRatio);

        #endregion

    }
}