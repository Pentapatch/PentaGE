using GLFW;
using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Entities;
using PentaGE.Core.Events;
using PentaGE.Core.Rendering;
using PentaGE.Maths;
using Serilog;
using System.Drawing;
using System.Numerics;

namespace PentaGE.Core
{
    /// <summary>
    /// Provides camera control functionalities for an editor environment.
    /// </summary>
    public sealed class EditorCameraController : CameraController
    {
        #region Fields and constants

        public const float MinFieldOfView = 10;
        public const float MaxFieldOfView = 170;

        private Vector3 _positionAxes = Vector3.Zero;
        private Vector3 _rotationAxes = Vector3.Zero;
        private float _fieldOfViewDirection = 0f;

        private Vector3? _targetPosition = null;
        private Rotation? _targetRotation = null;
        private float? _targetFieldOfView = null;
        private Vector3? _orbitTarget = null;

        private bool _alternateMode = false;

        private MouseModeEnum _mouseMode = MouseModeEnum.None;
        private Point _mouseInitialPosition = new(0, 0);
        private Point _mouseOffsetPosition = new(0, 0);
        private bool _mouseOffsetPositionSet = false;
        private Rotation _cameraRotationOffset = new(0, 0, 0);
        private int _mouseLastY = 0;
        private int _mouseLastX = 0;

        #endregion

        /// <summary>
        /// Enumeration representing different modes of mouse interaction.
        /// </summary>
        private enum MouseModeEnum
        {
            /// <summary>
            /// No mouse interaction is currently active.
            /// </summary>
            None = -1,

            /// <summary>
            /// The camera is being rotated around the yaw and pitch axes based on mouse movement.
            /// </summary>
            YawAndPitch = 0,

            /// <summary>
            /// The camera is being moved along the Z-axis (forward/backward) and rotated around the yaw axis based on mouse movement.
            /// </summary>
            MoveZAndYaw = 1,

            /// <summary>
            /// The camera is being moved along the X and Y axes (left/right and up/down) based on mouse movement.
            /// </summary>
            MoveXAndY = 2,
        }

        /// <summary>
        /// Gets or sets the movement speed of the camera.
        /// </summary>
        public float Speed { get; set; } = 5f;

        /// <summary>
        /// Gets or sets the sensitivity factor for mouse movement, which influences the rate at which the camera responds to mouse input.
        /// </summary>
        /// <remarks>
        /// The mouse sensitivity is used in conjunction with the <see cref="Speed"/> property to calculate the movement speed of the camera
        /// when using the mouse for camera control. Higher values of mouse sensitivity result in faster camera movement in response to mouse input.
        /// </remarks>
        public float MouseSpeedFactor { get; set; } = 0.002f;

        /// <summary>
        /// Gets or sets the rotation speed of the camera.
        /// </summary>
        public float RotationSpeed { get; set; } = 90f;

        /// <summary>
        /// Gets or sets the field of view change speed of the camera.
        /// </summary>
        public float FieldOfViewSpeed { get; set; } = 50f;

        /// <summary>
        /// Gets or sets the sensitivity of the mouse movement.
        /// </summary>
        public float MouseSensitivity { get; set; } = 1f;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorCameraController"/> class.
        /// </summary>
        public EditorCameraController()
        {

        }

        /// <summary>
        /// Sets the field of view of the camera.
        /// </summary>
        /// <param name="fieldOfView">The new field of view in degrees.</param>
        public void SetFieldOfView(float? fieldOfView)
        {
            if (fieldOfView > MaxFieldOfView)
            {
                Log.Warning($"Field of view cannot be greater than {MaxFieldOfView} degrees");
                fieldOfView = MaxFieldOfView;
            }
            else if (fieldOfView < MinFieldOfView)
            {
                Log.Warning($"Field of view cannot be less than {MinFieldOfView} degrees");
                fieldOfView = MinFieldOfView;
            }

            _targetFieldOfView = fieldOfView;
        }

