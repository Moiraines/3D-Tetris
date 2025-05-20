namespace Tetris
{
    using System;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    /// <summary>
    /// Управлява позицията, движението, ротацията и ефектите на камерата.
    /// Поддържа свободен и орбитален режим.
    /// </summary>
    public class Camera
    {
        // === Параметри и начални стойности ===
        private readonly float cameraSpeed = 8.5f;
        private readonly float orbitRadius = 50.0f;
        private readonly Vector3 initialPosition = new(5, 45, 55);
        private readonly Vector3 initialForward = new(0, -1, 0);
        private readonly Vector3 initialRight = new(1, 0, 0);

        private float sensitivity = 0.05f;
        private bool isOrbiting = false;
        private bool canToggleOrbit = true;
        private float lastToggleTime = 0.0f;

        // === Разклащане на камерата (Shake ефект) ===
        private float shakeDuration = 0f;
        private float shakeMagnitude = 0.1f;
        private Vector3 shakeOffset = Vector3.Zero;

        // === Ротация чрез мишка ===
        private Vector2 lastMousePos = Vector2.Zero;
        private float yaw = -90.0f, pitch = -40.0f;

        // === Позиция и ориентация ===
        private Vector3 target = new(10, 10, 10);
        private Vector3 position;
        private Vector3 forward;
        private Vector3 right;

        // === Матрици за трансформация (глобални) ===
        public static Matrix4 view;
        public static Matrix4 projection;

        /// <summary>
        /// Създава нова камера с начална позиция.
        /// </summary>
        public Camera()
        {
            right = initialRight;
            forward = initialForward;
            position = initialPosition;
        }

        /// <summary>
        /// Обновява позицията и матриците на камерата.
        /// Обработва орбитална логика и разклащане (shake).
        /// </summary>
        public void Update()
        {
            if (isOrbiting)
            {
                Vector3 orbitTarget = target + new Vector3(10, 0, 0);
                Vector3 offset = new(
                    (float)(orbitRadius * Math.Cos(MathHelper.DegreesToRadians(yaw + 180.0f)) * Math.Cos(MathHelper.DegreesToRadians(pitch))),
                    (float)(orbitRadius * Math.Sin(MathHelper.DegreesToRadians(pitch))) + 50.0f,
                    (float)(orbitRadius * Math.Sin(MathHelper.DegreesToRadians(yaw + 180.0f)) * Math.Cos(MathHelper.DegreesToRadians(pitch)))
                );
                position = target + offset;
                forward = Vector3.Normalize(target - position);
            }
            else
            {
                forward = new Vector3(
                    (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw)),
                    (float)Math.Sin(MathHelper.DegreesToRadians(pitch)),
                    (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw))
                );
                forward = Vector3.Normalize(forward);
            }

            // Shake ефект
            if (shakeDuration > 0)
            {
                float t = shakeDuration;
                shakeOffset = new Vector3(
                    MathF.Sin(t * 50f) * shakeMagnitude,
                    MathF.Cos(t * 70f) * shakeMagnitude,
                    MathF.Sin(t * 90f) * shakeMagnitude * 0.5f
                );
                shakeDuration -= 0.016f;
            }
            else
            {
                shakeOffset = Vector3.Zero;
            }

            view = Matrix4.LookAt(position + shakeOffset, position + forward + shakeOffset, Vector3.UnitY);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Game.WIDTH / Game.HEIGHT, 0.1f, 100.0f);
        }

        /// <summary>
        /// Обработва въвеждането от клавиатурата и мишката – ротация, движение, превключване на орбитален режим.
        /// </summary>
        public void HandleInput(KeyboardState input, MouseState mouseState, float deltaTime, bool isFocused)
        {
            if (!isFocused) return;

            Move(input, deltaTime);
            float currentTime = (float)GLFW.GetTime();

            // Превключване на орбитален режим със среден бутон
            if (mouseState.IsButtonDown(MouseButton.Middle) && canToggleOrbit && (currentTime - lastToggleTime) > 0.2f)
            {
                isOrbiting = !isOrbiting;
                canToggleOrbit = false;
                lastToggleTime = currentTime;

                if (!isOrbiting) Reset();
                Console.WriteLine("Orbit Mode: " + (isOrbiting ? "ON" : "OFF"));
            }
            if (!mouseState.IsButtonDown(MouseButton.Middle)) canToggleOrbit = true;

            // Въртене с ляв бутон (при орбитален режим)
            if (mouseState.IsButtonDown(MouseButton.Left) && isOrbiting)
            {
                Vector2 mouse = new(mouseState.X, mouseState.Y);
                if (!mouseState.WasButtonDown(MouseButton.Left)) lastMousePos = mouse;
                float deltaX = mouse.X - lastMousePos.X;
                float deltaY = lastMousePos.Y - mouse.Y;
                lastMousePos = mouse;
                Rotate(deltaY * sensitivity, deltaX * sensitivity);
            }

            // Въртене с десен бутон (при свободен режим)
            if (mouseState.IsButtonDown(MouseButton.Right) && !isOrbiting)
            {
                Vector2 mouse = new(mouseState.X, mouseState.Y);
                if (!mouseState.WasButtonDown(MouseButton.Right)) lastMousePos = mouse;
                float deltaX = mouse.X - lastMousePos.X;
                float deltaY = lastMousePos.Y - mouse.Y;
                lastMousePos = mouse;
                Rotate(deltaY * sensitivity, deltaX * sensitivity);
                Update();
            }
        }

        /// <summary>
        /// Движи камерата според натиснати клавиши – W, A, S, D, Shift, F.
        /// </summary>
        public void Move(KeyboardState input, float deltaTime)
        {
            Vector3 moveDirection = Vector3.Zero;

            switch (KeyPressed(input))
            {
                case Keys.A: moveDirection -= Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY)) * cameraSpeed * deltaTime; break;
                case Keys.D: moveDirection += Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY)) * cameraSpeed * deltaTime; break;
                case Keys.W: moveDirection += forward * cameraSpeed * deltaTime; break;
                case Keys.S: moveDirection -= forward * cameraSpeed * deltaTime; break;
                case Keys.LeftShift: moveDirection -= Vector3.UnitY * cameraSpeed * deltaTime; break;
                case Keys.F: moveDirection += Vector3.UnitY * cameraSpeed * deltaTime; break;
                case Keys.Space: Tetromino.TimeDown = 0.04f; break;
            }

            position += moveDirection;
            if (isOrbiting) target += moveDirection;
        }

        /// <summary>Ротира камерата по yaw и pitch, със зададен offset.</summary>
        public void Rotate(float pitchOffset, float yawOffset)
        {
            pitch += pitchOffset;
            yaw += yawOffset;
            pitch = Math.Clamp(pitch, -89.0f, 89.0f);
        }

        /// <summary>Нулира камерата до начална позиция и ориентация.</summary>
        public void Reset()
        {
            position = initialPosition;
            forward = initialForward;
            right = initialRight;
            yaw = -90.0f;
            pitch = -40.0f;
            isOrbiting = false;
        }

        /// <summary>Активира разклащане на камерата (shake ефект).</summary>
        public void Shake(float duration = 0.2f, float magnitude = 0.2f)
        {
            shakeDuration = duration;
            shakeMagnitude = magnitude;
        }

        /// <summary>Помощна функция: Проверява кой клавиш е натиснат.</summary>
        private static Keys? KeyPressed(KeyboardState state)
        {
            Keys? key = null;
            void DownPressed(Keys testKey) { if (state.IsKeyDown(testKey)) key = testKey; }
            if (!state.IsAnyKeyDown) return null;

            DownPressed(Keys.A);
            DownPressed(Keys.W);
            DownPressed(Keys.S);
            DownPressed(Keys.D);
            DownPressed(Keys.LeftShift);
            DownPressed(Keys.F);
            DownPressed(Keys.Space);

            return key;
        }
    }
}