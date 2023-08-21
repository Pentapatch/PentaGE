using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;

namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents a sun entity in the game world that emulates the position of the sun
    /// based on a directional light and camera position.
    /// </summary>
    public class SunEntity : Entity
    {
        private readonly Guid _directionalLightId;
        private readonly Guid _sunMeshRenderComponentId;
        private readonly Guid _cameraPositionComponentId;

        // This entity should always recieve update events
        public override UpdateMode UpdateMode => UpdateMode.Always;

        /// <summary>
        /// Gets or sets the distance of the sun from the camera.
        /// </summary>
        public int Distance { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SunEntity"/> class.
        /// </summary>
        /// <param name="mesh">The mesh for the sun.</param>
        /// <param name="shader">The shader for rendering the sun.</param>
        /// <param name="cameraController">The camera controller for camera-related data.</param>
        /// <param name="directionalLightEntity">The directional light entity to follow.</param>
        /// <param name="transform">The initial transform of the sun.</param>
        public SunEntity(
            Mesh mesh,
            Shader shader,
            CameraController cameraController,
            DirectionalLightEntity? directionalLightEntity = null,
            Transform? transform = null)
        {
            _directionalLightId = SetReference(directionalLightEntity);

            // Add sun mesh
            var meshRenderComponent = new MeshRenderComponent(mesh, shader, texture: null, material: null, transform ?? Transform.Identity);
            Components.Add(meshRenderComponent);
            _sunMeshRenderComponentId = SetReference(meshRenderComponent);

            Distance = 100;

            var cameraPositionComponent = new CameraPositionComponent(cameraController);
            Components.Add(cameraPositionComponent);
            _cameraPositionComponentId = SetReference(cameraPositionComponent);
        }

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            var direction = GetReference<DirectionalLightEntity>(_directionalLightId) is DirectionalLightEntity directionalLight ?
                directionalLight.Direction : World.ForwardVector;

            // Update the transform of the sun
            if (GetReference<MeshRenderComponent>(_sunMeshRenderComponentId) is not MeshRenderComponent sun) return;
            if (sun.Transform is not Transform transform) return;

            if (GetReference<CameraPositionComponent>(_cameraPositionComponentId) is not CameraPositionComponent cameraPosition) return;

            transform.Position = cameraPosition.Position + direction * Distance;

            sun.Transform = transform;
        }
    }
}