        /// <summary>
        /// Sets the position of the camera in world space.
        /// </summary>
        /// <param name="position">The new position of the camera.</param>
        public void SetPosition(Vector3? position)
        {
            _targetPosition = position;
        }

        /// <summary>
        /// Sets the position of the camera to be near an entity's position.
        /// </summary>
        /// <param name="entity">The entity near which to position the camera.</param>
        /// <param name="zOffset">The optional offset along the Z-axis.</param>
        public void SetPosition(Entity entity, float? zOffset = 2)
        {
            if (entity.GetComponent<TransformComponent>() is TransformComponent transformComponent)
            {
                var position = transformComponent.Transform.Position;
                if (zOffset.HasValue && ActiveCamera is not null)
                {
                    position += Rotation.GetLookAt(position, ActiveCamera.Position).GetForwardVector() * zOffset.Value;
                }
                _targetPosition = position;
            }
            else
            {
                _targetPosition = null;
            }
        }

        /// <summary>
        /// Sets the rotation of the camera.
        /// </summary>
        /// <param name="rotation">The new rotation of the camera.</param>
        public void SetRotation(Rotation? rotation)
        {
            _targetRotation = rotation;
        }

        /// <summary>
        /// Sets the orbit target for the camera.
        /// </summary>
        /// <param name="target">The new orbit target position.</param>
        public void SetOrbitTarget(Vector3? target)
        {
            _orbitTarget = target;
        }

        /// <summary>
        /// Sets the orbit target for the camera to be near an entity's position.
        /// </summary>
        /// <param name="entity">The entity near which to set the orbit target.</param>
        /// <param name="zOffset">The optional offset along the Z-axis.</param>
        public void SetOrbitTarget(Entity entity, float? zOffset = null)
        {
            if (entity.GetComponent<TransformComponent>() is TransformComponent transformComponent)
            {
                if (zOffset.HasValue) SetPosition(entity, zOffset);
                _orbitTarget = transformComponent.Transform.Position;
            }
            else
            {
                _orbitTarget = null;
            }
        }

        /// <summary>
        /// Updates the editor camera's behavior based on user input and target values.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        protected override void Update(float deltaTime)
        {
            if (ActiveCamera is not Camera3d camera) return;

            // Move the camera to the target position
            if (_targetPosition is Vector3 targetPosition)
            {
                var factor = MathF.Min(deltaTime * Speed, 1);
                camera.Position = Vector3.Lerp(camera.Position, targetPosition, factor);

                if (camera.Position.IsApproximately(targetPosition))
                {
                    camera.Position = targetPosition;
                    _targetPosition = null;
                }
            }

            // Rotate the camera to the target rotation
            if (_targetRotation is Rotation targetRotation)
            {
                var factor = MathF.Min(deltaTime * Speed, 1);
                camera.Rotation = Rotation.Lerp(camera.Rotation, targetRotation, factor);

                if (camera.Rotation.IsApproximately(targetRotation))
                {
                    camera.Rotation = targetRotation;
                    _targetRotation = null;
                }
            }

            // Change the field of view to the target field of view
            if (_targetFieldOfView is float targetFieldOfView)
            {
                var factor = MathF.Min(deltaTime * Speed, 1);
                camera.FieldOfView = MathHelper.Lerp(camera.FieldOfView, targetFieldOfView, factor);

                if (MathF.Abs(camera.FieldOfView - targetFieldOfView) < 0.1f)
                {
                    camera.FieldOfView = targetFieldOfView;
                    _targetFieldOfView = null;
                }
            }

            // Continously move the camera
            if (_targetPosition is null)
            {
                Rotation originalRotation = camera.Rotation;
                if (_alternateMode)
                    camera.Rotation = new(camera.Rotation.Yaw, 0, camera.Rotation.Roll);

                // Determine the direction to move the camera
                Vector3 positionVector = Vector3.Zero;
                if (_positionAxes.X == 1)
                    positionVector += camera.Rotation.GetRightVector();
                else if (_positionAxes.X == -1)
                    positionVector += camera.Rotation.GetLeftVector();

                if (_positionAxes.Y == 1)
                    positionVector += camera.Rotation.GetUpVector();
                else if (_positionAxes.Y == -1)
                    positionVector += camera.Rotation.GetDownVector();

                if (_positionAxes.Z == 1)
                    positionVector -= camera.Rotation.GetForwardVector();
                else if (_positionAxes.Z == -1)
                    positionVector -= camera.Rotation.GetBackwardVector();

                // Move the camera
                camera.Position += positionVector * (Speed * deltaTime);

                if (_alternateMode)
                    camera.Rotation = originalRotation;
            }

            // Continously rotate the camera
            if (_targetRotation is null && _orbitTarget is null)
            {
                // Rotate the camera
                float yaw = _rotationAxes.X * RotationSpeed * deltaTime;
                float pitch = _rotationAxes.Y * RotationSpeed * deltaTime;
                float roll = _rotationAxes.Z * RotationSpeed * deltaTime;
                camera.Rotation += new Rotation(yaw, pitch, roll);
            }
            else if (_orbitTarget is Vector3 target)
            {
                // Override all rotations if a orbit target is set
                camera.Rotation = Rotation.GetLookAt(target, camera.Position);
            }

            // Continously change the field of view
            if (_targetFieldOfView is null)
            {
                camera.FieldOfView += _fieldOfViewDirection * deltaTime * FieldOfViewSpeed;
            }
        }

