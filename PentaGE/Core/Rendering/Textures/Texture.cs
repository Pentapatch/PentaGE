using Serilog;
using StbImageSharp;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    public sealed class Texture : IDisposable
    {
        private readonly uint _id;
        private readonly int _type;

        public unsafe Texture(string filePath, int type, int slot, int format, int pixelType)
        {
            using var stream = File.OpenRead(filePath);
            var imageResult = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            if (imageResult is null)
            {
                Log.Error($"Failed to load texture from file: '{filePath}'");
                throw new Exception($"Failed to load texture from file: '{filePath}'");
            }

            var width = imageResult.Width;
            var height = imageResult.Height;
            var data = imageResult.Data;

            _type = type;
            _id = glGenTexture();

            glActiveTexture(slot);
            glBindTexture(_type, _id);

            if (_type == GL_TEXTURE_2D || _type == GL_TEXTURE_CUBE_MAP)
            {
                // Configures the type of algorithm that is used to make the image smaller or bigger
                glTexParameteri(_type, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_LINEAR);
                glTexParameteri(_type, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

                // Configures the way the texture repeats (if it does at all)
                glTexParameteri(_type, GL_TEXTURE_WRAP_S, GL_REPEAT);
                glTexParameteri(_type, GL_TEXTURE_WRAP_T, GL_REPEAT);
            }

            // Extra lines in case you choose to use GL_CLAMP_TO_BORDER
            // float flatColor[] = {1.0f, 1.0f, 1.0f, 1.0f};
            // glTexParameterfv(GL_TEXTURE_2D, GL_TEXTURE_BORDER_COLOR, flatColor);

            // Assigns the image to the OpenGL Texture object
            fixed (byte* ptr = data)
            {
                glTexImage2D(_type, 0, GL_RGBA, width, height, 0, format, pixelType, ptr);
            }

            // Generates MipMaps
            glGenerateMipmap(_type);

            Unbind();
        }

        public static void SetTextureSlot(Shader shader, string uniformName, int slot) => 
            shader.SetUniform(uniformName, slot);

        public void Bind() => 
            glBindTexture(_type, _id);

        public void Unbind() => 
            glBindTexture(_type, 0);

        public void Dispose() => 
            glDeleteTexture(_id);
    }
}