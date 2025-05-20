namespace Tetris
{
    using OpenTK.Graphics.OpenGL4;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Advanced;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    /// <summary>
    /// Клас за зареждане, създаване и управление на текстури за OpenGL.
    /// Поддържа както ImageSharp (.png, .bmp), така и System.Drawing (Bitmap).
    /// </summary>
    public class Texture : IDisposable
    {
        private int handle;

        /// <summary>
        /// Празен конструктор – използва се когато текстурата ще се създаде по-късно.
        /// </summary>
        public Texture() { }

        /// <summary>
        /// Създава текстура директно от път до файл.
        /// </summary>
        /// <param name="path">Път до изображение (може .png, .bmp и др.).</param>
        public Texture(string path)
        {
            Create(path);
        }

        /// <summary>
        /// Създава и зарежда текстура от файл с помощта на ImageSharp.
        /// </summary>
        /// <param name="path">Пълен път до изображението.</param>
        public void Create(string path)
        {
            handle = GL.GenTexture();
            Use();

            using Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(path);
            image.Mutate(x => x.Flip(FlipMode.Vertical)); // OpenGL чете отдолу нагоре

            var memoryGroup = image.GetPixelMemoryGroup();
            var span = memoryGroup[0].Span;

            List<byte> pixels = new();
            foreach (Rgba32 p in span)
            {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
            }

            // Настройки за филтриране и повтаряне
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Опционално: задаване на цвят за границите
            float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            // Качване на текстурата в GPU
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                image.Width, image.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

            // Генериране на mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        /// <summary>
        /// Създава текстура от Bitmap (System.Drawing).
        /// Използва се предимно за текстови изображения (букви).
        /// </summary>
        /// <param name="bitmap">Bitmap изображение.</param>
        public void Create(Bitmap bitmap)
        {
            handle = GL.GenTexture();
            BitmapData data = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Качване в GPU
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            // Филтриране
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        /// <summary>
        /// Активира текстурата за използване в шейдъра.
        /// </summary>
        /// <param name="unit">Текстурен юнит (по подразбиране: Texture0).</param>
        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }

        /// <summary>
        /// Освобождава текстурата от GPU.
        /// </summary>
        public void Delete()
        {
            if (handle != 0)
            {
                GL.DeleteTexture(handle);
                handle = 0;
            }
        }

        /// <summary>
        /// Автоматично извиква Delete() при изход или унищожаване на обекта.
        /// </summary>
        public void Dispose()
        {
            Delete();
            GC.SuppressFinalize(this);
        }
    }
}