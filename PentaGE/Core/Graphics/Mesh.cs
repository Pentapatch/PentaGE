using PentaGE.Common;
using System.Numerics;

namespace PentaGE.Core.Graphics
{
    /// <summary>
    /// Represents a 3D mesh consisting of vertices and optional indices.
    /// </summary>
    public sealed class Mesh
    {
        /// <summary>
        /// Gets or sets the list of vertices composing the mesh.
        /// </summary>
        internal List<Vertex> Vertices { get; set; }

        /// <summary>
        /// Gets or sets the list of indices defining the vertex connectivity (optional).
        /// </summary>
        internal List<uint>? Indices { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh"/> class with the specified vertices and optional indices.
        /// </summary>
        /// <param name="vertices">The list of vertices composing the mesh.</param>
        /// <param name="indices">The list of indices defining the vertex connectivity (optional).</param>
        internal Mesh(List<Vertex> vertices, List<uint>? indices = null)
        {
            Vertices = vertices;
            Indices = indices;
        }


        /// <summary>
        /// Offsets all vertex positions of the mesh by specified x, y, and z values.
        /// </summary>
        /// <param name="x">The offset along the x-axis.</param>
        /// <param name="y">The offset along the y-axis.</param>
        /// <param name="z">The offset along the z-axis.</param>
        public void Offset(float x, float y, float z) =>
            Offset(new Vector3(x, y, z));

        /// <summary>
        /// Offsets all vertex positions of the mesh by a specified vector.
        /// </summary>
        /// <param name="offset">The vector by which to offset the vertices.</param>
        public void Offset(Vector3 offset)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vertices[i].Coordinates + offset,
                    Vertices[i].Normal,
                    Vertices[i].TextureCoordinates);
            }
        }

        /// <summary>
        /// Scales the mesh uniformly by a specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        public void Scale(float scale) =>
            Scale(new Vector3(scale));

        /// <summary>
        /// Scales the mesh non-uniformly along the x, y, and z axes.
        /// </summary>
        /// <param name="x">The scale factor along the x-axis.</param>
        /// <param name="y">The scale factor along the y-axis.</param>
        /// <param name="z">The scale factor along the z-axis.</param>
        public void Scale(float x, float y, float z) =>
            Scale(new Vector3(x, y, z));

        /// <summary>
        /// Scales the mesh uniformly by a specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        public void Scale(Vector3 scale)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vertices[i].Coordinates * scale,
                    Vertices[i].Normal,
                    Vertices[i].TextureCoordinates);
            }
        }

        /// <summary>
        /// Rotates the mesh by a specified rotation.
        /// </summary>
        /// <param name="rotation">The rotation to apply.</param>
        public void Rotate(Rotation rotation)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vector3.Transform(
                        Vertices[i].Coordinates,
                        Matrix4x4.CreateFromYawPitchRoll(rotation.Yaw, rotation.Pitch, rotation.Roll)),
                    Vector3.Transform(
                        Vertices[i].Normal,
                        Matrix4x4.CreateFromYawPitchRoll(rotation.Yaw, rotation.Pitch, rotation.Roll)),
                    Vertices[i].TextureCoordinates);
            }
        }

        /// <summary>
        /// Rotates the mesh by the specified yaw, pitch, and roll angles.
        /// </summary>
        /// <param name="yaw">The yaw angle in degrees.</param>
        /// <param name="pitch">The pitch angle in degrees.</param>
        /// <param name="roll">The roll angle in degrees.</param>
        public void Rotate(float yaw, float pitch, float roll) =>
            Rotate(new Rotation(yaw, pitch, roll));

        /// <summary>
        /// Rotates the mesh by a specified angle around a specified axis.
        /// </summary>
        /// <param name="angle">The angle of rotation in radians.</param>
        /// <param name="axis">The axis of rotation.</param>
        public void Rotate(float angle, Vector3 axis)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vector3.Transform(
                        Vertices[i].Coordinates,
                        Matrix4x4.CreateFromAxisAngle(axis, angle)),
                    Vector3.Transform(
                        Vertices[i].Normal,
                        Matrix4x4.CreateFromAxisAngle(axis, angle)),
                    Vertices[i].TextureCoordinates);
            }
        }

        /// <summary>
        /// Rotates the mesh by a specified angle around a specified axis.
        /// </summary>
        /// <param name="angle">The angle of rotation in radians.</param>
        /// <param name="x">The x-coordinate of the rotation axis.</param>
        /// <param name="y">The y-coordinate of the rotation axis.</param>
        /// <param name="z">The z-coordinate of the rotation axis.</param>
        public void Rotate(float angle, float x, float y, float z) =>
            Rotate(angle, new Vector3(x, y, z));

        public void TileTexture(float x, float y)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vertices[i].Coordinates,
                    Vertices[i].Normal,
                    new Vector2(Vertices[i].TextureCoordinates.X * x, Vertices[i].TextureCoordinates.Y * y));
            }
        }
    }
}