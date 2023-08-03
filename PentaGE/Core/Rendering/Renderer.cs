using GLFW;
using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Entities;
using PentaGE.Core.Graphics;
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
        private Shader shader;
        private Shader lightShader;
        private Texture texture;
        private Mesh testMesh1;
        private Mesh lightMesh1;
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

        private readonly List<Vertex> vertices = new()
        {
            //new(new(-1.0f, -1.0f,  1.0f), new(0f, 0f, 0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(0.0f, 0.0f)),
            //new(new(-1.0f, -1.0f, -1.0f), new(0f, 0f, 0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(5.0f, 0.0f)),
            //new(new( 1.0f, -1.0f, -1.0f), new(0f, 0f, 0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(0.0f, 0.0f)),
            //new(new( 1.0f, -1.0f,  1.0f), new(0f, 0f, 0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(5.0f, 0.0f)),
            //new(new( 0.0f,  2.0f,  0.0f), new(0f, 0f, 0f), new(0.92f, 0.86f, 0.76f, 1.0f), new(2.5f, 5.0f)),
            new(new(-0.5f, 0.0f,  0.5f), new( 0.0f, -1.0f, 0.0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(0.0f, 0.0f)),      // Bottom side
	        new(new(-0.5f, 0.0f, -0.5f), new( 0.0f, -1.0f, 0.0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(0.0f, 5.0f)),      // Bottom side
	        new(new( 0.5f, 0.0f, -0.5f), new( 0.0f, -1.0f, 0.0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(5.0f, 5.0f)),     // Bottom side
	        new(new( 0.5f, 0.0f,  0.5f), new( 0.0f, -1.0f, 0.0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(5.0f, 0.0f)),     // Bottom side

            new(new(-0.5f, 0.0f,  0.5f), new(-0.8f, 0.5f,  0.0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(0.0f, 0.0f)),     // Left Side
	        new(new(-0.5f, 0.0f, -0.5f), new(-0.8f, 0.5f,  0.0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(5.0f, 0.0f)),     // Left Side
	        new(new( 0.0f, 0.8f,  0.0f), new(-0.8f, 0.5f,  0.0f), new(0.92f, 0.86f, 0.76f, 1.0f), new(2.5f, 5.0f)),     // Left Side

	        new(new(-0.5f, 0.0f, -0.5f), new( 0.0f, 0.5f, -0.8f), new(0.83f, 0.70f, 0.44f, 1.0f), new(5.0f, 0.0f)),     // Non-facing side
	        new(new( 0.5f, 0.0f, -0.5f), new( 0.0f, 0.5f, -0.8f), new(0.83f, 0.70f, 0.44f, 1.0f), new(0.0f, 0.0f)),     // Non-facing side
	        new(new( 0.0f, 0.8f,  0.0f), new( 0.0f, 0.5f, -0.8f), new(0.92f, 0.86f, 0.76f, 1.0f), new(2.5f, 5.0f)),     // Non-facing side

	        new(new( 0.5f, 0.0f, -0.5f), new( 0.8f, 0.5f,  0.0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(0.0f, 0.0f)),    // Right side
	        new(new( 0.5f, 0.0f,  0.5f), new( 0.8f, 0.5f,  0.0f), new(0.83f, 0.70f, 0.44f, 1.0f), new(5.0f, 0.0f)),     // Right side
	        new(new( 0.0f, 0.8f,  0.0f), new( 0.8f, 0.5f,  0.0f), new(0.92f, 0.86f, 0.76f, 1.0f), new(2.5f, 5.0f)),     // Right side

	        new(new( 0.5f, 0.0f,  0.5f), new( 0.0f, 0.5f,  0.8f), new(0.83f, 0.70f, 0.44f, 1.0f), new(5.0f, 0.0f)),     // Facing side
	        new(new(-0.5f, 0.0f,  0.5f), new( 0.0f, 0.5f,  0.8f), new(0.83f, 0.70f, 0.44f, 1.0f), new(0.0f, 0.0f)),     // Facing side
	        new(new( 0.0f, 0.8f,  0.0f), new( 0.0f, 0.5f,  0.8f), new(0.92f, 0.86f, 0.76f, 1.0f), new(2.5f, 5.0f))       // Facing side
        };

        private readonly List<uint> indices = new()
        {
            0, 1, 2, // Bottom side
	        0, 2, 3, // Bottom side
	        4, 6, 5, // Left side
	        7, 9, 8, // Non-facing side
	        10, 12, 11, // Right side
	        13, 15, 14 // Facing side
        };

        private readonly List<Vertex> lightVertices = new()
        {
            new(new(-0.1f, -0.1f,  0.1f)),
            new(new(-0.1f, -0.1f, -0.1f)),
            new(new( 0.1f, -0.1f, -0.1f)),
            new(new( 0.1f, -0.1f,  0.1f)),
            new(new(-0.1f,  0.1f,  0.1f)),
            new(new(-0.1f,  0.1f, -0.1f)),
            new(new( 0.1f,  0.1f, -0.1f)),
            new(new( 0.1f,  0.1f,  0.1f))
        };

        private readonly List<uint> lightIndices = new()
        {
            0, 1, 2,
            0, 2, 3,
            0, 4, 7,
            0, 7, 3,
            3, 7, 6,
            3, 6, 2,
            2, 6, 5,
            2, 5, 1,
            1, 5, 4,
            1, 4, 0,
            4, 5, 6,
            4, 6, 7
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
            using (var logger = Log.Logger.BeginPerfLogger("Loading default shader"))
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

            using (var logger = Log.Logger.BeginPerfLogger("Loading light shader"))
            {
                try
                {
                    lightShader = new(@"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Shaders\SourceCode\Light.shader");
                    lightShader.Load();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Error loading shader: {ex}");
                }
            }

            // Initialize test texture
            using (var logger = Log.Logger.BeginPerfLogger("Loading test texture"))
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

            // Initialize test mesh
            testMesh1 = new(vertices, indices);
            var transform = new Transform(new(0, 0, 0), new(0, 0, 0), new(1f, 1f, 1f));
            var renderableMesh = new RenderableMeshEntity(testMesh1, shader, texture);

            renderableMesh.AddComponent(new TransformComponent(transform));

            // Initialize test light
            lightMesh1 = new(lightVertices, lightIndices);
            var transform2 = new Transform(new(0.75f, 0.75f, 0.75f), new(0, 0, 0), new(1f, 1f, 1f));
            var renderableLight = new RenderableMeshEntity(lightMesh1, lightShader);

            renderableLight.AddComponent(new TransformComponent(transform2));

            _engine.Scene.AddEntity(renderableMesh);
            _engine.Scene.AddEntity(renderableLight);

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

                _engine.Scene[0].GetComponent<TransformComponent>()!.Transform = objectTransform;

                #endregion

                UpdateCameraPosition();

                // Update the current window's rendering context
                window.RenderingContext.Use();

                // Clear the screen to a dark gray color and clear the depth buffer
                glClearColor(0.2f, 0.2f, 0.2f, 1);
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                // Draw the test mesh
                _engine.Scene.Render(testCamera, window);
            }
        }

        internal void Terminate()
        {

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
                _direction = new(_direction.X, _direction.Y, 0);
            else if (e.Key == Key.S)
                _direction = new(_direction.X, _direction.Y, 0);
            else if (e.Key == Key.A)
                _direction = new(0, _direction.Y, _direction.Z);
            else if (e.Key == Key.D)
                _direction = new(0, _direction.Y, _direction.Z);
            else if (e.Key == Key.Q)
                _direction = new(_direction.X, 0, _direction.Z);
            else if (e.Key == Key.E)
                _direction = new(_direction.X, 0, _direction.Z);
            else if (e.Key == Key.LeftShift)
                _modifierPressed = false;
        }

        private void UpdateCameraPosition()
        {
            float increment = 5f;
            Vector3 direction = Vector3.Zero;

            Rotation originalRotation = testCamera.Rotation;
            if (_modifierPressed)
                testCamera.Rotation = new(testCamera.Rotation.Yaw, 0, testCamera.Rotation.Roll);

            if (_direction.X == 1)
                direction += testCamera.Rotation.GetRightVector();
            else if (_direction.X == -1)
                direction += testCamera.Rotation.GetLeftVector();
            if (_direction.Y == 1)
                direction += testCamera.Rotation.GetUpVector();
            else if (_direction.Y == -1)
                direction += testCamera.Rotation.GetDownVector();
            if (_direction.Z == 1)
                direction -= testCamera.Rotation.GetForwardVector();
            else if (_direction.Z == -1)
                direction -= testCamera.Rotation.GetBackwardVector();

            testCamera.Position += direction * (increment * (float)_engine.Timing.CurrentFrame.DeltaTime);

            if (_modifierPressed)
                testCamera.Rotation = originalRotation;
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

                float sensitivityFactor = 1f;

                float xDiff = (e.Position.X - _mouseInitialLocation.X) / ((float)e.Window.Size.Width / 2) * sensitivityFactor;

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