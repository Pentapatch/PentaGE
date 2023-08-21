using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Entities;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;
using System.Numerics;

namespace PentaGE.Core.Entities
{
    public sealed class DirectionalLightEntity : Entity
    {
        private readonly Guid _rotationComponentId;

        // This entity can only have one instance in the scene
        public override bool CanHaveMultipleInstances => false;

        /// <summary>
        /// Gets the direction of the light.
        /// </summary>
        internal Vector3 Direction => GetReference<RotationComponent>(_rotationComponentId)!.Rotation.GetForwardVector();

        /// <summary>
        /// Gets or sets the position of the widget.
        /// </summary>
        public Transform WidgetTransform
        {
            get => Components.Get<TransformComponent>()!.Transform;
            set => Components.Get<TransformComponent>()!.Transform = value;
        }

        /// <summary>
        /// Gets or sets the rotation of the light.
        /// </summary>
        public Rotation Rotation
        {
            get => Components.Get<RotationComponent>()!.Rotation;
            set => Components.Get<RotationComponent>()!.Rotation = value;
        }

        /// <summary>
        /// Gets or sets the color of the light.
        /// </summary>
        public Vector4 Color
        {
            get => Components.Get<ColorComponent>()!.Color;
            set => Components.Get<ColorComponent>()!.Color = value;
        }

        /// <summary>
        /// Gets or sets whether the light should follow the camera.
        /// </summary>
        public bool FollowCamera { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectionalLightEntity"/> class.
        /// </summary>
        /// <param name="mesh">The mesh for the widget.</param>
        /// <param name="shader">The shader for rendering the widget.</param>
        /// <param name="rotation">The initial rotation of the sun.</param>
        /// <param name="widgetTransform">The initial transform of the widget.</param>
        /// <param name="color">The color of the sun.</param>
        /// <param name="followCamera">Indicates if the light should follow the camera.</param>
        public DirectionalLightEntity(
            Mesh mesh,
            Shader shader,
            Rotation? rotation = null,
            Transform? widgetTransform = null,
            Vector4? color = null,
            bool followCamera = false)
        {
            var rotationComponent = new RotationComponent()
            {
                Rotation = rotation is not null ? (Rotation)rotation : new(45, -45, 0)
            };

            Components.Add(rotationComponent);
            _rotationComponentId = SetReference(rotationComponent);

            Components.Add(new TransformComponent()
            {
                Transform = widgetTransform is not null ? (Transform)widgetTransform : new(new(0, 1, 0), Rotation.Zero, Vector3.One)
            });

            Components.Add(new ColorComponent()
            {
                Color = color is not null ? (Vector4)color : new(1, 1, 1, 1)
            });

            // Add widget mesh
            var renderableMeshComponent = new MeshRenderComponent(mesh, shader, texture: null, material: null);
            Components.Add(renderableMeshComponent);

            FollowCamera = followCamera;
        }

        public override void Update(float deltaTime)
        {
            // TODO: Remove this entire update method as its only for show
            var rotationComponent = GetReference<RotationComponent>(_rotationComponentId);
            var rot = rotationComponent!.Rotation;
            rot = new(rot.Yaw + deltaTime * 10, rot.Pitch, rot.Roll);
            rotationComponent.Rotation = rot;
        }
    }
}