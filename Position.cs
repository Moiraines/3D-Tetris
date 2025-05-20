namespace Tetris
{
    /// <summary>
    /// Прост клас за съхранение на позиция в 3D пространството.
    /// Използва се за координати на Shape обекти, без допълнителни зависимости като Vector3.
    /// </summary>
    public class Position
    {
        /// <summary>
        /// X координата.
        /// </summary>
        public float x;

        /// <summary>
        /// Y координата.
        /// </summary>
        public float y;

        /// <summary>
        /// Z координата.
        /// </summary>
        public float z;

        /// <summary>
        /// Конструктор с начални координати.
        /// </summary>
        /// <param name="x">X координата</param>
        /// <param name="y">Y координата</param>
        /// <param name="z">Z координата</param>
        public Position(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}