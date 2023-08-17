using PentaGE.Common;
using PentaGE.Core.Components;

namespace Sandbox.Components
{
    public sealed class ConstantRotator : Component
    {
        private float _totalElapsedTime = 0f;

        public ConstantRotator() { }

        public ConstantRotator(bool yaw, bool pitch, bool roll)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }

        public int Range { get; set; } = 360;

        public float Speed { get; set; } = 1f;

        public bool Yaw { get; set; } = true;

        public bool Pitch { get; set; } = false;

        public bool Roll { get; set; } = false;

        public override bool CanHaveMultiple => true;

        public override void Update(float deltaTime)
        {
            var transformComponent = Entity!.Components.Get<TransformComponent>();
            if (transformComponent is null) return;

            var transform = transformComponent.Transform;

            var angle = MathF.Sin(_totalElapsedTime * Speed) * (Range / 2);
            var yaw = Yaw ? angle : transform.Rotation.Yaw;
            var pitch = Pitch ? angle : transform.Rotation.Pitch;
            var roll = Roll ? angle : transform.Rotation.Roll;

            transform.Rotation = new Rotation(yaw, pitch, roll);

            transformComponent.Transform = transform;

            _totalElapsedTime += deltaTime;
        }

        /// <inheritdoc />
        public override object Clone() =>
            new ConstantRotator
            {
                Range = Range,
                Speed = Speed,
                Yaw = Yaw,
                Pitch = Pitch,
                Roll = Roll,
                Enabled = Enabled,
            };
    }
}