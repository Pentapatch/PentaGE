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
            float halfWidth = viewportWidth / 2.0f;
            float halfHeight = viewportHeight / 2.0f;

            float left = -halfWidth / Zoom;
            float right = halfWidth / Zoom;
            float top = halfHeight / Zoom;
            float bottom = -halfHeight / Zoom;

            return Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, NearPlaneClipping, FarPlaneClipping);
        }

        public override Matrix4x4 GetViewMatrix()
        {
            var cameraPosition = new Vector3(Transform.Position.X, Transform.Position.Y, Transform.Position.Z);
            var zTarget = IsTopDown ? Transform.Position.Z - 1.0f : 0;
            var target = new Vector3(Transform.Position.X, Transform.Position.Y, zTarget);

            return Matrix4x4.CreateLookAt(cameraPosition, target, World.UpVector);
        }
    }
}