using PentaGE.Common;
using System.Numerics;

namespace PentaGE.Core.Rendering
{
    public abstract class Camera
    {

        public abstract Matrix4x4 GetViewMatrix();

        public abstract Matrix4x4 GetProjectionMatrix(int viewportWidth, int viewportHeight);

        public float NearPlaneClipping { get; set; }

        public float FarPlaneClipping { get; set; }

        public Transform Transform { get; set; }

        public Rotation Rotation { get; set; }

    }
}