using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents the draw mode used to render the mesh.
    /// </summary>
    public enum DrawMode
    {
        /// <summary>
        /// Each vertex represents a single point.
        /// </summary>
        Points = GL_POINTS,

        /// <summary>
        /// Each pair of vertices represents a line segment.
        /// </summary>
        Lines = GL_LINES,

        /// <summary>
        /// A loop of connected line segments. The last vertex connects to the first.
        /// </summary>
        LineLoop = GL_LINE_LOOP,

        /// <summary>
        /// A strip of connected line segments.
        /// </summary>
        LineStrip = GL_LINE_STRIP,

        /// <summary>
        /// Each set of three vertices defines a separate triangle.
        /// </summary>
        Triangles = GL_TRIANGLES,

        /// <summary>
        /// A strip of triangles, sharing vertices and forming a continuous strip.
        /// </summary>
        TriangleStrip = GL_TRIANGLE_STRIP,

        /// <summary>
        /// A fan of triangles, sharing a central vertex.
        /// </summary>
        TriangleFan = GL_TRIANGLE_FAN,
    }
}