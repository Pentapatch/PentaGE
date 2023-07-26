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

        public Vector3 Position { get; set; }

        public Camera()
        {
            NearPlaneClipping = 0.1f;  
            FarPlaneClipping = 1000.0f; 
            Position = Vector3.Zero;
        }
    }
}