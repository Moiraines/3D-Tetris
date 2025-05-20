namespace Tetris
{
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Desktop;

    /// <summary>
    /// Класът Program съдържа входната точка на приложението.
    /// Стартира нова инстанция на играта с дефинирани настройки на прозореца.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Главният метод, от който започва изпълнението на програмата.
        /// Създава прозорец с определен размер и заглавие и стартира играта.
        /// </summary>
        public static void Main()
        {
            // Настройки за честота на кадрите и поведение на прозореца (напр. VSync)
            var gameSettings = new GameWindowSettings();

            // Настройки за прозореца – резолюция и заглавие
            var nativeSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(1024, 768),
                Title = "Tetris"
            };

            // Създаване и стартиране на играта
            using var game = new Game(gameSettings, nativeSettings);
            game.Run();
        }
    }
}