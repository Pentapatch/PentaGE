using PentaGE.Core.Components;
using PentaGE.Core.Rendering;
using System.Numerics;

namespace Sandbox.Components
{
    public sealed class MaterialModulator : Component
    {
        private float _totalElapsedTime = 0f;

        public override bool CanHaveMultiple => true;

        public MaterialModulator() { }

        public MaterialModulator(bool albedo, bool specularStrength)
        {
            Albedo = albedo;
            SpecularStrength = specularStrength;
        }

        public bool Albedo { get; set; } = true;

        public Vector3 AlbedoModulators { get; set; } = new(1f, 0.75f, 0.25f);

        public bool SpecularStrength { get; set; } = false;

        public float SpecularStrengthModulator { get; set; } = 0f;

        public override object Clone() =>
            new MaterialModulator()
            {
                Albedo = Albedo,
                AlbedoModulators = AlbedoModulators,
                SpecularStrength = SpecularStrength,
                SpecularStrengthModulator = SpecularStrengthModulator,
                Enabled = Enabled,
            };

        public override void Update(float deltaTime)
        {
            _totalElapsedTime += deltaTime;

            var meshRendererComponent = Entity!.Components.Get<MeshRenderComponent>();
            if (meshRendererComponent is null) return;

            var material = meshRendererComponent.Material;

            var sin = Sin();
            var cos = Cos();

            if (Albedo)
            {
                var hue = sin * AlbedoModulators.X;
                var saturation = cos * AlbedoModulators.Y;
                var lightness = sin * AlbedoModulators.Z;

                material.Albedo = ColorFromHSL(1 - hue, 1 - saturation, 1 - lightness);
            }

            if (SpecularStrength)
            {
                material.SpecularStrength = sin * SpecularStrengthModulator;
            }
        }

        private float Sin() => (MathF.Sin(_totalElapsedTime) * 0.5f + 0.5f);  // 0 - 1

        private float Cos() => (MathF.Cos(_totalElapsedTime) * 0.5f + 0.5f);  // 0 - 1

        public static Vector3 ColorFromHSL(float hue, float saturation, float lightness)
        {
            if (saturation == 0f)
            {
                return new Vector3(lightness, lightness, lightness);
            }
            else
            {
                float q = lightness < 0.5f ? lightness * (1 + saturation) : lightness + saturation - lightness * saturation;
                float p = 2 * lightness - q;
                float[] rgb = new float[3];
                rgb[0] = hue + 1f / 3f;
                rgb[1] = hue;
                rgb[2] = hue - 1f / 3f;
                for (int i = 0; i < 3; i++)
                {
                    if (rgb[i] < 0f) rgb[i]++;
                    if (rgb[i] > 1f) rgb[i]--;
                    if (6f * rgb[i] < 1f)
                    {
                        rgb[i] = p + ((q - p) * 6f * rgb[i]);
                    }
                    else if (2f * rgb[i] < 1f)
                    {
                        rgb[i] = q;
                    }
                    else if (3f * rgb[i] < 2f)
                    {
                        rgb[i] = p + ((q - p) * 6f * ((2f / 3f) - rgb[i]));
                    }
                    else
                    {
                        rgb[i] = p;
                    }
                }
                return new Vector3(rgb[0], rgb[1], rgb[2]);
            }
        }
    }
}