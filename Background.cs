using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace Tetris
{
    /// <summary>
    /// Клас за визуализиране на пода на игралното поле (фон).
    /// Използва VAO/VBO, текстура и специален шейдър.
    /// </summary>
    public class Background : IDisposable
    {
        private readonly int vao;
        private readonly int vbo;
        private Matrix4 floorModel;
        private readonly Texture floorTexture;

        // Статичен шейдър, споделян от всички Background обекти
        private static readonly Shader shader;

        /// <summary>
        /// Зареждане на статичния шейдър само веднъж при стартиране.
        /// </summary>
        static Background()
        {
            shader = new Shader(
                Game.ProjectPlace + @"\shaders\Background.vert",
                Game.ProjectPlace + @"\shaders\Background.frag"
            );
        }

        /// <summary>
        /// Конструктор, който създава пода с текстура и буфери.
        /// </summary>
        public Background()
        {
            floorTexture = new Texture(Game.ProjectPlace + @"\res\floor_texture.png");

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            float[] vertices = GetVertices();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Позиции
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Текстурни координати
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);

            // Транслация – премества пода леко надолу и встрани
            floorModel = Matrix4.CreateTranslation(-1f, 0f, -1f);
        }

        /// <summary>
        /// Рендира пода със зададената текстура и трансформации.
        /// </summary>
        public void Render()
        {
            shader.Use();
            shader.SetMatrix4(ref Camera.view, "view");
            shader.SetMatrix4(ref Camera.projection, "projection");
            shader.SetMatrix4(ref floorModel, "model");

            GL.ActiveTexture(TextureUnit.Texture0);
            floorTexture.Use();
            shader.SetInt("floorTexture", 0);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Създава върхове за пода – включително горна част, дъно и страни.
        /// Това създава визуално усещане за "обем" на пода.
        /// </summary>
        private float[] GetVertices()
        {
            float width = 20f;
            float depth = 20f;
            float height = 1f;
            float y = -1f;

            return new float[]
            {
                // Горна повърхност
                0, y, 0, 0, 0,    width, y, 0, 1, 0,    width, y, depth, 1, 1,
                0, y, depth, 0, 1,    0, y, 0, 0, 0,    width, y, depth, 1, 1,

                // Долна повърхност
                0, y - height, 0, 0, 0,    width, y - height, 0, 1, 0,    width, y - height, depth, 1, 1,
                0, y - height, depth, 0, 1,    0, y - height, 0, 0, 0,    width, y - height, depth, 1, 1,

                // Предна стена
                0, y, depth, 0, 0,    width, y, depth, 1, 0,    width, y - height, depth, 1, 1,
                0, y, depth, 0, 0,    width, y - height, depth, 1, 1,    0, y - height, depth, 0, 1,

                // Задна стена
                0, y, 0, 0, 0,    width, y, 0, 1, 0,    width, y - height, 0, 1, 1,
                0, y, 0, 0, 0,    width, y - height, 0, 1, 1,    0, y - height, 0, 0, 1,

                // Лява стена
                0, y, 0, 0, 0,    0, y, depth, 1, 0,    0, y - height, depth, 1, 1,
                0, y, 0, 0, 0,    0, y - height, depth, 1, 1,    0, y - height, 0, 0, 1,

                // Дясна стена
                width, y, 0, 0, 0,    width, y, depth, 1, 0,    width, y - height, depth, 1, 1,
                width, y, 0, 0, 0,    width, y - height, depth, 1, 1,    width, y - height, 0, 0, 1
            };
        }

        /// <summary>
        /// Освобождаване на ресурси (VAO, VBO и текстура).
        /// </summary>
        public void Dispose()
        {
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            floorTexture.Dispose();
        }
    }
}