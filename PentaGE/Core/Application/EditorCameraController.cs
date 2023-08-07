using GLFW;
using PentaGE.Common;
using PentaGE.Core.Events;
using PentaGE.Core.Rendering;
using PentaGE.Maths;
using System.Drawing;
using System.Numerics;

namespace PentaGE.Core
{
    public sealed class EditorCameraController : CameraController
    {
        #region Fields

        private Vector3 _positionAxes = Vector3.Zero;
        private Vector3 _rotationAxes = Vector3.Zero;
        private float _fieldOfViewDirection = 0f;

        private Vector3? _targetPosition = null;
        private Rotation? _targetRotation = null;
        private float? _targetFieldOfView = null;
        private Vector3? _orbitTarget = null;

        private bool _alternateMode = false;

        private MouseModeEnum _mouseMode = MouseModeEnum.None;
        private Point _mouseInitialLocation = new(0, 0);
        private bool _mouseInitialLocationSet = false;
        private Rotation _initialRotation = new(0, 0, 0);
        private int _mouseLastY = 0;
        private int _mouseLastX = 0;

        #endregion

        private enum MouseModeEnum
        {
            None = -1,
            YawAndPitch = 0,
            MoveZAndYaw = 1,
            MoveXAndY = 2,
        }

        public float Speed { get; set; } = 5f;

        public float MouseSpeedFactor { get; set; } = 0.002f;

        public float RotationSpeed { get; set; } = 90f;

        public float FieldOfViewSpeed { get; set; } = 50f;

        public float MouseSensitivity { get; set; } = 1f;

        public EditorCameraController()
        {

        }

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

        protected override void MouseDown(object? sender, Events.MouseButtonEventArgs e)
        {
            if (e.Button == Common.MouseButton.Left && _mouseMode == MouseModeEnum.None)
            {
                _mouseInitialLocationSet = false;
                _mouseMode = MouseModeEnum.YawAndPitch;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Hidden);
            }
            else if (e.Button == Common.MouseButton.Right && _mouseMode == MouseModeEnum.None)
            {
                _mouseInitialLocationSet = false;
                _mouseMode = MouseModeEnum.MoveZAndYaw;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Hidden);
            }
            else if (e.Button == Common.MouseButton.Middle)
            {
                _mouseInitialLocationSet = false;
                _mouseMode = MouseModeEnum.MoveXAndY;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Hidden);
            }
        }

        protected override void MouseUp(object? sender, Events.MouseButtonEventArgs e)
        {
            if (e.Button == Common.MouseButton.Left && _mouseMode == MouseModeEnum.YawAndPitch)
            {
                _mouseMode = MouseModeEnum.None;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Normal);
            }
            else if (e.Button == Common.MouseButton.Right && _mouseMode == MouseModeEnum.MoveZAndYaw)
            {
                _mouseMode = MouseModeEnum.None;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Normal);
            }
            else if (e.Button == Common.MouseButton.Middle && _mouseMode == MouseModeEnum.MoveXAndY)
            {
                _mouseMode = MouseModeEnum.None;
                Glfw.SetInputMode(e.Window.Handle, InputMode.Cursor, (int)CursorMode.Normal);
            }
        }

        protected override void MouseMoved(object? sender, MouseMovedEventArgs e)
        {
            if (ActiveCamera is not Camera3d camera) return;

            // If the mouse initial location has not been set, 
            // set it to the current mouse position
            if (!_mouseInitialLocationSet)
            {
                _mouseInitialLocation = e.Position;
                _mouseInitialLocationSet = true;
                _mouseLastY = e.Position.Y;
                _mouseLastX = e.Position.X;
                _initialRotation = camera.Rotation;
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
            if (e.Position.X > e.Window.Size.Width || e.Position.X < 0 ||
                e.Position.Y > e.Window.Size.Height || e.Position.Y < 0)
            {
                Glfw.SetCursorPosition(e.Window.Handle, e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                _mouseInitialLocation = new(e.Window.Size.Width / 2, e.Window.Size.Height / 2);
                _initialRotation = camera.Rotation;
            }
        }

        private void YawAndPitch(MouseMovedEventArgs e, Camera3d camera)
        {
            // Calculate the yaw and pitch angles
            float yaw = CalculateYaw(e);
            float pitch = CalculatePitch(e);

            // Update the yaw and pitch angles
            camera.Rotation = new(yaw, pitch, camera.Rotation.Roll);
        }

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

        private float CalculateYaw(MouseMovedEventArgs e)
        {
            float xDiff = (e.Position.X - _mouseInitialLocation.X) / (e.Window.Size.Width / 2f) * MouseSensitivity;
            return _initialRotation.Yaw - (xDiff * 90);
        }

        private float CalculatePitch(MouseMovedEventArgs e)
        {
            float yDiff = (e.Position.Y - _mouseInitialLocation.Y) / (e.Window.Size.Height / 2f) * MouseSensitivity;
            return _initialRotation.Pitch - (yDiff * 90);
        }

    }
}