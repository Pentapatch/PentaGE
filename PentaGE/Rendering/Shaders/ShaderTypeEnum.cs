namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents the type of shader used in the <see cref="Shader"/> class.
    /// </summary>
    internal enum ShaderType
    {
        /// <summary>
        /// Indicates an unknown or unspecified shader type.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Indicates a vertex shader, responsible for processing vertex data.
        /// </summary>
        Vertex = 1,

        /// <summary>
        /// Indicates a fragment shader, responsible for processing fragments (pixels) after rasterization.
        /// </summary>
        Fragment = 2,
    }
}