namespace Tetris
{
    using System;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using OpenTK.Windowing.Common;
    using Tetris.enums;

    /// <summary>
    /// Главен клас за управление на прозореца на играта.
    /// Наследява OpenTK.GameWindow и обработва логиката, визуализацията и събития.
    /// </summary>
    public class Game : GameWindow
    {
        // === Основни компоненти ===
        private Playground playground;
        private Background background;
        private readonly Camera camera;

        // Следваща фигура (визуализация)
        private readonly Shape[] nextUpShapes = new Shape[4];
        private Position[] showTetrominoPosition = new Position[4];

        // Текстови елементи
        public Text Score;
        public Text Level;
        public Text Lines;
        public Text NextUpTetromino;

        // Статични данни за ниво, точки и размер на прозореца
        public static Text score;
        public static Text level;
        public static Text lines;

        public static int points = 0;
        public static int currentlevel = 1;
        public static int linesCleared = 0;
        public static float WIDTH;
        public static float HEIGHT;
        public static bool showNextTetromino = false;

        /// <summary>Път към проекта, използван за зареждане на ресурси.</summary>
        public static string ProjectPlace => AppDomain.CurrentDomain.BaseDirectory;


        /// <summary>
        /// Създава прозореца на играта и регистрира събития.
        /// </summary>
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            WIDTH = ClientSize.X;
            HEIGHT = ClientSize.Y;
            camera = new Camera();

            GameEvents.OnTetrominoLanded += t =>
            {
                Console.WriteLine($"Тетромино от тип {t.Kind} кацна.");
                camera.Shake(0.5f, 0.3f);
            };
        }

        /// <summary>
        /// Зарежда начални обекти, цветове, текстове и активира OpenGL дълбочина.
        /// </summary>
        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f, 0f, 1.0f);
            playground = new Playground();
            background = new Background();

            Level = new Text("Level", Vector3.Zero, new Vector3(0.7f, 0.7f, 0.7f));
            Lines = new Text("Lines", Vector3.Zero, new Vector3(0.7f, 0.7f, 0.7f));
            Score = new Text("Score", Vector3.Zero, new Vector3(0.7f, 0.7f, 0.7f));
            NextUpTetromino = new Text("Next up:", Vector3.Zero, new Vector3(0.7f, 0.7f, 0.7f));

            CalculateTextPositions();
            GL.Enable(EnableCap.DepthTest);

            base.OnLoad();
        }

        /// <summary>Преизчислява позициите на всички текстове при промяна в размера.</summary>
        private void CalculateTextPositions()
        {
            float textLeftX = -((WIDTH / 2f) * 0.01f);
            float textStartY = (HEIGHT / 2f) * 0.025f;
            float lineSpacing = textStartY * 0.5f;
            float valueOffsetY = textStartY - (lineSpacing * 0.3f);

            Level.Position = new Vector3(textLeftX, textStartY, 0);
            Lines.Position = new Vector3(textLeftX, (float)(textStartY - (0.9 * lineSpacing)), 0);
            Score.Position = new Vector3(textLeftX, (float)(textStartY - 1.8 * lineSpacing), 0);

            level.Position = new Vector3(textLeftX, valueOffsetY, 0);
            lines.Position = new Vector3(textLeftX, (float)(valueOffsetY - 0.9 * lineSpacing), 0);
            score.Position = new Vector3(textLeftX, (float)(valueOffsetY - 1.8 * lineSpacing), 0);

            NextUpTetromino.Position = new Vector3(textLeftX, (float)(valueOffsetY - 2.5 * lineSpacing), 0);
        }

        /// <summary>Рендерира всички текстови обекти на екрана.</summary>
        private void RenderAllText()
        {
            GL.BindVertexArray(Text.Vao);
            score.Render();
            Level.Render();
            Score.Render();
            Lines.Render();
            lines.Render();
            level.Render();
            NextUpTetromino.Render();
        }

        /// <summary>Обновява viewport и позициите на елементите при resize.</summary>
        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            WIDTH = ClientSize.X;
            HEIGHT = ClientSize.Y;
            CalculateTextPositions();
            base.OnResize(e);
        }

        protected override void OnUnload() => base.OnUnload();

        /// <summary>Обновява камерата, управлението и логиката на играта.</summary>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (!IsFocused) return;

            camera.Update();
            playground.Update();
            camera.HandleInput(KeyboardState, MouseState, (float)e.Time, IsFocused);
        }

        /// <summary>Извършва цялото рендериране: фон, текстове, фигури.</summary>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            background.Render();
            RenderAllText();
            playground.Render((float)e.Time);
            ShowNextUpTetromino();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        /// <summary>Обработва натискане на клавиши: движение, въртене, рестарт, save/load, статистика.</summary>
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            Playground.tetromino.Move(e.Key);

            float rotationAngle = (float)Math.PI / 2;
            if (KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl))
                rotationAngle = -rotationAngle;

            if (e.Key == Keys.Enter && Playground.gameover)
                playground = new Playground();

            if (e.Key == Keys.X) Playground.tetromino.Rotate(rotationAngle, Axis.X);
            if (e.Key == Keys.C) Playground.tetromino.Rotate(rotationAngle, Axis.Y);
            if (e.Key == Keys.Z) Playground.tetromino.Rotate(rotationAngle, Axis.Z);
            if (e.Key == Keys.R) camera.Reset();

            if (e.Key == Keys.F2)
            {
                playground.SaveToFile("save.json");
                Console.WriteLine("📝 Състоянието е записано в save.json");
            }
            if (e.Key == Keys.F3)
            {
                playground.LoadFromFile("save.json");
                Console.WriteLine("📂 Състоянието е заредено от save.json");
            }
            if (e.Key == Keys.F1)
            {
                playground.PrintLinqStats();
            }

            base.OnKeyDown(e);
        }

        /// <summary>Увеличава скоростта на падане докато се държи Space.</summary>
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.Space)
                Tetromino.TimeDown = Tetromino.LevelTimeDown();

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Визуализира следващата фигура (Next Up) встрани от игралното поле.
        /// </summary>
        public void ShowNextUpTetromino()
        {
            if (showNextTetromino)
            {
                showTetrominoPosition = Playground.NextPositions;
                for (int i = 0; i < showTetrominoPosition.Length; i++)
                {
                    float posX = Playground.NextPositions[i].x - 10;
                    float posY = Playground.NextPositions[i].y - 15;
                    showTetrominoPosition[i].x = posX;
                    showTetrominoPosition[i].y = posY;

                    nextUpShapes[i] = Playground.CreateShape(posX, posY, showTetrominoPosition[i].z);
                    nextUpShapes[i].ApplyAppearance(Playground.NextTetrominoColor);
                    nextUpShapes[i].SetVisible();
                }
                showNextTetromino = false;
            }

            GL.BindVertexArray(playground.GetVAO());
            foreach (var shape in nextUpShapes)
                shape.Render();
        }

        /// <summary>
        /// Проверява дали играчът е достигнал ново ниво спрямо точките.
        /// </summary>
        public static bool IsGivenLevel(int score, int level)
        {
            if (currentlevel == level && points >= score)
            {
                currentlevel++;
                Game.level.ChangeText(currentlevel.ToString());
                return true;
            }
            return false;
        }
    }
}