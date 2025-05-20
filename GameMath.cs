namespace Tetris
{
    using OpenTK.Mathematics;

    /// <summary>
    /// Статичен клас с помощни методи за 3D трансформации.
    /// Използва се при изчисление на Model матрици за обектите.
    /// </summary>
    public static class GameMath
    {
        /// <summary>
        /// Създава трансформационна матрица от мащабиране, въртене и транслация.
        /// </summary>
        /// <param name="translation">Позиция на обекта в пространството.</param>
        /// <param name="scalex">Мащаб по X.</param>
        /// <param name="scaley">Мащаб по Y.</param>
        /// <param name="scalez">Мащаб по Z.</param>
        /// <param name="anglex">Ъгъл на ротация по X (в градуси).</param>
        /// <param name="angley">Ъгъл на ротация по Y (в градуси).</param>
        /// <param name="anglez">Ъгъл на ротация по Z (в градуси).</param>
        /// <returns>Готова 4x4 трансформационна матрица.</returns>
        public static Matrix4 TransformMatrix(
            Vector3 translation,
            float scalex = 1.0f,
            float scaley = 1.0f,
            float scalez = 1.0f,
            float anglex = 0.0f,
            float angley = 0.0f,
            float anglez = 0.0f)
        {
            var rotation =
                Matrix4.CreateRotationX(MathHelper.DegreesToRadians(anglex)) *
                Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angley)) *
                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(anglez));

            var model = Matrix4.CreateTranslation(translation);
            model *= Matrix4.CreateScale(new Vector3(scalex, scaley, scalez));
            model *= rotation;

            return model;
        }
    }
}