using PentaGE.Common;
using PentaGE.Core.Components;

namespace Sandbox.Components
{
    /// <summary>
    /// Represents a component responsible for continuously rotating an entity based on specified rotation axes.
    /// </summary>
    public sealed class ConstantRotator : Component
    {
        private float _totalElapsedTime = 0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantRotator"/> class.
        /// </summary>
        public ConstantRotator()
        {
            Range = 360;
            Speed = 1f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantRotator"/> class with specified rotation axis flags.
        /// </summary>
        /// <param name="yaw">Set whether yaw rotation is enabled.</param>
        /// <param name="pitch">Set whether pitch rotation is enabled.</param>
        /// <param name="roll">Set whether roll rotation is enabled.</param>
        public ConstantRotator(bool yaw, bool pitch, bool roll) : this()
        {
            YawEnabled = yaw;
            PitchEnabled = pitch;
            RollEnabled = roll;
        }

        /// <summary>
        /// Gets or sets the rotation range in degrees.
        /// </summary>
        public int Range { get; set; }

        /// <summary>
        /// Gets or sets the rotation speed factor.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether yaw rotation is enabled.
        /// </summary>
        public bool YawEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether pitch rotation is enabled.
        /// </summary>
        public bool PitchEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether roll rotation is enabled.
        /// </summary>
        public bool RollEnabled { get; set; } = false;

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            var transformComponent = Entity!.Components.Get<TransformComponent>();
            if (transformComponent is null) return;

            var transform = transformComponent.Transform;

            var angle = MathF.Sin(_totalElapsedTime * Speed) * (Range / 2);
            var yaw = YawEnabled ? angle : transform.Rotation.Yaw;
            var pitch = PitchEnabled ? angle : transform.Rotation.Pitch;
            var roll = RollEnabled ? angle : transform.Rotation.Roll;

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
                YawEnabled = YawEnabled,
                PitchEnabled = PitchEnabled,
                RollEnabled = RollEnabled,
                Enabled = Enabled,
            };
    }
}