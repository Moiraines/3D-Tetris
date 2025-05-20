namespace Tetris
{
    /// <summary>
    /// Статичен клас, съдържащ събития, свързани с поведението на играта.
    /// Използва се за известяване, когато тетромино кацне.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// Делегат за събитие, което приема тетромино като аргумент.
        /// </summary>
        public delegate void TetrominoLandedHandler(Tetromino tetromino);

        /// <summary>
        /// Събитие, което се вдига при кацане на тетромино.
        /// Абонати могат да се закачат и да реагират на него.
        /// </summary>
        public static event TetrominoLandedHandler OnTetrominoLanded;

        /// <summary>
        /// Активира събитието при кацане на тетромино.
        /// </summary>
        public static void RaiseTetrominoLanded(Tetromino tetromino)
        {
            OnTetrominoLanded?.Invoke(tetromino);
        }
    }
}