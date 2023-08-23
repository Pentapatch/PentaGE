using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Rendering;
using PentaGE.Core.Rendering.Sprites;
using System.Numerics;

namespace PentaGE.Core.Entities
{
    public sealed class BillboardEntity : Entity
    {
        private readonly Guid _cameraPositionComponentId;
        private readonly Guid _SpriteRenderComponentId;

        // This entity should always recieve update events
        public override UpdateMode UpdateMode => UpdateMode.Always;

        public bool OnlyYaw { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardEntity"/> class with the specified sprite and shader.
        /// </summary>
        /// <param name="sprite">The sprite to be rendered by the entity.</param>
        /// <param name="shader">The shader used for rendering.</param>
        /// <param name="cameraController">The camera controller for camera-related data.</param>
        /// <param name="position">The position applied to the <see cref="SpriteRenderComponent"/> (optional).</param>
        /// <param name="scale">The scale applied to the <see cref="SpriteRenderComponent"/> (optional).</param>
        public BillboardEntity(Sprite sprite, Shader shader, CameraController cameraController, Vector3? position = null, Vector2? scale = null)
        {
            Transform? transform = position is Vector3 pos
                ? new Transform(pos, Rotation.Zero, scale is Vector2 v2scale ? new(v2scale.X, v2scale.Y, 0f) : Vector3.One) : null;

            var spriteRenderComponent = new SpriteRenderComponent(sprite, shader, transform)
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