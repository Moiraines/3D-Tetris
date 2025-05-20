namespace Tetris
{
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;
    using System;
    using System.Collections.Generic;
    using Tetris.enums;
    using Tetris.Interfaces;

    /// <summary>
    /// Представлява сферична фигура, която се рендира с цвят чрез шейдъри.
    /// Използва се за демонстрация на полиморфизъм и поддръжка на различни форми.
    /// </summary>
    public class Sphere : Shape, IVertexDataProvider
    {
        private float sinceDestroyed = 0f;

        /// <summary>Брой ленти по ширина (latitude).</summary>
        private readonly int latitudeBands = 48;

        /// <summary>Брой ленти по дължина (longitude).</summary>
        private readonly int longitudeBands = 48;

        /// <summary>Радиус на сферата.</summary>
        private float radius = 0.5f;

        /// <summary>Цвят на сферата, зададен при ApplyAppearance().</summary>
        private Vector3 color = new Vector3(0.8f, 0.5f, 0.2f); // default orange

        /// <summary>Матрица за позициониране в 3D пространството.</summary>
        private Matrix4 model;

        /// <summary>
        /// Конструктор за Sphere. Инициализира позиция, шейдър и начален цвят.
        /// </summary>
        public Sphere(float x, float y, float z)
            : base(x, y, z)
        {
            if (GlobalShader == null)
                GlobalShader = new Shader(Game.ProjectPlace + @"\shaders\Sphere.vert", Game.ProjectPlace + @"\shaders\Sphere.frag");

            model = GameMath.TransformMatrix(new Vector3(x, y, z), 2.0f, 2.0f, 2.0f);
            State = ShapeState.Empty;
        }

        /// <summary>
        /// Задава фигурата като видима (промяна на състоянието). Няма визуална логика тук.
        /// </summary>
        public override void SetVisible()
        {
            base.SetVisible();
        }

        /// <summary>
        /// Рендира сферата. Ако е в процес на унищожение, чака ~0.4 сек. преди да я изтрие.
        /// </summary>
        public override void Render(float deltaTime)
        {
            if (Destroyed)
            {
                Playground.tetromino.IsDown = false;
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

            GlobalShader.Use();
            GlobalShader.SetMatrix4(ref model, "model");
            GlobalShader.SetFloat("minDepth", 0.0f);
            GlobalShader.SetFloat("maxDepth", 10.0f);
            GlobalShader.SetVector3("color", color);

            float[] vertices = GetVertices();

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, vertices);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 3);
            GL.DisableClientState(ArrayCap.VertexArray);
        }

        /// <summary>
        /// Променя цвета на сферата според подадения TetrominoColor.
        /// </summary>
        public override void ApplyAppearance(TetrominoColor color)
        {
            AssignedColor = color;
            this.color = color switch
            {
                TetrominoColor.Blue => new Vector3(0.2f, 0.4f, 0.9f),
                TetrominoColor.Red => new Vector3(0.9f, 0.2f, 0.2f),
                TetrominoColor.Green => new Vector3(0.2f, 0.8f, 0.3f),
                TetrominoColor.Yellow => new Vector3(1.0f, 0.9f, 0.2f),
                TetrominoColor.Purple => new Vector3(0.6f, 0.3f, 0.7f),
                _ => new Vector3(0.8f, 0.5f, 0.2f)
            };
        }

        /// <summary>
        /// Копира цвета и вида от друга сфера.
        /// </summary>
        public override void CopyAppearanceFrom(Shape other)
        {
            if (other is Sphere sphere)
            {
                this.color = sphere.color;
                AssignedColor = sphere.AssignedColor;
            }
        }

        /// <summary>
        /// Променя размера на сферата чрез скалиране на радиуса.
        /// </summary>
        public override void Resize(float scale)
        {
            radius *= scale;
        }

        /// <summary>
        /// Генерира върховете на сферата на база latitude и longitude разпределение.
        /// </summary>
        public float[] GetVertices()
        {
            var data = new List<float>();

            for (int lat = 0; lat < latitudeBands; lat++)
            {
                float theta1 = lat * MathF.PI / latitudeBands;
                float theta2 = (lat + 1) * MathF.PI / latitudeBands;

                for (int lon = 0; lon <= longitudeBands; lon++)
                {
                    float phi = lon * 2 * MathF.PI / longitudeBands;

                    float x1 = MathF.Sin(theta1) * MathF.Cos(phi);
                    float y1 = MathF.Cos(theta1);
                    float z1 = MathF.Sin(theta1) * MathF.Sin(phi);

                    float x2 = MathF.Sin(theta2) * MathF.Cos(phi);
                    float y2 = MathF.Cos(theta2);
                    float z2 = MathF.Sin(theta2) * MathF.Sin(phi);

                    data.AddRange(new float[] { x1 * radius, y1 * radius, z1 * radius, x1, y1, z1 });
                    data.AddRange(new float[] { x2 * radius, y2 * radius, z2 * radius, x2, y2, z2 });
                }
            }

            return data.ToArray();
        }
    }
}
