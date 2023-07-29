using PentaGE.Common;
using Serilog;
using System.Numerics;
using System.Text;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents a shader program used for rendering graphics on the GPU.
    /// </summary>
    public class Shader : IDisposable
    {
        private string _vertexSourceCode = string.Empty;
        private string _fragmentSourceCode = string.Empty;
        private readonly string _filePath = string.Empty;

        public uint ProgramId { get; private set; } = 0u;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class with vertex and fragment shader source code.
        /// </summary>
        /// <param name="vertexSourceCode">The source code of the vertex shader written in GLSL (OpenGL Shading Language).</param>
        /// <param name="fragmentSourceCode">The source code of the fragment shader written in GLSL (OpenGL Shading Language).</param>
        public Shader(string vertexSourceCode, string fragmentSourceCode)
        {
            _vertexSourceCode = vertexSourceCode;
            _fragmentSourceCode = fragmentSourceCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class by loading shader source code from a file.
        /// </summary>
        /// <param name="filePath">The path to the file containing the shader source code.</param>
        /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
        public Shader(string filePath)
        {
            _filePath = filePath;
            ParseShaderFile();
        }

        /// <summary>
        /// Loads and compiles the vertex and fragment shaders to create the shader program.
        /// </summary>
        /// <returns><c>true</c> if the shader program was loaded and compiled successfully; otherwise, <c>false</c>.</returns>
        public bool Load()
        {
            if (ProgramId != 0)
            {
                Log.Error("Attempted to load shader that is already loaded.");
                return false;
            }

            uint vertexShader, fragmentShader;
            bool hasError = false;

            // Compile and create the vertex and fragment shaders
            vertexShader = CompileShader(GL_VERTEX_SHADER, _vertexSourceCode, ref hasError);
            fragmentShader = CompileShader(GL_FRAGMENT_SHADER, _fragmentSourceCode, ref hasError);

            // Create and link the shader program with the vertex and fragment shaders
            LinkShaderProgram(vertexShader, fragmentShader, ref hasError);

            // Detach and delete the shaders after they have been linked into the program
            // This is safe because the shader program has its own copy of the shader source code
            // The linked program no longer requires the individual shaders once they are linked
            glDetachShader(ProgramId, vertexShader);
            glDetachShader(ProgramId, fragmentShader);
            glDeleteShader(vertexShader);
            glDeleteShader(fragmentShader);

            return !hasError;
        }

        /// <summary>
        /// Reloads the shader program by parsing the shader source code from the file specified in the constructor,
        /// if it was initiated using the file path constructor. If the shader program was already loaded, it will be
        /// disposed of before reloading.
        /// </summary>
        /// <returns><c>true</c> if the shader program was reloaded successfully; otherwise, <c>false</c>.</returns>
        public bool Reload()
        {
            if (_filePath == string.Empty) return false;

            if (ProgramId != 0)
            {
                Dispose();
            }

            ParseShaderFile();

            return Load();
        }

        /// <summary>
        /// Reloads the shader program with new vertex and fragment shader source code.
        /// If the shader program was already loaded, it will be disposed of before reloading.
        /// </summary>
        /// <param name="vertexSourceCode">The new source code for the vertex shader.</param>
        /// <param name="fragmentSourceCode">The new source code for the fragment shader.</param>
        /// <returns><c>true</c> if the shader program was reloaded successfully; otherwise, <c>false</c>.</returns>
        public bool Reload(string vertexSourceCode, string fragmentSourceCode)
        {
            _vertexSourceCode = vertexSourceCode;
            _fragmentSourceCode = fragmentSourceCode;

            if (ProgramId != 0)
            {
                Dispose();
            }

            ParseShaderFile();

            return Load();
        }

        /// <summary>
        /// Uses the shader program for rendering. If the shader program is not initialized (not loaded), this method will return false.
        /// </summary>
        /// <returns><c>true</c> if the shader program was successfully used; otherwise, <c>false</c>.</returns>
        public bool Use()
        {
            if (ProgramId == 0)
            {
                Log.Error("Attempting to use a program that is not initialized.");
                return false;
            }

            glUseProgram(ProgramId);
            return true;
        }

        /// <summary>
        /// Releases the shader program and frees any associated resources. After calling this method, the shader program will be uninitialized.
        /// </summary>
        public void Dispose()
        {
            if (ProgramId != 0u)
            {
                glDeleteProgram(ProgramId);
                ProgramId = 0u;
            }
        }

        /// <summary>
        /// Sets an integer uniform in the shader program.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The integer value to set.</param>
        public void SetInt(string name, int value)
        {
            int location = glGetUniformLocation(ProgramId, name);
            glUniform1i(location, value);
        }

        /// <summary>
        /// Sets a floating-point uniform in the shader program.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The floating-point value to set.</param>
        public void SetFloat(string name, float value)
        {
            int location = glGetUniformLocation(ProgramId, name);
            glUniform1f(location, value);
        }

        /// <summary>
        /// Sets a 2D vector (Vec2) uniform in the shader program.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 2D vector (Vec2) value to set.</param>
        public void SetVec2(string name, Vector2 value)
        {
            int location = glGetUniformLocation(ProgramId, name);
            glUniform2f(location, value.X, value.Y);
        }

        /// <summary>
        /// Sets a 3D vector (Vec3) uniform in the shader program.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 3D vector (Vec3) value to set.</param>
        public void SetVec3(string name, Vector3 value)
        {
            int location = glGetUniformLocation(ProgramId, name);
            glUniform3f(location, value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Sets a 4D vector (Vec4) uniform in the shader program.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 4D vector (Vec4) value to set.</param>
        public void SetVec4(string name, Vector4 value)
        {
            int location = glGetUniformLocation(ProgramId, name);
            glUniform4f(location, value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Sets a 3x2 matrix uniform in the shader program.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 3x2 matrix value to set.</param>
        public void SetMatrix3x2(string name, Matrix3x2 value)
        {
            int location = glGetUniformLocation(ProgramId, name);
            glUniformMatrix3x2fv(location, 1, false, value.ToArray());
        }

        /// <summary>
        /// Sets a 4x4 matrix uniform in the shader program.
        /// </summary>
        /// <param name="name">The name of the uniform variable.</param>
        /// <param name="value">The 4x4 matrix value to set.</param>
        public void SetMatrix4(string name, Matrix4x4 value)
        {
            int location = glGetUniformLocation(ProgramId, name);
            glUniformMatrix4fv(location, 1, false, value.ToArray());
        }

        public void SetVec2Array(string name, float[] values)
        {
            int location = glGetUniformLocation(ProgramId, name);
            int length = values.Length / 2; // Divide by 2 since each vector has 2 components (X, Y)

            // Set the uniform data using glUniform3fv
            glUniform2fv(location, length, values);
        }

        public void SetVec3Array(string name, float[] values)
        {
            int location = glGetUniformLocation(ProgramId, name);
            int length = values.Length / 3; // Divide by 3 since each vector has 3 components (RGB/XYZ)

            // Set the uniform data using glUniform3fv
            glUniform3fv(location, length, values);
        }

        /// <summary>
        /// Compiles a shader of the specified type using the provided source code.
        /// </summary>
        /// <param name="type">The type of the shader (e.g., GL_VERTEX_SHADER or GL_FRAGMENT_SHADER).</param>
        /// <param name="sourceCode">The source code of the shader.</param>
        /// <param name="error">A reference to a boolean flag that will be set to true if there is an error in the compilation process.</param>
        /// <returns>The ID of the compiled shader if successful; otherwise, an invalid shader ID.</returns>
        private static uint CompileShader(int type, string sourceCode, ref bool error)
        {
            uint shader = glCreateShader(type);
            glShaderSource(shader, sourceCode);
            glCompileShader(shader);

            // Check for errors compiling the shader
            int[] status = glGetShaderiv(shader, GL_COMPILE_STATUS, 1);
            if (status[0] == 0)
            {
                error = true;
                string errorMessage = glGetShaderInfoLog(shader);
                Log.Error($"Error compiling {ShaderTypeToString(type)} shader: {errorMessage}");
            }

            return shader;
        }

        /// <summary>
        /// Links the shader program with the specified vertex and fragment shaders.
        /// </summary>
        /// <param name="vertexShader">The ID of the compiled vertex shader.</param>
        /// <param name="fragmentShader">The ID of the compiled fragment shader.</param>
        /// <param name="error">A reference to a boolean flag that will be set to true if there is an error in the linking process.</param>
        private void LinkShaderProgram(uint vertexShader, uint fragmentShader, ref bool error)
        {
            // Create the shader program and attach the shaders to it
            ProgramId = glCreateProgram();
            glAttachShader(ProgramId, vertexShader);
            glAttachShader(ProgramId, fragmentShader);

            // Link the shader program, creating an executable that runs on the GPU
            glLinkProgram(ProgramId);

            // Check for any errors
            int[] status = glGetProgramiv(ProgramId, GL_LINK_STATUS, 1);
            if (status[0] == 0)
            {
                error = true;
                string errorMessage = glGetProgramInfoLog(ProgramId);
                Log.Error($"Shader program linking error: {errorMessage}");
            }
        }

        /// <summary>
        /// Parses the shader file specified in the constructor to extract vertex and fragment shader source code.
        /// </summary>
        /// <remarks>
        /// This method reads the shader file line by line and separates the source code based on the "#shader vertex" and "#shader fragment" tags.
        /// The extracted source code is then stored in the respective variables for later shader compilation.
        /// </remarks>
        private void ParseShaderFile()
        {
            if (!File.Exists(_filePath)) throw new FileNotFoundException();

            // Create string builders to store the vertex and fragment shader source code
            StringBuilder vertexShaderSource = new();
            StringBuilder fragmentShaderSource = new();

            // Set the parsing mode
            ShaderType parsingMode = ShaderType.Unknown;

            // Iterate through the lines and extract the shader source code
            foreach (string line in File.ReadAllLines(_filePath))
            {
                if (line.StartsWith("#shader vertex"))
                {
                    parsingMode = ShaderType.Vertex;
                }
                else if (line.StartsWith("#shader fragment"))
                {
                    parsingMode = ShaderType.Fragment;
                }
                else if (parsingMode == ShaderType.Vertex)
                {
                    vertexShaderSource.AppendLine(line);
                }
                else if (parsingMode == ShaderType.Fragment)
                {
                    fragmentShaderSource.AppendLine(line);
                }
            }

            // Set the source code values
            _vertexSourceCode = vertexShaderSource.ToString();
            _fragmentSourceCode = fragmentShaderSource.ToString();
        }

        /// <summary>
        /// Converts the specified shader type to its string representation.
        /// </summary>
        /// <param name="type">The shader type (e.g., GL_VERTEX_SHADER or GL_FRAGMENT_SHADER).</param>
        /// <returns>The string representation of the shader type.</returns>
        private static string ShaderTypeToString(int type) => type switch
        {
            GL_VERTEX_SHADER => "vertex",
            GL_FRAGMENT_SHADER => "fragment",
            _ => throw new NotSupportedException()
        };

    }
}