

using GLFW;
using PentaGE.Common;
using PentaGE.Core.Logging;
using Serilog;
using System.Drawing;
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
        private VertexArray vao;
        private VertexBuffer vbo;
        private ElementBuffer ebo;
        private Shader shader;
        private Texture texture;
        private bool rotate = true;
        private bool wireframe = false;

        /// <summary>
        /// Creates a new instance of the Renderer class.
        /// </summary>
        /// <param name="engine">The PentaGameEngine instance associated with this Renderer.</param>
        internal Renderer(PentaGameEngine engine)
        {
            _engine = engine;
        }

        // Set up the rendered test objects transform
        private Transform objectTransform = new()
        {
            Position = new(0, 0, 0),    // in units
            Rotation = new(0, 0, 0),    // in degrees
            Scale = new(1, 1, 1),       // multipliers
        };

        private readonly Camera3d testCamera = new()
        {
            Position = new(0, 0, 2.5f),
            FieldOfView = 90,
            Rotation = new(
                0,  // Yaw 
                0,  // Pitch
                0   // Roll
            )
        };

        private float[] vertices = new float[]
        {
            // Coordinates          // Colors               // Texture Coordinates
            //-1.0f, -1.0f, -1.0f,    0.83f, 0.70f, 0.44f,    0.1f, 1.0f, // Cube
            // 1.0f, -1.0f, -1.0f,    0.83f, 0.70f, 0.44f,    0.1f, 1.0f, // -- " --
            // 1.0f,  1.0f, -1.0f,    0.83f, 0.70f, 0.44f,    0.1f, 1.0f, // -- " --
            //-1.0f,  1.0f, -1.0f,    0.83f, 0.70f, 0.44f,    0.1f, 1.0f, // -- " --
            //-1.0f, -1.0f,  1.0f,    0.92f, 0.86f, 0.76f,    0.1f, 1.0f, // -- " --
            // 1.0f, -1.0f,  1.0f,    0.92f, 0.86f, 0.76f,    0.1f, 1.0f, // -- " --
            // 1.0f,  1.0f,  1.0f,    0.92f, 0.86f, 0.76f,    0.1f, 1.0f, // -- " --
            //-1.0f,  1.0f,  1.0f,    0.92f, 0.86f, 0.76f,    0.1f, 1.0f  // -- " --
            //-1.0f, -1.0f, 0.0f,     1.0f, 0.0f, 0.0f,       0.0f, 0.0f, // Plane
	        //-1.0f,  1.0f, 0.0f,     0.0f, 1.0f, 0.0f,       0.0f, 1.0f, // -- " --
	        // 1.0f,  1.0f, 0.0f,     0.0f, 0.0f, 1.0f,       1.0f, 1.0f, // -- " --
	        // 1.0f, -1.0f, 0.0f,     1.0f, 1.0f, 1.0f,       1.0f, 0.0f  // -- " --
            -1.0f, -1.0f,  1.0f,     0.83f, 0.70f, 0.44f,    0.0f, 0.0f, // Pyramid
            -1.0f, -1.0f, -1.0f,     0.83f, 0.70f, 0.44f,    5.0f, 0.0f, // -- " --
             1.0f, -1.0f, -1.0f,     0.83f, 0.70f, 0.44f,    0.0f, 0.0f, // -- " --
             1.0f, -1.0f,  1.0f,     0.83f, 0.70f, 0.44f,    5.0f, 0.0f, // -- " --
             0.0f,  2.0f,  0.0f,     0.92f, 0.86f, 0.76f,    2.5f, 5.0f  // -- " --
        };

        private uint[] indices = new uint[]
        {
            //0, 1, 2,    2, 3, 0,    // Cube
            //4, 5, 6,    6, 7, 4,    // -- " --
            //0, 4, 7,    7, 3, 0,    // -- " --
            //1, 5, 6,    6, 2, 1,    // -- " --
            //3, 2, 6,    6, 7, 3,    // -- " --
            //0, 1, 5,    5, 4, 0     // -- " --
            //0, 2, 1,    0, 3, 2     // Plane
            0, 1, 2,                // Pyramid
            0, 2, 3,                // -- " --
            0, 1, 4,                // -- " --
            1, 2, 4,                // -- " --
            2, 3, 4,                // -- " --
            3, 0, 4                 // -- " --
        };

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

            _engine.Events.GlfwError += Events_GlfwError;

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

            // Enable face culling
            //glEnable(GL_CULL_FACE);
            //glCullFace(GL_BACK);
            //glFrontFace(GL_CW);
            glEnable(GL_DEPTH_TEST);

            // Add engine events for moving the camera (during debug - remove later)
            _engine.Events.KeyDown += Events_KeyDown;
            _engine.Events.KeyUp += Events_KeyUp;
            _engine.Events.KeyRepeat += Events_KeyDown;
            _engine.Events.MouseDown += Events_MouseDown;
            _engine.Events.MouseMoved += Events_MouseMoved;
            _engine.Events.MouseUp += Events_MouseUp;

            #region Set up a test object to render

            // Initializing test shader
            using (var logger = Log.Logger.BeginPerfLogger("Loading shader"))
            {
                try
                {
                    shader = new(@"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Shaders\SourceCode\Default.shader");
                    shader.Load();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Error loading shader: {ex}");
                }
            }

            // Initialize test texture
            using (var logger = Log.Logger.BeginPerfLogger("Loading texture"))
            {
                try
                {
                    texture = new(
                    @"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Textures\SourceFiles\TestTexture.jpg",
                    GL_TEXTURE_2D,
                    GL_TEXTURE0,
                    GL_RGBA,
                    GL_UNSIGNED_BYTE);
                    Texture.SetTextureSlot(shader, "tex0", 0); // Set the texture slot to 0
                }
                catch { /* Gets logged in the constructor */ }
            }

            // Create a VAO, VBO, and EBO
            vao = new();
            vbo = new(ref vertices, sizeof(float) * vertices.Length);
            ebo = new(ref indices, sizeof(uint) * indices.Length);

            // Bind the VAO, VBO, and EBO to the current context
            vao.Bind();
            ebo.Bind();

            // Specify how the vertex attributes should be interpreted.
            vao.LinkAttribute(ref vbo, 0, 3, GL_FLOAT, 8 * sizeof(float), (void*)0);                    // Coordinates
            vao.LinkAttribute(ref vbo, 1, 3, GL_FLOAT, 8 * sizeof(float), (void*)(3 * sizeof(float)));  // Colors
            vao.LinkAttribute(ref vbo, 2, 2, GL_FLOAT, 8 * sizeof(float), (void*)(6 * sizeof(float)));  // Texture coordinates

            // Unbind the VBO, EBO, and VAO to prevent further changes to them.
            VertexBuffer.Unbind();  // Unbind the VBO
            ElementBuffer.Unbind(); // Unbind the EBO
            VertexArray.Unbind();   // Unbind the VAO

            #endregion

            return true;
        }

        /// <summary>
        /// Handles graphics rendering.
        /// </summary>
        internal unsafe void Render()
        {
            foreach (var window in _engine.Windows)
            {
                #region Test rotation

                // Rotate the object based on the current time
                //objectTransform.Rotation = rotate ? new(
                //    MathF.Sin((float)_engine.Timing.TotalElapsedTime) * 90,
                //    MathF.Cos((float)_engine.Timing.TotalElapsedTime) * 90,
                //    0) :
                //    new(0, 0, 0);
                objectTransform.Rotation = rotate ? new(
                    MathF.Sin((float)_engine.Timing.TotalElapsedTime) * 180,
                    0,
                    0) :
                    objectTransform.Rotation;//new(0, 0, 0);

                #endregion

                UpdateCameraPosition();

                // Update the current window's rendering context
                window.RenderingContext.Use();

                // Clear the screen to a dark gray color and clear the depth buffer
                glClearColor(0.2f, 0.2f, 0.2f, 1);
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                // Use the shader program
                shader.Use();

                // Calculate the view and projection matrices from the camera
                // ViewMatrix means "camera space" (or "eye space") and is used for moving the camera.
                // ProjectionMatrix means "clip space" (or "normalized device coordinates")
                //  and is used for clipping and perspective.
                // ModelMatrix means "object space" (or "model space") - the default space for an object.
                var viewMatrix = testCamera.GetViewMatrix();
                var projectionMatrix = testCamera.GetProjectionMatrix(window.Size.Width, window.Size.Height);
                var modelMatrix = Matrix4x4.CreateScale(objectTransform.Scale)
                    * objectTransform.Rotation.ToMatrix4x4()
                    * Matrix4x4.CreateTranslation(objectTransform.Position);

                // Combine the model, view, and projection matrices to get the final MVP matrix
                var mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;

                // Pass the matrices to the shader (must be done after shader.Use())
                shader.SetUniform("mvp", mvpMatrix);

                // Bind the texture to the current context
                texture.Bind();

                // Bind the Vertex Array Object (VAO) to use the configuration
                // of vertex attributes stored in it.
                vao.Bind();

                // Draw the object using the indices of the EBO
                glPolygonMode(GL_FRONT_AND_BACK, wireframe ? GL_LINE : GL_FILL);
                glDrawElements(GL_TRIANGLES, indices); // Draw the object using the indices of the EBO

                // Unbind the VAO, VBO & EBO to prevent accidental modification.
                VertexArray.Unbind();   // Unbind the VAO
                VertexBuffer.Unbind();  // Unbind the VBO (not necessary, but good practice)
                ElementBuffer.Unbind(); // Unbind the EBO (not necessary, but good practice)
            }
        }

        internal void Terminate()
        {
            vbo.Dispose();
            ebo.Dispose();
        }

        private void Events_GlfwError(object? sender, Events.GlfwErrorEventArgs e)
        {
            Log.Error($"GLFW Error: {e.ErrorCode} - {e.Message}");
        }

        // This code does not belong here, but is here for testing purposes

        #region Keyboard events

        private Vector3 _direction = Vector3.Zero;
        private bool _modifierPressed = false;

        private void Events_KeyDown(object? sender, Events.KeyDownEventArgs e)
        {
            // For moving the camera (during debug - remove later)
            if (e.Key == Key.W)
            {
                _direction = new(_direction.X, _direction.Y, 1);
            }
            else if (e.Key == Key.S)
            {
                _direction = new(_direction.X, _direction.Y, -1);
            }
            else if (e.Key == Key.A)
            {
                _direction = new(-1, _direction.Y, _direction.Z);
            }
            else if (e.Key == Key.D)
            {
                _direction = new(1, _direction.Y, _direction.Z);
            }
            else if (e.Key == Key.Q)
            {
                _direction = new(_direction.X, -1, _direction.Z);
            }
            else if (e.Key == Key.E)
            {
                _direction = new(_direction.X, 1, _direction.Z);
            }
            else if (e.Key == Key.Left)
            {
                objectTransform.Rotation += new Rotation(-1, 0, 0) * 5;
                Log.Information($"Object yaw left: {objectTransform.Rotation}");
            }
            else if (e.Key == Key.Right)
            {
                objectTransform.Rotation += new Rotation(1, 0, 0) * 5;
                Log.Information($"Object yaw right: {objectTransform.Rotation}");
            }
            else if (e.Key == Key.Up)
            {
                objectTransform.Rotation += new Rotation(0, 1, 0) * 5;
                Log.Information($"Object pitch up: {objectTransform.Rotation}");
            }
            else if (e.Key == Key.Down)
            {
                objectTransform.Rotation += new Rotation(0, -1, 0) * 5;
                Log.Information($"Object pitch down: {objectTransform.Rotation}");
            }
            else if (e.Key == Key.Z)
            {
                testCamera.FieldOfView += 5;
                Log.Information($"Camera FoV increasing: {testCamera.FieldOfView}");
            }
            else if (e.Key == Key.C)
            {
                testCamera.FieldOfView -= 5;
                Log.Information($"Camera FoV decreasing: {testCamera.FieldOfView}");
            }
            else if (e.Key == Key.R)
            {
                rotate = !rotate;
            }
            else if (e.Key == Key.F1)
            {
                wireframe = !wireframe;
            }
            else if (e.Key == Key.Alpha1)
            {
                testCamera.Rotation += new Rotation(0, 0, 1) * 5;
                Log.Information($"Object roll left: {testCamera.Rotation}");
            }
            else if (e.Key == Key.Alpha2)
            {
                testCamera.Rotation += new Rotation(0, 0, -1) * 5;
                Log.Information($"Object roll right: {testCamera.Rotation}");
            }
            else if (e.Key == Key.LeftShift)
            {
                _modifierPressed = true;
            }
        }

        private void Events_KeyUp(object? sender, Events.KeyUpEventArgs e)
        {
            if (e.Key == Key.W)
            {
                _direction = new(_direction.X, _direction.Y, 0);
            }
            else if (e.Key == Key.S)
            {
                _direction = new(_direction.X, _direction.Y, 0);
            }
            else if (e.Key == Key.A)
            {
                _direction = new(0, _direction.Y, _direction.Z);
            }
            else if (e.Key == Key.D)
            {
                _direction = new(0, _direction.Y, _direction.Z);
            }
            else if (e.Key == Key.Q)
            {
                _direction = new(_direction.X, 0, _direction.Z);
            }
            else if (e.Key == Key.E)
            {
                _direction = new(_direction.X, 0, _direction.Z);
            }
            else if (e.Key == Key.LeftShift)
            {
                _modifierPressed = false;
            }
        }

        private void UpdateCameraPosition()
        {
            float increment = 5f;
            Vector3 direction = Vector3.Zero;

            if (_direction.X == 1)
                direction += testCamera.Rotation.GetRightVector();
            else if (_direction.X == -1)
                direction += testCamera.Rotation.GetLeftVector();
            if (_direction.Y == 1)
                direction += testCamera.Rotation.GetUpVector();
            else if (_direction.Y == -1)
                direction += testCamera.Rotation.GetDownVector();
            if (_direction.Z == 1)
            {
                if (_modifierPressed)
                {
                    Rotation originalRotation = testCamera.Rotation;
                    testCamera.Rotation = new(testCamera.Rotation.Yaw, 0, testCamera.Rotation.Roll);

                    direction -= testCamera.Rotation.GetForwardVector();

                    testCamera.Rotation = originalRotation;
                }
                else
                {
                    direction -= testCamera.Rotation.GetForwardVector();
                }
            }
            else if (_direction.Z == -1)
            {
                if (_modifierPressed)
                {
                    Rotation originalRotation = testCamera.Rotation;
                    testCamera.Rotation = new(testCamera.Rotation.Yaw, 0, testCamera.Rotation.Roll);

                    direction -= testCamera.Rotation.GetBackwardVector();

                    testCamera.Rotation = originalRotation;
                }
                else
                {
                    direction -= testCamera.Rotation.GetBackwardVector();
                }
            }

            testCamera.Position += direction * (increment * (float)_engine.Timing.CurrentFrame.DeltaTime);
        }

        #endregion

        #region Mouse events

        private int _mouseMode = 0;
        private Point _mouseInitialLocation = new(0, 0);
        private bool _mouseInitialLocationSet = false;
        private Rotation _initialRotation = new(0, 0, 0);
        private int _lastY = 0;
        private int _lastX = 0;

        private void Events_MouseMoved(object? sender, Events.MouseMovedEventArgs e)
        {
            // TODO: Needs serious refactoring
            if (_mouseMode == 1)
            {
                if (!_mouseInitialLocationSet)
                {
                    _mouseInitialLocation = e.Position;
                    _mouseInitialLocationSet = true;
                    _initialRotation = testCamera.Rotation;
                }

                float sensitivity = 1f;

                float xDiff = (e.Position.X - _mouseInitialLocation.X) / ((float)e.Window.Size.Width / 2) * sensitivity;
                float yDiff = (e.Position.Y - _mouseInitialLocation.Y) / ((float)e.Window.Size.Height / 2) * sensitivity;

                float yaw = _initialRotation.Yaw - (xDiff * 90);
                float pitch = _initialRotation.Pitch - (yDiff * 90);

                testCamera.Rotation = new(yaw, pitch, testCamera.Rotation.Roll);

                // Reset the mouse position to the center of the screen
                if (e.Position.X > e.Window.Size.Width || e.Position.X < 0 ||
                    e.Position.Y > e.Window.Size.Height || e.Position.Y < 0)
                {
                    Glfw.SetCursorPosition(e.Window.Handle, e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                    _mouseInitialLocation = new(e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                    _initialRotation = testCamera.Rotation;
                }
            }
            else if (_mouseMode == 2)
            {
                if (!_mouseInitialLocationSet)
                {
                    _mouseInitialLocation = e.Position;
                    _mouseInitialLocationSet = true;
                    _initialRotation = testCamera.Rotation;
                    _lastY = e.Position.Y;
                }

                float sensitivity = 1f;

                float xDiff = (e.Position.X - _mouseInitialLocation.X) / ((float)e.Window.Size.Width / 2) * sensitivity;

                float yaw = _initialRotation.Yaw - (xDiff * 90);

                testCamera.Rotation = new(yaw, testCamera.Rotation.Pitch, testCamera.Rotation.Roll);

                // Temporarily set the pitch angle to zero
                Rotation originalRotation = testCamera.Rotation;
                testCamera.Rotation = new(testCamera.Rotation.Yaw, 0, testCamera.Rotation.Roll);

                Vector3 direction = Vector3.Zero;

                if (e.Position.Y > _lastY)
                    direction += testCamera.Rotation.GetForwardVector();
                else if (e.Position.Y < _lastY)
                    direction += testCamera.Rotation.GetBackwardVector();

                _lastY = e.Position.Y;

                float increment = 50f;
                testCamera.Position += direction * (increment * (float)_engine.Timing.CurrentFrame.DeltaTime);

                testCamera.Rotation = originalRotation;

                // Reset the mouse position to the center of the screen
                if (e.Position.X > e.Window.Size.Width || e.Position.X < 0 ||
                    e.Position.Y > e.Window.Size.Height || e.Position.Y < 0)
                {
                    Glfw.SetCursorPosition(e.Window.Handle, e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                    _mouseInitialLocation = new(e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                    _initialRotation = testCamera.Rotation;
                }
            }
            else if (_mouseMode == 3)
            {
                if (!_mouseInitialLocationSet)
                {
                    _mouseInitialLocation = e.Position;
                    _mouseInitialLocationSet = true;
                    _initialRotation = testCamera.Rotation;
                    _lastY = e.Position.Y;
                    _lastX = e.Position.X;
                }

                // Temporarily set the pitch angle to zero
                Rotation originalRotation = testCamera.Rotation;
                testCamera.Rotation = new(testCamera.Rotation.Yaw, 0, testCamera.Rotation.Roll);

                Vector3 direction = Vector3.Zero;

                if (e.Position.X > _lastX)
                    direction += testCamera.Rotation.GetRightVector();
                else if (e.Position.X < _lastX)
                    direction += testCamera.Rotation.GetLeftVector();

                if (e.Position.Y > _lastY)
                    direction -= testCamera.Rotation.GetUpVector();
                else if (e.Position.Y < _lastY)
                    direction -= testCamera.Rotation.GetDownVector();

                testCamera.Rotation = originalRotation;

                _lastX = e.Position.X;
                _lastY = e.Position.Y;

                float increment = 50f;
                testCamera.Position += direction * (increment * (float)_engine.Timing.CurrentFrame.DeltaTime);

                // Reset the mouse position to the center of the screen
                if (e.Position.X > e.Window.Size.Width || e.Position.X < 0 ||
                    e.Position.Y > e.Window.Size.Height || e.Position.Y < 0)
                {
                    Glfw.SetCursorPosition(e.Window.Handle, e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                    _mouseInitialLocation = new(e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                    _initialRotation = testCamera.Rotation;
                }
            }
        }

        private void Events_MouseDown(object? sender, Events.MouseButtonEventArgs e)
        {
            if (e.Button == Common.MouseButton.Left && _mouseMode == 0)
            {
                _mouseMode = 1;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Hidden);
            }
            else if (e.Button == Common.MouseButton.Right && _mouseMode == 0)
            {
                _mouseMode = 2;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Hidden);
            }
            else if (e.Button == Common.MouseButton.Middle)
            {
                _mouseMode = 3;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Hidden);
            }
        }

        private void Events_MouseUp(object? sender, Events.MouseButtonEventArgs e)
        {
            if (e.Button == Common.MouseButton.Left && _mouseMode == 1)
            {
                _mouseMode = 0;
                _mouseInitialLocationSet = false;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Normal);
            }
            else if (e.Button == Common.MouseButton.Right && _mouseMode == 2)
            {
                _mouseMode = 0;
                _mouseInitialLocationSet = false;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Normal);
            }
            else if (e.Button == Common.MouseButton.Middle && _mouseMode == 3)
            {
                _mouseMode = 0;
                _mouseInitialLocationSet = false;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Normal);
            }
        }

        #endregion

    }
}