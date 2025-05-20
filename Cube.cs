namespace Tetris
{
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;
    using System;
    using Tetris.enums;
    using Tetris.Interfaces;

    /// <summary>
    /// Представлява кубче в 3D Тетрис, което се визуализира чрез текстура.
    /// Наследява Shape и имплементира IVertexDataProvider.
    /// </summary>
    public class Cube : Shape, IVertexDataProvider
    {
        private float sinceDestroyed = 0f;
        private Vector3 vecposition;
        private string texturepath;

        /// <summary>Текстурата, използвана за този куб.</summary>
        public Texture tetrominoTexture;

        /// <summary>Моделна матрица за трансформации на куба.</summary>
        public Matrix4 model;

        /// <summary>Координати на върховете на куба с текстурни координати.</summary>
        // Статични върхове: позиция + текстурни координати (x, y, z, u, v)
        public static readonly float[] vertices = [
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
         0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        ];

        /// <summary>
        /// Конструктор за Cube.
        /// Инициализира шейдъра, текстурата и позицията.
        /// </summary>
        public Cube(float x, float y, float z, string texture)
            : base(x, y, z)
        {
            if (GlobalShader == null)
                GlobalShader = new Shader(Game.ProjectPlace + @"\shaders\Cube.vert", Game.ProjectPlace + @"\shaders\Cube.frag");

            State = ShapeState.Empty;
            texturepath = Game.ProjectPlace + @"\res\" + texture;
            tetrominoTexture = new Texture(texturepath);
            UpdateModelMatrix();
        }

        /// <summary>
        /// Актуализира матрицата за трансформации спрямо текущата позиция.
        /// </summary>
        private void UpdateModelMatrix()
        {
            vecposition = new Vector3(X, Y, Z);
            model = GameMath.TransformMatrix(vecposition, 2.0f, 2.0f, 2.0f);
        }

        /// <summary>
        /// Прилага текстура в зависимост от подадения цвят.
        /// </summary>
        public override void ApplyAppearance(TetrominoColor color)
        {
            AssignedColor = color;

            string texture = color switch
            {
                TetrominoColor.Blue => "element_blue_square_glossy.png",
                TetrominoColor.Red => "element_red_square_glossy.png",
                TetrominoColor.Green => "element_green_square_glossy.png",
                TetrominoColor.Yellow => "element_yellow_square_glossy.png",
                TetrominoColor.Purple => "element_purple_cube_glossy.png",
                _ => "element_grey_square_glossy.png"
            };

            texturepath = Game.ProjectPlace + @"\res\" + texture;
        }

        /// <summary>
        /// Задава визуално състояние като "видимо" и създава текстура, ако съществува такава.
        /// </summary>
        public override void SetVisible()
        {
            base.SetVisible();
            if (!string.IsNullOrWhiteSpace(texturepath))
                tetrominoTexture.Create(texturepath);
        }

        /// <summary>
        /// Копира визуалните свойства от друг обект от тип Cube.
        /// </summary>
        public override void CopyAppearanceFrom(Shape other)
        {
            if (other is Cube otherCube)
            {
                AssignedColor = otherCube.AssignedColor;
                texturepath = otherCube.texturepath;
                tetrominoTexture = new Texture(texturepath);
            }
        }

        /// <summary>
        /// Рендира куба с текущата текстура и шейдър.
        /// Ако е в състояние на унищожаване – визуализира се в сиво и се изтрива.
        /// </summary>
        public override void Render(float deltaTime)
        {
            if (Destroyed)
            {
                Playground.tetromino.IsDown = false;
                tetrominoTexture.Create(Game.ProjectPlace + @"\res\element_grey_square_glossy.png");
                sinceDestroyed += deltaTime;

                if (sinceDestroyed >= 0.4f)
                {
                    Playground.NotDestroyed = false;
                    State = ShapeState.Empty;
                    Destroyed = false;
                    sinceDestroyed = 0f;
                }
            }

            if (State == ShapeState.Empty && !Destroyed) return;
            if (tetrominoTexture == null) return;

            tetrominoTexture.Use();
            GlobalShader.SetMatrix4(ref model, "model");
            GlobalShader.SetFloat("minDepth", 0.0f);
            GlobalShader.SetFloat("maxDepth", 10.0f);

            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length);
        }

        /// <summary>
        /// Cube не поддържа динамично скалиране не е разработено.
        /// </summary>
        public override void Resize(float scale)
        {
            throw new NotSupportedException("Resize is not supported for Cube.");
        }

        /// <summary>
        /// Връща координатите на върховете за OpenGL буфериране.
        /// </summary>
        public float[] GetVertices()
        {
            return vertices;
        }

        /// <summary>
        /// Определя цвета на куба по текущият път до текстурата.
        /// </summary>
        public TetrominoColor GetColorFromTexture()
        {
            if (texturepath.Contains("blue")) return TetrominoColor.Blue;
            if (texturepath.Contains("red")) return TetrominoColor.Red;
            if (texturepath.Contains("green")) return TetrominoColor.Green;
            if (texturepath.Contains("yellow")) return TetrominoColor.Yellow;
            if (texturepath.Contains("purple")) return TetrominoColor.Purple;
            return TetrominoColor.Blue;
        }
    }
}

