using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Rendering;
using System.Numerics;

namespace Sandbox.Components
{
    /// <summary>
    /// Represents a component that modulates material properties over time based on HSL values.
    /// </summary>
    public sealed class MaterialModulator : Component
    {
        private float _totalElapsedTime = 0f;

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <summary>
        /// Gets or sets whether albedo modulation is enabled.
        /// </summary>
        public bool AlbedoEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the factors for modulating albedo properties.
        /// </summary>
        /// <remarks>
        /// The X, Y, and Z components of the <see cref="AlbedoModulatorFactors"/> will affect the red (R), green (G), and blue (B) channels in the albedo color respecively.
        /// </remarks>
        public Vector3 AlbedoModulatorFactors { get; set; }

        /// <summary>
        /// Gets or sets whether specular strength modulation is enabled.
        /// </summary>
        public bool SpecularStrengthEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the factor for modulating specular strength.
        /// </summary>
        public float SpecularStrengthModulatorFactor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialModulator"/> class.
        /// </summary>
        public MaterialModulator() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialModulator"/> class with specified properties.
        /// </summary>
        /// <param name="albedo">Whether albedo modulation is enabled.</param>
        /// <param name="specularStrength">Whether specular strength modulation is enabled.</param>
        public MaterialModulator(bool albedo, bool specularStrength)
        {
            AlbedoEnabled = albedo;
            SpecularStrengthEnabled = specularStrength;
            AlbedoModulatorFactors = new(1f, 0.75f, 0.25f);
            SpecularStrengthModulatorFactor = 0f;
        }

        /// <inheritdoc />
        public override object Clone() =>
            new MaterialModulator()
            {
                AlbedoEnabled = AlbedoEnabled,
                AlbedoModulatorFactors = AlbedoModulatorFactors,
                SpecularStrengthEnabled = SpecularStrengthEnabled,
                SpecularStrengthModulatorFactor = SpecularStrengthModulatorFactor,
                Enabled = Enabled,
            };

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            _totalElapsedTime += deltaTime;

            var meshRendererComponent = Entity!.Components.Get<MeshRenderComponent>();
            if (meshRendererComponent is null) return;

            var material = meshRendererComponent.Material;

            var sin = Sin();
            var cos = Cos();

            UpdateAlbedo(material, sin, cos);
            UpdateSpecularStrength(material, sin);
        }

        /// <summary>
        /// Updates the albedo properties of the <paramref name="material"/> based on modulator factors and time-dependent values.
        /// </summary>
        /// <param name="material">The material to update.</param>
        /// <param name="sin">The sine value used for modulation calculations.</param>
        /// <param name="cos">The cosine value used for modulation calculations.</param>
        private void UpdateAlbedo(PBRMaterial material, float sin, float cos)
        {
            if (!AlbedoEnabled) return;

            var hue = sin * AlbedoModulatorFactors.X;
            var saturation = cos * AlbedoModulatorFactors.Y;
            var lightness = sin * AlbedoModulatorFactors.Z;

            material.Albedo = Colors.Vector3FromHSL(1 - hue, 1 - saturation, 1 - lightness);
        }

        /// <summary>
        /// Updates the specular strength property of the <paramref name="material"/> based on a modulator factor and time-dependent value.
        /// </summary>
        /// <param name="material">The material to update.</param>
        /// <param name="sin">The sine value used for modulation calculations.</param>
        private void UpdateSpecularStrength(PBRMaterial material, float sin)
        {
            if (!SpecularStrengthEnabled) return;

            material.SpecularStrength = sin * SpecularStrengthModulatorFactor;
        }

        /// <summary>
        /// Calculates the sine value of the total elapsed time scaled to the range [0, 1].
        /// </summary>
        /// <returns>The scaled sine value.</returns>
        private float Sin() => (MathF.Sin(_totalElapsedTime) * 0.5f + 0.5f);

        /// <summary>
        /// Calculates the cosine value of the total elapsed time scaled to the range [0, 1].
        /// </summary>
        /// <returns>The scaled cosine value.</returns>
        private float Cos() => (MathF.Cos(_totalElapsedTime) * 0.5f + 0.5f);
    }
}