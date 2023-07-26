using GLFW;
using PentaGE.Common;
using PentaGE.Core.Logging;
using PentaGE.Maths;
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
        private Shader shader;

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

            // Testing TODO: Remove

            // Generate and bind a Vertex Array Object (VAO).
            // VAOs store the configurations of vertex attributes, allowing for easier vertex attribute setup.
            vao = glGenVertexArray();
            uint vbo = glGenBuffer();

            // Generate and bind a Vertex Buffer Object (VBO).
            // VBOs store vertex data, such as positions, colors, or texture coordinates.
            glBindVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            // Define the vertices for the rectangle.
            // Each line below represents a vertex and its attributes (x, y, r, g, b).
            float[] vertices = new[]
            {
                -0.5f, 0.5f, 1f, 0f, 0f,   // Vertex 1 (position: (-0.5, 0.5), color: (1, 0, 0))
                0.5f, 0.5f, 0f, 1f, 0f,    // Vertex 2 (position: (0.5, 0.5), color: (0, 1, 0))
                -0.5f, -0.5f, 0f, 0f, 1f,   // Vertex 3 (position: (-0.5, -0.5), color: (0, 0, 1))

                0.5f, 0.5f, 0f, 1f, 0f,    // Vertex 4 (position: (0.5, 0.5), color: (0, 1, 0))
                0.5f, -0.5f, 0f, 1f, 1f,   // Vertex 5 (position: (0.5, -0.5), color: (0, 1, 1))
                -0.5f, -0.5f, 0f, 0f, 1f,  // Vertex 6 (position: (-0.5, -0.5), color: (0, 0, 1))
            };

            // Copy the vertex data from the managed array to the VBO.
            // We use 'fixed' to pin the managed array in memory so that it can be accessed by an unmanaged pointer.
            fixed (float* v = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
            }

            // Specify how the vertex attributes should be interpreted.
            // In this case, attribute 0 represents the position (x, y) and has two components (GL_FLOAT), 
            // and attribute 1 represents the color (r, g, b) and has three components.
            // The stride is the byte offset between consecutive vertex attribute data.
            // The last argument is the offset of the attribute within the vertex data.
            glVertexAttribPointer(0, 2, GL_FLOAT, false, 5 * sizeof(float), (void*)0);
            glVertexAttribPointer(1, 3, GL_FLOAT, false, 5 * sizeof(float), (void*)(2 * sizeof(float)));

            // Enable vertex attributes 0 and 1.
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);

            // Unbind the VBO and VAO to prevent further changes to them.
            glBindBuffer(GL_ARRAY_BUFFER, 0);
            glBindVertexArray(0);

            _engine.Events.KeyDown += Events_KeyDown;
            _engine.Events.KeyRepeat += Events_KeyDown;

            return true;
        }

        private readonly Camera2d testCamera = new(1, false) // 1 x Zoom, not top down
        {
            Position = new(0, 0, -1),
            Zoom = 1,
        };

        private void Events_KeyDown(object? sender, Events.KeyDownEventArgs e)
        {
            if (e.Key == Common.Key.W)
            {
                float factor = 10 * (float)_engine.Timing.CurrentFrame.DeltaTime;
                testCamera.Position = new(testCamera.Position.X, testCamera.Position.Y, testCamera.Position.Z + factor);
                Log.Information($"Camera moving forward: {testCamera.Position}");
            }
            else if (e.Key == Common.Key.A)
            {
                float factor = 10 * (float)_engine.Timing.CurrentFrame.DeltaTime;
                testCamera.Position = new(testCamera.Position.X - factor, testCamera.Position.Y, testCamera.Position.Z);
                Log.Information($"Camera moving forward: {testCamera.Position}");
            }
            else if (e.Key == Common.Key.D)
            {
                float factor = 10 * (float)_engine.Timing.CurrentFrame.DeltaTime;
                testCamera.Position = new(testCamera.Position.X + factor, testCamera.Position.Y, testCamera.Position.Z);
                Log.Information($"Camera moving forward: {testCamera.Position}");
            }
            else if (e.Key == Common.Key.S)
            {
                float factor = 10 * (float)_engine.Timing.CurrentFrame.DeltaTime;
                testCamera.Position = new(testCamera.Position.X, testCamera.Position.Y, testCamera.Position.Z - factor);
                Log.Information($"Camera moving forward: {testCamera.Position}");
            }
            else if (e.Key == Common.Key.Space)
            {
                float factor = 10 * (float)_engine.Timing.CurrentFrame.DeltaTime;
                testCamera.Position += World.ForwardVector * factor;
                Log.Information($"Camera moving forward test: {testCamera.Position}");
            }
        }

        /// <summary>
        /// Handles graphics rendering.
        /// </summary>
        internal void Render()
        {
            foreach (var window in _engine.Windows)
            {
                window.RenderingContext.Use();

                //glClearColor(MathF.Sin((float)_engine.Timing.TotalElapsedTime), MathF.Cos((float)_engine.Timing.TotalElapsedTime), 0, 1);
                glClearColor(0.2f, 0.2f, 0.2f, 1);
                glClear(GL_COLOR_BUFFER_BIT);

                // Test drawing triangles
                shader.Use();

                // Bind the Vertex Array Object (VAO) to use the configuration of vertex attributes stored in it.
                glBindVertexArray(vao);

                // Set up the rendered test objects transform
                Transform objectTransform = new()
                {
                    Position = new(0, 0, 0),  // in pixels
                    Rotation = new(0, 0, 0), // in degrees
                    Scale = new(800, 600, 1), // in pixels
                };

                // Calculate the view and projection matrices from the camera
                var viewMatrix = testCamera.GetViewMatrix();
                var projectionMatrix = testCamera.GetProjectionMatrix(window.Size.Width, window.Size.Height);
                var modelMatrix = Matrix4x4.CreateScale(objectTransform.Scale) *
                       Matrix4x4.CreateFromYawPitchRoll(
                           MathHelper.DegreesToRadians(objectTransform.Rotation.Yaw),
                           MathHelper.DegreesToRadians(objectTransform.Rotation.Pitch),
                           MathHelper.DegreesToRadians(objectTransform.Rotation.Roll)) *
                       Matrix4x4.CreateTranslation(objectTransform.Position);

                // Combine the model, view, and projection matrices to get the final MVP matrix
                var mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;

                // Pass the matrices to the shader
                shader.SetMatrix4("mvp", viewMatrix);

                // Draw the array of vertices as triangles.
                // GL_TRIANGLES specifies the primitive type to render (triangles in this case).
                // The second argument, '0', indicates the starting index within the VBO to start drawing.
                // The last argument, '6', indicates the number of vertices to draw.
                glDrawArrays(GL_TRIANGLES, 0, 6);

                // Unbind the VAO to prevent accidental modification.
                glBindVertexArray(0);

                // Log OpenGL errors
                LogGlErrors();
            }
        }

        private static void ClearGLErrors()
        {
            while (Glfw.GetError(out string _) != ErrorCode.None) ;
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