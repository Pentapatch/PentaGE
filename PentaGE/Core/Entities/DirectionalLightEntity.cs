using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;
using System.Numerics;

namespace PentaGE.Core.Entities
{
    public sealed class DirectionalLightEntity : Entity
    {
        // This entity can only have one instance in the scene
        public override bool CanHaveMultipleInstances => false;

        /// <summary>
        /// Gets the direction of the light.
        /// </summary>
        internal Vector3 Direction => Components.Get<RotationComponent>()!.Rotation.GetForwardVector();

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

        public DirectionalLightEntity(Mesh mesh, Shader shader, Rotation? rotation = null, Transform? widgetTransform = null,
                                      Vector4? color = null, bool followCamera = false)
        {
            Components.Add(new RotationComponent()
            {
                Rotation = rotation is not null ? (Rotation)rotation : new(45, -45, 0)
            });

            Components.Add(new TransformComponent()
            {
                Transform = widgetTransform is not null ? (Transform)widgetTransform : new(new(0, 1, 0), Rotation.Zero, Vector3.One)
            });

            Components.Add(new ColorComponent()
            {
                Color = color is not null ? (Vector4)color : new(1, 1, 1, 1)
            });

            var material = new PBRMaterial()
            {
                Albedo = Components.Get<ColorComponent>()!.Color3,
                Metallic = 0.0f,
                Roughness = 0.0f,
                AmbientOcclusion = 1.0f
            };

            var renderableMeshComponent = new MeshRenderComponent(mesh, shader, texture: null, material);
            Components.Add(renderableMeshComponent);

            FollowCamera = followCamera;
        }
    }
}