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
            var cameraPosition = new Vector3(Position.X, Position.Y, Position.Z);
            var zTarget = IsTopDown ? Position.Z - 1.0f : 0;
            var target = new Vector3(Position.X, Position.Y, zTarget);

            return Matrix4x4.CreateLookAt(cameraPosition, target, World.UpVector);
        }
    }
}