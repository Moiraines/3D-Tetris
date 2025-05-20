namespace Tetris
{
    using System.Drawing;

    /// <summary>
    /// Клас за зареждане и извличане на спрайтове (символи) от спрайт лист изображение.
    /// Използва се от класа Text за визуализация на текст чрез спрайтове.
    /// </summary>
    public class SpriteSheet
    {
        private readonly Bitmap spriteSheet;
        private readonly int spriteWidth, spriteHeight;
        private readonly int columns, rows;

        /// <summary>
        /// Конструктор, който зарежда спрайт лист от файл и изчислява колони и редове.
        /// </summary>
        /// <param name="relativePath">Път до .bmp файла (относителен спрямо проекта).</param>
        /// <param name="spriteWidth">Ширина на отделен спрайт (в пиксели).</param>
        /// <param name="spriteHeight">Височина на отделен спрайт (в пиксели).</param>
        public SpriteSheet(string relativePath, int spriteWidth, int spriteHeight)
        {
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;

            spriteSheet = new Bitmap(Game.ProjectPlace + relativePath);
            columns = spriteSheet.Width / spriteWidth;
            rows = spriteSheet.Height / spriteHeight;
        }

        /// <summary>
        /// Извлича определен спрайт от листа по индекс.
        /// </summary>
        /// <param name="index">Индекс на символа в спрайт листа.</param>
        /// <returns>Bitmap изображение със съответния символ или null, ако не съществува.</returns>
        public Bitmap GetImage(int index)
        {
            int currentIndex = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (currentIndex == index)
                    {
                        Rectangle section = new Rectangle(
                            col * spriteWidth,
                            row * spriteHeight,
                            spriteWidth,
                            spriteHeight
                        );
                        return Extract(spriteSheet, section);
                    }
                    currentIndex++;
                }
            }

            return null;
        }

        /// <summary>
        /// Изрязва определена част от изображение (Rect) и връща като нов Bitmap.
        /// </summary>
        /// <param name="source">Оригиналният Bitmap (спрайт лист).</param>
        /// <param name="section">Район за изрязване.</param>
        /// <returns>Изрязано изображение с точния символ.</returns>
        private static Bitmap Extract(Bitmap source, Rectangle section)
        {
            Bitmap result = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(source, new Rectangle(0, 0, section.Width, section.Height), section, GraphicsUnit.Pixel);
            }
            return result;
        }
    }
}