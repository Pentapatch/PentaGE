using PentaGE.Common;
using PentaGE.Maths;
using Serilog;
using System.Numerics;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents a 3D camera for viewing scenes in 3D space with perspective projection.
    /// </summary>
    public sealed class Camera3d : Camera
    {
        public const float MinFieldOfView = 10;
        public const float MaxFieldOfView = 170;

        private float _fieldOfView = 90;

        /// <summary>
        /// Gets or sets the field of view of the camera in degrees.
        /// Valid range is between <see cref="MinFieldOfView"/> and <see cref="MaxFieldOfView"/>.
        /// </summary>
        public float FieldOfView
        {
            get => _fieldOfView;
            set
            {
                if (value > MaxFieldOfView)
                {
                    Log.Warning($"Field of view cannot be greater than {MaxFieldOfView} degrees");
                    value = MaxFieldOfView;
                }
                else if (value < MinFieldOfView)
                {
                    Log.Warning($"Field of view cannot be less than {MinFieldOfView} degrees");
                    value = MinFieldOfView;
                }

                _fieldOfView = value;
            }
        }

        /// <summary>
        /// Gets the aspect ratio of the camera's viewport.
        /// </summary>
        public float AspectRatio { get; private set; }

        /// <summary>
        /// Gets or sets the rotation of the camera.
        /// </summary>
        public Rotation Rotation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera3d"/> class with default values.
        /// </summary>
        public Camera3d() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera3d"/> class with a specified field of view.
        /// </summary>
        /// <param name="fieldOfView">The field of view in degrees.</param>
        public Camera3d(float fieldOfView) : base()
        {
            FieldOfView = fieldOfView;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override Matrix4x4 GetViewMatrix()
        {
            var targetPosition = Position - Rotation.GetForwardVector();
            var viewMatrix = Matrix4x4.CreateLookAt(Position, targetPosition, Rotation.GetUpVector());

            return viewMatrix;
        }
    }
}