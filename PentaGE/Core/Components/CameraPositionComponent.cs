using PentaGE.Core.Rendering;
using System.Numerics;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Represents a component that provides access to the position of a camera within the game world.
    /// </summary>
    public class CameraPositionComponent : Component
    {
        private readonly CameraController _cameraController;

        /// <inheritdoc />
        public override bool CanHaveMultiple => false;

        /// <summary>
        /// Gets the position of the active camera.
        /// </summary>
        public Vector3 Position =>
            _cameraController.ActiveCamera is Camera camera ? camera.Position : Vector3.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPositionComponent"/> class.
        /// </summary>
        /// <param name="cameraController">The camera controller providing camera-related data.</param>
        public CameraPositionComponent(CameraController cameraController)
        {
            _cameraController = cameraController;
        }

        /// <inheritdoc />
        public override object Clone() => MemberwiseClone();

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            // Do nothing
        }
    }
}