        /// <summary>
        /// Handles the key down event by interpreting the pressed key and modifying camera behavior accordingly.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments containing information about the pressed key.</param>
        protected override void KeyDown(object? sender, KeyDownEventArgs e)
        {
            if (e.Key == Key.W)
                _positionAxes = new(_positionAxes.X, _positionAxes.Y, 1);
            else if (e.Key == Key.S)
                _positionAxes = new(_positionAxes.X, _positionAxes.Y, -1);
            else if (e.Key == Key.A)
                _positionAxes = new(-1, _positionAxes.Y, _positionAxes.Z);
            else if (e.Key == Key.D)
                _positionAxes = new(1, _positionAxes.Y, _positionAxes.Z);
            else if (e.Key == Key.Q)
                _positionAxes = new(_positionAxes.X, -1, _positionAxes.Z);
            else if (e.Key == Key.E)
                _positionAxes = new(_positionAxes.X, 1, _positionAxes.Z);
            else if (e.Key == Key.Home)
            {
                _targetPosition = new(0, 0, 2);
                _targetRotation = new(0, 0, 0);
                _targetFieldOfView = 90f;
            }
            else if (e.Key == Key.Backspace)
            {
                if (_orbitTarget is null)
                    _orbitTarget = new(0, 0, 0);
                else
                    _orbitTarget = null;
            }
            else if (e.Key == Key.LeftShift && e.ModifierKeys == ModifierKey.Shift)
                _alternateMode = true;
            else if (e.Key == Key.Z)
            {
                if (_alternateMode)
                    _rotationAxes = new(0, 0, 1);
                else
                    _fieldOfViewDirection = 1;
            }
            else if (e.Key == Key.C)
            {
                if (_alternateMode)
                    _rotationAxes = new(0, 0, -1);
                else
                    _fieldOfViewDirection = -1;
            }
        }

        /// <summary>
        /// Handles the key up event by interpreting the released key and reverting camera behavior changes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments containing information about the released key.</param>
        protected override void KeyUp(object? sender, KeyUpEventArgs e)
        {
            if (e.Key == Key.W || e.Key == Key.S)
                _positionAxes = new(_positionAxes.X, _positionAxes.Y, 0);
            else if (e.Key == Key.A || e.Key == Key.D)
                _positionAxes = new(0, _positionAxes.Y, _positionAxes.Z);
            else if (e.Key == Key.Q || e.Key == Key.E)
                _positionAxes = new(_positionAxes.X, 0, _positionAxes.Z);
            else if (_alternateMode && (e.Key == Key.C || e.Key == Key.Z))
                _rotationAxes = new(_rotationAxes.X, _rotationAxes.Y, 0);
            else if (!_alternateMode && (e.Key == Key.C || e.Key == Key.Z))
                _fieldOfViewDirection = 0f;
            else if (e.Key == Key.LeftShift)
                _alternateMode = false;
        }

