namespace Tetris
{
    using System.Drawing;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    /// <summary>
    /// Отговаря за рендирането на текст чрез спрайтове и OpenGL.
    /// Използва се за "Score", "Level", "Lines", както и други надписи.
    /// </summary>
    public class Text
    {
        // Квадрат (плосък) за рендер на всяка буква (позиция + текстурни координати)
        private static readonly float[] QuadVertices = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
        };

        // Букви и специални символи за поддръжка
        private static readonly char[] UpperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] SpecialCharacters = "0123456789.,;:$#_!\"/?%&()@".ToCharArray();

        // Спрайтове (SpriteSheets)
        private static readonly SpriteSheet BigLettersSheet = new(@"\res\BigLetters.bmp", 110, 120);
        private static readonly SpriteSheet SmallLettersSheet = new(@"\res\SmallLetters.bmp", 110, 120);
        private static readonly SpriteSheet CharactersSheet = new(@"\res\Characters.bmp", 110, 120);

        // OpenGL обекти
        private static int vbo, vao;
        private static Shader shader;

        // Данни за конкретния текст
        private readonly Texture textTexture = new();
        private Bitmap[] textImages;
        private Matrix4 modelMatrix;
        private Matrix4 projectionMatrix;
        private Vector3 scale = new(30f, 30f, 2f);

        private string content;
        private Vector3 position;
        private Vector3 color;

        /// <summary>
        /// Достъп до VAO (глобално).
        /// </summary>
        public static int Vao
        {
            get => vao;
            private set => vao = value;
        }

        /// <summary>
        /// Позиция, където се рендира текстът в 3D пространството.
        /// </summary>
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        /// <summary>
        /// Конструктор със стандартна скала.
        /// </summary>
        public Text(string text, Vector3 position, Vector3 color)
        {
            this.content = text;
            this.position = position;
            this.color = color;

            shader ??= new Shader(Game.ProjectPlace + @"\shaders\text.vert", Game.ProjectPlace + @"\shaders\text.frag");

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, QuadVertices.Length * sizeof(float), QuadVertices, BufferUsageHint.StreamDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            textImages = GenerateTextImages();
            FlipImages();
        }

        /// <summary>
        /// Конструктор с ръчно подадена скала.
        /// </summary>
        public Text(string text, Vector3 position, Vector3 color, Vector3 customScale)
            : this(text, position, color)
        {
            scale = customScale;
        }

        /// <summary>
        /// Завърта изображението, така че да е в правилна ориентация за OpenGL.
        /// </summary>
        private void FlipImages()
        {
            foreach (var image in textImages)
                image?.RotateFlip(RotateFlipType.Rotate180FlipX);
        }

        /// <summary>
        /// Създава изображение за всеки символ от текста, използвайки спрайтовете.
        /// </summary>
        private Bitmap[] GenerateTextImages()
        {
            Bitmap[] images = new Bitmap[content.Length];

            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                int? index;

                if ((index = IndexOf(UpperLetters, c)).HasValue)
                    images[i] = BigLettersSheet.GetImage(index.Value);
                else if ((index = IndexOf(ToLower(UpperLetters), c)).HasValue)
                    images[i] = SmallLettersSheet.GetImage(index.Value);
                else if ((index = IndexOf(SpecialCharacters, c)).HasValue)
                    images[i] = CharactersSheet.GetImage(index.Value);
            }

            return images;
        }

        private static char[] ToLower(char[] source)
        {
            char[] result = new char[source.Length];
            for (int i = 0; i < source.Length; i++)
                result[i] = char.ToLower(source[i]);
            return result;
        }

        private static int? IndexOf(char[] array, char target)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i] == target)
                    return i;
            return null;
        }

        /// <summary>
        /// Променя съдържанието на текста и го презарежда.
        /// </summary>
        public void ChangeText(string newText)
        {
            content = newText;
            textImages = GenerateTextImages();
            FlipImages();
        }

        /// <summary>
        /// Рендира текста на екрана, символ по символ.
        /// Всеки символ е текстуриран върху 3D квадрат.
        /// </summary>
        public void Render()
        {
            shader.Use();

            projectionMatrix = Matrix4.CreateOrthographic(Game.WIDTH, Game.HEIGHT, 0.1f, 100f);
            shader.SetMatrix4(ref projectionMatrix, "projection");
            shader.SetVectorToUniform(color, shader.GetUniformLocation("color"));

            textTexture.Use();
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            float xOffset = Position.X;

            for (int i = textImages.Length - 1; i >= 0; i--)
            {
                var img = textImages[i];
                if (img == null)
                {
                    xOffset -= 0.7f;
                    continue;
                }

                textTexture.Create(img);
                modelMatrix = GameMath.TransformMatrix(new Vector3(xOffset, Position.Y, Position.Z), scale.X, scale.Y, scale.Z);
                shader.SetMatrix4(ref modelMatrix, "model");
                xOffset -= 0.8f;

                GL.DrawArrays(PrimitiveType.Triangles, 0, QuadVertices.Length / 5);
            }
        }
    }
}