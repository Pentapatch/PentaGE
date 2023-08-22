using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;
using PentaGE.Core.Rendering.Sprites;

namespace PentaGE.Core.Entities
{
    public sealed class BillboardEntity : Entity
    {
        private readonly Guid _cameraPositionComponentId;
        private readonly Guid _SpriteRenderComponentId;

        // This entity should always recieve update events
        public override UpdateMode UpdateMode => UpdateMode.Always;

        public bool OnlyYaw { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardEntity"/> class with the specified sprite and shader.
        /// </summary>
        /// <param name="sprite">The sprite to be rendered by the entity.</param>
        /// <param name="shader">The shader used for rendering.</param>
        /// <param name="cameraController">The camera controller for camera-related data.</param>
        /// <param name="mesh">The mesh for the entity.</param>'
        /// <param name="transform">The transform of the entity.</param>
        /// <param name="meshTransform">The transform of the mesh.</param>
        public BillboardEntity(
            Sprite sprite,
            Shader shader,
            CameraController cameraController,
            Mesh? mesh = null,
            Transform? transform = null,
            Transform? meshTransform = null)
        {
            var spriteRenderComponent = new SpriteRenderComponent(sprite, shader, mesh, transform, meshTransform)
            {
                EnableCulling = true
            };
            Components.Add(spriteRenderComponent);
            _SpriteRenderComponentId = SetReference(spriteRenderComponent);

            var cameraPositionComponent = new CameraPositionComponent(cameraController);
            Components.Add(cameraPositionComponent);
            _cameraPositionComponentId = SetReference(cameraPositionComponent);
        }

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            // Update the transform of the sun
            if (GetReference<SpriteRenderComponent>(_SpriteRenderComponentId) is not SpriteRenderComponent sprite) return;
            var transform = sprite.GetTransform();

            if (GetReference<CameraPositionComponent>(_cameraPositionComponentId) is not CameraPositionComponent cameraPosition) return;

            var lookAtRotation = Rotation.GetLookAt(transform.Position, cameraPosition.Position);
            transform.Rotation = OnlyYaw ? new Rotation(
                lookAtRotation.Yaw,
                transform.Rotation.Pitch,
                transform.Rotation.Roll) : lookAtRotation;

            sprite.Transform = transform;
        }
    }
}