        /// <summary>
        /// Handles the mouse button down event by interpreting the pressed mouse button and initiating camera control modes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments containing information about the pressed mouse button.</param>
        protected override void MouseDown(object? sender, Events.MouseButtonEventArgs e)
        {
            if (e.Button == Common.MouseButton.Left && _mouseMode == MouseModeEnum.None)
            {
                _mouseOffsetPositionSet = false;
                _mouseMode = MouseModeEnum.YawAndPitch;
            }
            else if (e.Button == Common.MouseButton.Right && _mouseMode == MouseModeEnum.None)
            {
                _mouseOffsetPositionSet = false;
                _mouseMode = MouseModeEnum.MoveZAndYaw;
            }
            else if (e.Button == Common.MouseButton.Middle)
            {
                _mouseOffsetPositionSet = false;
                _mouseMode = MouseModeEnum.MoveXAndY;
            }

            if (_mouseMode != MouseModeEnum.None)
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Hidden);
        }

        /// <summary>
        /// Handles the mouse button up event by interpreting the released mouse button and ending camera control modes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments containing information about the released mouse button.</param>
        protected override void MouseUp(object? sender, Events.MouseButtonEventArgs e)
        {
            if (e.Button == Common.MouseButton.Left && _mouseMode == MouseModeEnum.YawAndPitch)
                _mouseMode = MouseModeEnum.None;
            else if (e.Button == Common.MouseButton.Right && _mouseMode == MouseModeEnum.MoveZAndYaw)
                _mouseMode = MouseModeEnum.None;
            else if (e.Button == Common.MouseButton.Middle && _mouseMode == MouseModeEnum.MoveXAndY)
                _mouseMode = MouseModeEnum.None;

            if (_mouseMode == MouseModeEnum.None)
            {
                Glfw.SetCursorPosition(e.Window.Handle, _mouseInitialPosition.X, _mouseInitialPosition.Y);
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Normal);
            }
        }

        /// <summary>
        /// Handles the mouse moved event by updating the camera's orientation and position based on mouse movement.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments containing information about the mouse movement.</param>
        protected override void MouseMoved(object? sender, MouseMovedEventArgs e)
        {
            if (ActiveCamera is not Camera3d camera) return;

            // If the mouse initial location has not been set, 
            // set it to the current mouse position
            if (!_mouseOffsetPositionSet)
            {
                _mouseOffsetPosition = e.Position;
                _mouseInitialPosition = e.Position;
                _mouseOffsetPositionSet = true;
                _mouseLastY = e.Position.Y;
                _mouseLastX = e.Position.X;
                _cameraRotationOffset = camera.Rotation;
            }

            // Perform the appropriate rotation and/or movement based on the mouse mode
            if (_mouseMode == MouseModeEnum.YawAndPitch)
                YawAndPitch(e, camera);
            else if (_mouseMode == MouseModeEnum.MoveZAndYaw)
                MoveZAndYaw(e, camera);
            else if (_mouseMode == MouseModeEnum.MoveXAndY)
                MoveXAndY(e, camera);

            // Reset the mouse position to the center of the screen
            // when the mouse cursor reaches the edge of the screen
            if (_mouseMode != MouseModeEnum.None &&
                (e.Position.X >= e.Window.Size.Width || e.Position.X <= 0 ||
                 e.Position.Y >= e.Window.Size.Height || e.Position.Y <= 0))
            {
                Glfw.SetCursorPosition(e.Window.Handle, e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                _mouseOffsetPosition = new(e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                _cameraRotationOffset = camera.Rotation;
            }
        }

        /// <summary>
        /// Updates the camera's orientation based on yaw and pitch angles.
        /// </summary>
        /// <param name="e">The mouse movement event arguments.</param>
        /// <param name="camera">The camera to be updated.</param>
        private void YawAndPitch(MouseMovedEventArgs e, Camera3d camera)
        {
            // Calculate the yaw and pitch angles
            float yaw = CalculateYaw(e);
            float pitch = CalculatePitch(e);

            // Update the yaw and pitch angles
            camera.Rotation = new(yaw, pitch, camera.Rotation.Roll);
        }

        /// <summary>
        /// Moves the camera along the Z-axis and updates its yaw angle.
        /// </summary>
        /// <param name="e">The mouse movement event arguments.</param>
        /// <param name="camera">The camera to be updated.</param>
        private void MoveZAndYaw(MouseMovedEventArgs e, Camera3d camera)
        {
            // Calculate the yaw angle
            float yaw = CalculateYaw(e);

            // Update the yaw angle
            camera.Rotation = new(yaw, camera.Rotation.Pitch, camera.Rotation.Roll);

            // Temporarily set the pitch angle to zero
            // since this mode is ignoring the pitch angle
            Rotation originalRotation = camera.Rotation;
            camera.Rotation = new(camera.Rotation.Yaw, 0, camera.Rotation.Roll);

            // Calculate the direction to move the camera
            Vector3 direction = Vector3.Zero;

            if (e.Position.Y > _mouseLastY)
                direction += camera.Rotation.GetForwardVector();
            else if (e.Position.Y < _mouseLastY)
                direction += camera.Rotation.GetBackwardVector();

            // Reset the pitch angle
            camera.Rotation = originalRotation;

            // Move the camera
            float movementSpeed = Speed * MouseSpeedFactor;
            camera.Position += direction * movementSpeed;

            // Update the last mouse position
            _mouseLastY = e.Position.Y;
        }

        /// <summary>
        /// Moves the camera along the X and Y axes based on mouse movement.
        /// </summary>
        /// <param name="e">The mouse movement event arguments.</param>
        /// <param name="camera">The camera to be updated.</param>
        private void MoveXAndY(MouseMovedEventArgs e, Camera3d camera)
        {
            // Temporarily set the pitch angle to zero since this
            // mode is ignoring the pitch angle
            Rotation originalRotation = camera.Rotation;
            camera.Rotation = new(camera.Rotation.Yaw, 0, camera.Rotation.Roll);

            // Calculate the direction to move the camera
            Vector3 direction = Vector3.Zero;

            if (e.Position.X > _mouseLastX)
                direction += camera.Rotation.GetRightVector();
            else if (e.Position.X < _mouseLastX)
                direction += camera.Rotation.GetLeftVector();

            if (e.Position.Y > _mouseLastY)
                direction -= camera.Rotation.GetUpVector();
            else if (e.Position.Y < _mouseLastY)
                direction -= camera.Rotation.GetDownVector();

            // Reset the pitch angle
            camera.Rotation = originalRotation;

            // Move the camera
            float movementSpeed = Speed * MouseSpeedFactor;
            camera.Position += direction * movementSpeed;

            // Update the last mouse position
            _mouseLastX = e.Position.X;
            _mouseLastY = e.Position.Y;
        }

        /// <summary>
        /// Calculates the yaw angle based on mouse movement.
        /// </summary>
        /// <param name="e">The mouse movement event arguments.</param>
        /// <returns>The calculated yaw angle.</returns>
        private float CalculateYaw(MouseMovedEventArgs e)
        {
            float xDiff = (e.Position.X - _mouseOffsetPosition.X) / (e.Window.Size.Width / 2f) * MouseSensitivity;
            return _cameraRotationOffset.Yaw - (xDiff * 90);
        }

        /// <summary>
        /// Calculates the pitch angle based on mouse movement.
        /// </summary>
        /// <param name="e">The mouse movement event arguments.</param>
        /// <returns>The calculated pitch angle.</returns>
        private float CalculatePitch(MouseMovedEventArgs e)
        {
            float yDiff = (e.Position.Y - _mouseOffsetPosition.Y) / (e.Window.Size.Height / 2f) * MouseSensitivity;
            return _cameraRotationOffset.Pitch - (yDiff * 90);
        }

    }
}