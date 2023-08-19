using System.Numerics;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents a PBR (Physically-Based Rendering) material with properties for rendering realistic surfaces.
    /// </summary>
    public sealed class PBRMaterial : ICloneable
    {
        private Vector3 _albedo;
        private float _roughness;
        private float _metallic;
        private float _ambientOcclusion;
        private float _specularStrength;
        private float _opacity;

        /// <summary>
        /// Gets or sets the base color (albedo) of the material.
        /// </summary>
        /// <remarks>
        /// The albedo property defines the base color of the material, representing the surface's intrinsic color.
        /// This property is typically used to control the color of the material without any lighting effects.
        /// For non-metallic materials, the albedo color is also used to simulate diffuse reflection.
        /// The RGB values of the albedo should be within the range [0, 1], where (0, 0, 0) represents black and (1, 1, 1) represents white.
        /// </remarks>
        public Vector3 Albedo
        {
            get => _albedo;
            set => _albedo = Vector3.Clamp(value, Vector3.Zero, Vector3.One);
        }

        /// <summary>
        /// Gets or sets the roughness of the material. A value of 0 indicates a smooth surface, while 1 indicates a rough surface.
        /// </summary>
        /// <remarks>
        /// The roughness property controls the microsurface roughness of the material's surface, affecting how light is scattered
        /// or diffused across the surface.
        /// Lower roughness values result in more focused and sharp reflections, typically seen on smooth surfaces like glass or metal.
        /// Higher roughness values produce diffuse and blurred reflections, typical of rough surfaces like wood or concrete.
        /// The roughness should be within the range [0, 1], where 0 represents a perfectly smooth surface and 1 represents a completely rough surface.
        /// </remarks>
        public float Roughness
        {
            get => _roughness;
            set => _roughness = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets or sets the metalness of the material. A value of 0 indicates a dielectric (non-metallic) material, while 1 indicates a fully metallic material.
        /// </summary>
        /// <remarks>
        /// The metalness property defines whether the material behaves as a metal or a dielectric (non-metallic) material.
        /// For non-metallic materials, the metalness should be 0, and for fully metallic materials, the metalness should be 1.
        /// Metalness affects the behavior of specular reflections, with metals having sharp and bright specular highlights
        /// while dielectrics have less reflective and diffuse surfaces.
        /// </remarks>
        public float Metallic
        {
            get => _metallic;
            set => _metallic = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets or sets the ambient occlusion factor of the material. This property represents the amount of ambient light occlusion in the material, typically used for darkening crevices and corners.
        /// </summary>
        /// <remarks>
        /// The ambient occlusion property controls how much ambient light is occluded or blocked from reaching certain areas on the surface.
        /// This effect simulates the shadowing and darkening of crevices, corners, and intersections due to ambient light scattering.
        /// A value of 0 means no ambient occlusion, resulting in uniform lighting across the surface, while a value of 1 means full ambient occlusion,
        /// producing stronger shadowing and darker areas in crevices and corners.
        /// </remarks>
        public float AmbientOcclusion
        {
            get => _ambientOcclusion;
            set => _ambientOcclusion = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets or sets the specular strength of the material.
        /// </summary>
        /// <remarks>
        /// The specular strength controls the intensity of the specular highlights on the material's surface.
        /// A higher value results in stronger and more prominent highlights, while a lower value gives a duller and less reflective appearance.
        /// The value should be between 0 and 1, where 0 means no specular highlights (completely diffuse) and 1 means fully reflective with strong specular highlights.
        /// </remarks>
        public float SpecularStrength
        {
            get => _specularStrength;
            set => _specularStrength = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Gets or sets the opacity of the material. A value of 0 indicates a fully transparent material, while 1 indicates a fully opaque material.
        /// </summary>
        public float Opacity
        {
            get => _opacity;
            set => _opacity = Math.Clamp(value, 0, 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PBRMaterial"/> class with default property values.
        /// </summary>
        public PBRMaterial()
        {
            Albedo = Vector3.One;
            Roughness = 0.5f;
            Metallic = 0.5f;
            AmbientOcclusion = 1.0f;
            SpecularStrength = 0.5f;
            Opacity = 1.0f;
        }

        /// <inheritdoc />
        public object Clone() => MemberwiseClone();
    }
}