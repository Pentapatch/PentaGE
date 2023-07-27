using GLFW;
using PentaGE.Common;
using PentaGE.Core.Logging;
using Serilog;
using System.Numerics;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// The renderer responsible for handling graphics rendering in the PentaGameEngine.
    /// </summary>
    internal class Renderer
    {
        private readonly PentaGameEngine _engine;
        private uint vao;
        private uint vbo;
        private uint ebo;
        private Shader shader;

        // Set up the rendered test objects transform
        private Transform objectTransform = new()
        {
            Position = new(0, 0, 0),    // in pixels
            Rotation = new(0, 0, 0),    // in degrees
            Scale = new(1, 1, 1),  // in pixels
        };

        private readonly Camera3d testCamera = new()
        {
            Position = new(0, 0, -2),
            FieldOfView = 90,
            Rotation = new(
                0,  // Yaw 
                0,  // Pitch
                0   // Roll
            )
        };

        private readonly float[] cubeVertices = new[]
        {
            // Front face
            -0.5f, -0.5f,  0.5f,    1f, 0f, 0f,  // Vertex 0
             0.5f, -0.5f,  0.5f,    0f, 1f, 0f,  // Vertex 1
             0.5f,  0.5f,  0.5f,    0f, 0f, 1f,  // Vertex 2
            -0.5f,  0.5f,  0.5f,    1f, 0f, 0f,  // Vertex 3
                                    
            // Back face            
            -0.5f, -0.5f, -0.5f,    0f, 1f, 0f,  // Vertex 4
             0.5f, -0.5f, -0.5f,    0f, 0f, 1f,  // Vertex 5
             0.5f,  0.5f, -0.5f,    1f, 0f, 0f,  // Vertex 6
            -0.5f,  0.5f, -0.5f,    0f, 1f, 0f,  // Vertex 7
        };

        // Indices to define the triangles for each face
        private readonly uint[] cubeIndices = new uint[]
        {
            // Front face
            0, 1, 2,
            2, 3, 0,

            // Right face
            1, 5, 6,
            6, 2, 1,

            // Back face
            5, 4, 7,
            7, 6, 5,

            // Left face
            4, 0, 3,
            3, 7, 4,

            // Top face
            3, 2, 6,
            6, 7, 3,

            // Bottom face
            0, 4, 5,
            5, 1, 0,
        };

        /// <summary>
        /// Creates a new instance of the Renderer class.
        /// </summary>
        /// <param name="engine">The PentaGameEngine instance associated with this Renderer.</param>
        internal Renderer(PentaGameEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Initializes GLFW and sets up the necessary context hints for rendering.
        /// </summary>
        /// <returns><c>true</c> if GLFW is successfully initialized; otherwise, <c>false</c>.</returns>
        internal unsafe bool InitializeGLFW()
        {
            if (!Glfw.Init())
            {
                Log.Fatal("Failed to initialize GLFW");
                return false;
            }

            // Set up GLFW versioning
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            // Initialize and create all windows
            if (!_engine.Windows.Initialize())
            {
                Log.Fatal("Failed to create all GLFW windows.");
                return false;
            }

            // Initializing test shader
            using var logger = Log.Logger.BeginPerfLogger("Loading shader");
            try
            {
                shader = new(@"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Shaders\SourceCode\CameraTest.shader");
                shader.Load();
            }
            catch (System.Exception ex)
            {
                Log.Error($"Error loading shader: {ex}");
            }

            #region testing

            vao = glGenVertexArray(); // Vertex array object
            vbo = glGenBuffer(); // Vertex buffer object
            ebo = glGenBuffer(); // Element buffer object

            glBindVertexArray(vao); // Bind the VAO to the current context

            // Bind and copy the vertex data from the managed array to the VBO.
            glBindBuffer(GL_ARRAY_BUFFER, vbo);
            fixed (float* v = &cubeVertices[0])
            {
                // Sets the data for the currently bound buffer
                // GL_STATIC_DRAW means the data will not be changed (much)
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * cubeVertices.Length, v, GL_STATIC_DRAW);
            }

            // Bind and copy the indices to the EBO.
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
            fixed (uint* i = &cubeIndices[0])
            {
                glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(uint) * cubeIndices.Length, i, GL_STATIC_DRAW);
            }

            // Specify how the vertex attributes should be interpreted.
            glVertexAttribPointer(0, 3, GL_FLOAT, false, 6 * sizeof(float), (void*)0); // Positions
            glVertexAttribPointer(1, 3, GL_FLOAT, false, 6 * sizeof(float), (void*)(3 * sizeof(float))); // Colors
            glEnableVertexAttribArray(0); // Enable the vertex attribute at index 0 for positions
            glEnableVertexAttribArray(1); // Enable the vertex attribute at index 1 for colors

            // Unbind the VBO, EBO, and VAO to prevent further changes to them.
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0); // Unbind the EBO
            glBindBuffer(GL_ARRAY_BUFFER, 0);         // Unbind the VBO
            glBindVertexArray(0);                     // Unbind the VAO

            #endregion

            // Add engine events for moving the camera (during debug - remove later)
            _engine.Events.KeyDown += Events_KeyDown;
            _engine.Events.KeyRepeat += Events_KeyDown;

            return true;
        }

        private void Events_KeyDown(object? sender, Events.KeyDownEventArgs e)
        {
            // For moving the camera (during debug - remove later)
            float increment = 0.25f;
            if (e.Key == Key.W)
            {
                testCamera.Position += World.ForwardVector * increment;
                //testCamera.Position += World.UpVector * increment; // For 2D
                Log.Information($"Camera moving forward: {testCamera.Position}");
            }
            else if (e.Key == Key.A)
            {
                testCamera.Position += World.LeftVector * increment;
                Log.Information($"Camera moving left: {testCamera.Position}");
            }
            else if (e.Key == Key.D)
            {
                testCamera.Position += World.RightVector * increment;
                Log.Information($"Camera moving right: {testCamera.Position}");
            }
            else if (e.Key == Key.S)
            {
                testCamera.Position += World.BackwardVector * increment;
                //testCamera.Position += World.DownVector * increment; // For 2D
                Log.Information($"Camera moving backward: {testCamera.Position}");
            }
            else if (e.Key == Key.Q)
            {
                testCamera.Position += World.UpVector * increment;
                Log.Information($"Camera moving up: {testCamera.Position}");
            }
            else if (e.Key == Key.E)
            {
                testCamera.Position += World.DownVector * increment;
                Log.Information($"Camera moving down: {testCamera.Position}");
            }
            else if (e.Key == Key.Left)
            {
                testCamera.Rotation += new Rotation(1, 0, 0) * 5;
                Log.Information($"Camera yaw right: {testCamera.Rotation}");
            }
            else if (e.Key == Key.Right)
            {
                testCamera.Rotation += new Rotation(-1, 0, 0) * 5;
                Log.Information($"Camera yaw left: {testCamera.Rotation}");
            }
            else if (e.Key == Key.Up)
            {
                testCamera.Rotation += new Rotation(0, 1, 0) * 5;
                Log.Information($"Camera yaw right: {testCamera.Rotation}");
            }
            else if (e.Key == Key.Down)
            {
                testCamera.Rotation += new Rotation(0, -1, 0) * 5;
                Log.Information($"Camera yaw left: {testCamera.Rotation}");
            }
            else if (e.Key == Key.Z)
            {
                testCamera.FieldOfView += 5;
                Log.Information($"Camera FoV increasing: {testCamera.FieldOfView}");
            }
            else if (e.Key == Key.X)
            {
                testCamera.FieldOfView -= 5;
                Log.Information($"Camera FoV decreasing: {testCamera.FieldOfView}");
            }
        }

        /// <summary>
        /// Handles graphics rendering.
        /// </summary>
        internal unsafe void Render()
        {
            foreach (var window in _engine.Windows)
            {
                window.RenderingContext.Use();

                glClearColor(0.2f, 0.2f, 0.2f, 1);
                glClear(GL_COLOR_BUFFER_BIT);

                // Use the shader program
                shader.Use();

                // Calculate the view and projection matrices from the camera
                var viewMatrix = testCamera.GetViewMatrix();
                var projectionMatrix = testCamera.GetProjectionMatrix(window.Size.Width, window.Size.Height);
                var modelMatrix = Matrix4x4.CreateScale(objectTransform.Scale)
                    * objectTransform.Rotation.ToMatrix4x4()
                    * Matrix4x4.CreateTranslation(objectTransform.Position);

                // Combine the model, view, and projection matrices to get the final MVP matrix
                var mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;

                // Pass the matrices to the shader (must be done after shader.Use())
                shader.SetMatrix4("mvp", mvpMatrix);

                // Bind the Vertex Array Object (VAO) to use the configuration of vertex attributes stored in it.
                glBindVertexArray(vao); // Bind the VAO to the current context

                // Draw the cube using the indices of the EBO
                glDrawElements(GL_TRIANGLES, cubeIndices);

                // Unbind the VAO to prevent accidental modification.
                glBindVertexArray(0);                     // Unbind the VAO
                glBindBuffer(GL_ARRAY_BUFFER, 0);         // Unbind the VBO (optional, but good practice)
                glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0); // Unbind the EBO (optional, but good practice)
            }
        }

        private static void LogGlErrors()
        {
            while (Glfw.GetError(out string description) != ErrorCode.None)
            {
                Log.Error(description);
            }
        }
    }
}