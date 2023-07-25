using PentaGE.Maths;
using System.Numerics;

namespace PentaGE.Common
{
    /// <summary>
    /// Represents a transformation in 3D space, defined by position, rotation, and scale.
    /// </summary>
    public struct Transform
    {
        /// <summary>
        /// Gets or sets the position of the transform in 3D space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the transform.
        /// </summary>
        public Rotation Rotation { get; set; }

        /// <summary>
        /// Gets or sets the scale of the transform in 3D space.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> struct with the specified position, rotation, and scale.
        /// </summary>
        /// <param name="position">The position of the transform in 3D space.</param>
        /// <param name="rotation">The rotation of the transform.</param>
        /// <param name="scale">The scale of the transform in 3D space.</param>
        public Transform(Vector3 position, Rotation rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform"/> struct with default values (position: [0, 0, 0], rotation: [0, 0, 0], scale: [1, 1, 1]).
        /// </summary>
        public Transform()
        {
            Position = Vector3.Zero;
            Rotation = Rotation.Zero;
            Scale = Vector3.One;
        }

        #region Operator overloads

        /// <summary>
        /// Adds two <see cref="Transform"/> instances component-wise.
        /// </summary>
        /// <param name="transformA">The first <see cref="Transform"/> to add.</param>
        /// <param name="transformB">The second <see cref="Transform"/> to add.</param>
        /// <returns>A new <see cref="Transform"/> representing the sum of the two input transforms.</returns>
        public static Transform operator +(Transform transformA, Transform transformB) =>
            new(transformA.Position + transformB.Position,
                transformA.Rotation + transformB.Rotation,
                transformA.Scale + transformB.Scale);

        /// <summary>
        /// Subtracts one <see cref="Transform"/> from another component-wise.
        /// </summary>
        /// <param name="transformA">The <see cref="Transform"/> to subtract from (minuend).</param>
        /// <param name="transformB">The <see cref="Transform"/> to subtract (subtrahend).</param>
        /// <returns>A new <see cref="Transform"/> representing the difference between the two input transforms.</returns>
        public static Transform operator -(Transform transformA, Transform transformB) =>
            new(transformA.Position - transformB.Position,
                transformA.Rotation - transformB.Rotation,
                transformA.Scale - transformB.Scale);

        /// <summary>
        /// Negates the transform by negating the position, rotation, and scale components.
        /// </summary>
        /// <param name="transform">The transform to negate.</param>
        /// <returns>A new <see cref="Transform"/> instance representing the negated transform.</returns>
        public static Transform operator -(Transform transform) =>
            new(-transform.Position, -transform.Rotation, -transform.Scale);

        /// <summary>
        /// Multiplies a <see cref="Transform"/> by a scalar value component-wise.
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> to multiply.</param>
        /// <param name="scalar">The scalar value to multiply the transform by.</param>
        /// <returns>A new <see cref="Transform"/> representing the result of the multiplication.</returns>
        public static Transform operator *(Transform transform, float scalar) =>
            new(transform.Position * scalar,
                transform.Rotation * scalar,
                transform.Scale * scalar);

        /// <summary>
        /// Multiplies a scalar value by a <see cref="Transform"/> component-wise.
        /// </summary>
        /// <param name="scalar">The scalar value to multiply.</param>
        /// <param name="transform">The <see cref="Transform"/> to multiply.</param>
        /// <returns>A new <see cref="Transform"/> representing the result of the multiplication.</returns>
        public static Transform operator *(float scalar, Transform transform) =>
            transform * scalar;

        /// <summary>
        /// Divides a <see cref="Transform"/> by a scalar value component-wise.
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> to divide.</param>
        /// <param name="scalar">The scalar value to divide the transform by.</param>
        /// <returns>A new <see cref="Transform"/> representing the result of the division.</returns>
        public static Transform operator /(Transform transform, float scalar) =>
            new(transform.Position / scalar,
                transform.Rotation / scalar,
                transform.Scale / scalar);

        #endregion

        #region Math

        /// <summary>
        /// Linearly interpolates between this transform and another transform using the specified interpolation factor.
        /// </summary>
        /// <param name="transform">The target transform to interpolate towards.</param>
        /// <param name="t">The interpolation factor in the range (0 - 1).</param>
        /// <returns>A new <see cref="Transform"/> instance representing the interpolated transform.</returns>
        public Transform Lerp(Transform transform, float t) =>
            Lerp(this, transform, t);

        /// <summary>
        /// Linearly interpolates between two <see cref="Transform"/> instances using the specified interpolation factor.
        /// </summary>
        /// <param name="transformA">The starting transform.</param>
        /// <param name="transformB">The target transform to interpolate towards.</param>
        /// <param name="t">The interpolation factor in the range (0 - 1).</param>
        /// <returns>A new <see cref="Transform"/> instance representing the interpolated transform.</returns>
        public static Transform Lerp(Transform transformA, Transform transformB, float t)
        {
            // Interpolate each component individually
            Vector3 positionInterpolated = Vector3.Lerp(transformA.Position, transformB.Position, t);
            Rotation rotationInterpolated = Rotation.Lerp(transformA.Rotation, transformB.Rotation, t);
            Vector3 scaleInterpolated = Vector3.Lerp(transformA.Scale, transformB.Scale, t);

            return new Transform(positionInterpolated, rotationInterpolated, scaleInterpolated);
        }

        /// <summary>
        /// Interpolates between two <see cref="Transform"/> instances using the specified interpolation factor and curve type.
        /// </summary>
        /// <param name="transformA">The starting transform.</param>
        /// <param name="transformB">The target transform to interpolate towards.</param>
        /// <param name="t">The interpolation factor in the range (0 - 1).</param>
        /// <param name="curvePower">The power/exponent to apply to the interpolation factor (for custom curves).</param>
        /// <param name="curveType">The type of interpolation curve to use.</param>
        /// <returns>A new <see cref="Transform"/> instance representing the interpolated transform.</returns>
        public static Transform Interpolate(
            Transform transformA,
            Transform transformB,
            float t,
            float curvePower,
            InterpolationCurve curveType) =>
            Lerp(transformA, transformB, MathHelper.TransformInterpolationFactor(t, curvePower, curveType));

        /// <summary>
        /// Interpolates between this transform and a target transform using the specified interpolation factor and curve type.
        /// </summary>
        /// <param name="transform">The target transform to interpolate towards.</param>
        /// <param name="t">The interpolation factor in the range (0 - 1).</param>
        /// <param name="curvePower">The power/exponent to apply to the interpolation factor (for custom curves).</param>
        /// <param name="curveType">The type of interpolation curve to use.</param>
        /// <returns>A new <see cref="Transform"/> instance representing the interpolated transform.</returns>
        public Transform Interpolate(Transform transform, float t, float curvePower, InterpolationCurve curveType) =>
            Interpolate(this, transform, t, curvePower, curveType);

        /// <summary>
        /// Normalizes this transform by normalizing its position, rotation angles, and scale.
        /// </summary>
        /// <returns>A new <see cref="Transform"/> instance with normalized position, rotation, and scale.</returns>
        public Transform Normalize() =>
            Normalize(this);

        /// <summary>
        /// Normalizes a transform by normalizing its position, rotation angles, and scale.
        /// </summary>
        /// <param name="transform">The transform to normalize.</param>
        /// <returns>A new <see cref="Transform"/> instance with normalized position, rotation, and scale.</returns>
        public static Transform Normalize(Transform transform)
        {
            Vector3 positionNormalized = Vector3.Normalize(transform.Position);
            Rotation rotationNormalized = Rotation.Normalize(transform.Rotation);
            Vector3 scaleNormalized = Vector3.Normalize(transform.Scale);

            return new Transform(positionNormalized, rotationNormalized, scaleNormalized);
        }

        /// <summary>
        /// Inverts this transform by inverting the position, rotation, and scale components.
        /// </summary>
        /// <returns>A new <see cref="Transform"/> instance representing the inverted transform.</returns>
        public Transform Invert() => Invert(this);

        /// <summary>
        /// Inverts a transform by inverting its position, rotation, and scale components.
        /// </summary>
        /// <param name="source">The transform to invert.</param>
        /// <returns>A new <see cref="Transform"/> instance representing the inverted transform.</returns>
        public static Transform Invert(Transform source)
        {
            Vector3 invertedPosition = -source.Position;
            Rotation invertedRotation = -source.Rotation;
            Vector3 invertedScale = -source.Scale;

            return new Transform(invertedPosition, invertedRotation, invertedScale);
        }

        #endregion

    }
}