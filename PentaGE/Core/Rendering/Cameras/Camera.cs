using System.Numerics;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents an abstract camera with properties and methods for managing view and projection matrices.
    /// </summary>
    public abstract class Camera
    {

        /// <summary>
        /// Calculates and returns the view matrix of the camera.
        /// </summary>
        /// <returns>The view matrix.</returns>
        public abstract Matrix4x4 GetViewMatrix();

        /// <summary>
        /// Calculates and returns the projection matrix of the camera.
        /// </summary>
        /// <param name="viewportWidth">The width of the viewport.</param>
        /// <param name="viewportHeight">The height of the viewport.</param>
        /// <returns>The projection matrix.</returns>
        public abstract Matrix4x4 GetProjectionMatrix(int viewportWidth, int viewportHeight);

        /// <summary>
        /// Gets or sets the distance to the near clipping plane.
        /// </summary>
        public float NearPlaneClipping { get; set; }

        /// <summary>
        /// Gets or sets the distance to the far clipping plane.
        /// </summary>
        public float FarPlaneClipping { get; set; }

        /// <summary>
        /// Gets or sets the position of the camera in 3D space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class with default values.
        /// </summary>
        public Camera()
        {
            NearPlaneClipping = 0.1f;
            FarPlaneClipping = 1000.0f;
            Position = Vector3.Zero;
        }
    }
}