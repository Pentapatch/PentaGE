using PentaGE.Common;
using System.Numerics;

namespace PentaGE.Core.Graphics
{
    public sealed class Mesh
    {
        internal List<Vertex> Vertices { get; set; }

        internal List<uint>? Indices { get; set; }

        internal Mesh(List<Vertex> vertices, List<uint>? indices = null)
        {
            Vertices = vertices;
            Indices = indices ?? new List<uint>();
        }

        public void Offset(float x, float y, float z) =>
            Offset(new Vector3(x, y, z));

        public void Offset(Vector3 offset)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vertices[i].Coordinates + offset,
                    Vertices[i].Normal,
                    Vertices[i].Color,
                    Vertices[i].TextureCoordinates);
            }
        }

        public void Scale(float scale) => 
            Scale(new Vector3(scale));

        public void Scale(float x, float y, float z) =>
            Scale(new Vector3(x, y, z));

        public void Scale(Vector3 scale)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vertices[i].Coordinates * scale,
                    Vertices[i].Normal,
                    Vertices[i].Color,
                    Vertices[i].TextureCoordinates);
            }
        }

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
                    Vertices[i].Color,
                    Vertices[i].TextureCoordinates);
            }
        }

        public void Rotate(float yaw, float pitch, float roll) =>
            Rotate(new Rotation(yaw, pitch, roll));

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
                    Vertices[i].Color,
                    Vertices[i].TextureCoordinates);
            }
        }

        public void Rotate(float angle, float x, float y, float z) =>
            Rotate(angle, new Vector3(x, y, z));

    }
}