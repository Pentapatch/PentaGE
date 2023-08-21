using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;
using PentaGE.Core.Scenes;

namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents a sun entity in the game world that emulates the position of the sun
    /// based on a directional light and camera position.
    /// </summary>
    public class SunEntity : Entity
    {
        private readonly Guid? _directionalLightId;
        private readonly Guid _meshRenderComponentId;
        private readonly Guid _cameraPositionComponentId;
        private DirectionalLightEntity? _directionalLight;
        private MeshRenderComponent? _sun;
        private CameraPositionComponent? _cameraPosition;

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
            _directionalLightId = directionalLightEntity?.ID;

            // Add sun mesh
            var meshRenderComponent = new MeshRenderComponent(mesh, shader, texture: null, material: null, transform ?? Transform.Identity);
            if (Components.Add(meshRenderComponent) is Guid meshRenderId)
            {
                _meshRenderComponentId = meshRenderId;
            };

            Distance = 100;

            var cameraPositionComponent = new CameraPositionComponent(cameraController);
            if (Components.Add(cameraPositionComponent) is Guid cameraPositionId)
            {
                _cameraPositionComponentId = cameraPositionId;
            };
        }

        /// <inheritdoc />
        public override void SceneBegin(Scene scene)
        {
            if (_directionalLightId is Guid directionalLightId)
                _directionalLight = scene.Get<DirectionalLightEntity>(directionalLightId);

            if (_meshRenderComponentId is Guid meshRenderComponentId)
                _sun = Components.Get<MeshRenderComponent>(meshRenderComponentId);

            if (_cameraPositionComponentId is Guid cameraPositionId)
                _cameraPosition = Components.Get<CameraPositionComponent>(cameraPositionId);
        }

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            var direction = _directionalLight is DirectionalLightEntity directionalLight ? 
                directionalLight.Direction : World.ForwardVector;

            // Update the transform of the sun
            if (_sun is not MeshRenderComponent sun) return;
            if (sun.Transform is not Transform transform) return;

            if (_cameraPosition is not CameraPositionComponent cameraPosition) return;

            transform.Position = cameraPosition.Position + direction * Distance;

            sun.Transform = transform;
        }
    }
}