namespace Tetris
{
    using System;
    using Tetris.enums;
    using Tetris.Interfaces;

    /// <summary>
    /// Абстрактен клас Shape, който дефинира базовите свойства и методи за всички фигури.
    /// Използва се като основа за конкретни имплементации като Cube и Sphere.
    /// </summary>
    public abstract class Shape : IStatefulShape
    {
        // Публични свойства

        /// <summary>Координата X на формата в 3D пространството.</summary>
        public float X { get; set; }

        /// <summary>Координата Y на формата в 3D пространството.</summary>
        public float Y { get; set; }

        /// <summary>Координата Z на формата в 3D пространството.</summary>
        public float Z { get; set; }

        /// <summary>Обединена позиция като обект Position (x, y, z).</summary>
        public Position Position { get; set; }

        /// <summary>Флаг дали формата е унищожена (напр. за премахване).</summary>
        public bool Destroyed { get; set; }

        /// <summary>Състояние на формата – празна, пълна и т.н.</summary>
        public ShapeState State { get; set; }

        /// <summary>Цвят, асоцииран с формата.</summary>
        public TetrominoColor AssignedColor { get; set; }

        // 🟪 Статичен шейдър
        /// <summary>Глобален споделен шейдър, използван от всички форми.</summary>
        public static Shader GlobalShader;

        // 🟩 Конструктор

        /// <summary>
        /// Създава нова форма с начални координати.
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="z">Координата Z</param>
        protected Shape(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            Position = new Position(x, y, z);
        }

        // Основни методи

        /// <summary>
        /// Премества формата с определено разстояние по всяка ос.
        /// </summary>
        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            X += deltaX;
            Y += deltaY;
            Z += deltaZ;

            Position.x = X;
            Position.y = Y;
            Position.z = Z;
        }

        /// <summary>
        /// Задава формата да бъде видима (променя състоянието ѝ на Solid).
        /// </summary>
        public virtual void SetVisible()
        {
            State = ShapeState.Solid;
        }

        /// <summary>
        /// Завърта формата – по подразбиране извежда съобщение, трябва да се override-не.
        /// </summary>
        public virtual void Rotate()
        {
            Console.WriteLine("Rotate not implemented for this shape.");
        }

        /// <summary>
        /// Рендира формата, използвайки текущото време.
        /// </summary>
        public virtual void Render()
        {
            Render(0f);
        }

        // Абстрактни методи

        /// <summary>
        /// Рендира формата, използвайки подадено време (напр. за анимации).
        /// </summary>
        /// <param name="deltaTime">Изминало време от последния кадър</param>
        public abstract void Render(float deltaTime);

        /// <summary>
        /// Променя размера на формата с определен скейл.
        /// </summary>
        public abstract void Resize(float scale);

        /// <summary>
        /// Прилага цвят към формата или текстура взависимост от вида фигура.
        /// </summary>
        public abstract void ApplyAppearance(TetrominoColor color);

        /// <summary>
        /// Копира визуалните свойства от друга форма.
        /// </summary>
        public abstract void CopyAppearanceFrom(Shape other);
    }
}
