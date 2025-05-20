namespace Tetris
{
    using System;
    using System.IO;
    using System.Text;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;

    /// <summary>
    /// Клас за зареждане, компилиране и управление на GLSL шейдъри (Vertex + Fragment).
    /// Позволява задаване на uniforms към GPU.
    /// </summary>
    public class Shader
    {
        private readonly int handle;
        private readonly int vertexShader;
        private readonly int fragmentShader;

        /// <summary>
        /// Създава и компилира нов шейдър от зададени vertex и fragment GLSL файлове.
        /// </summary>
        /// <param name="vertexPath">Път до vertex шейдър файла (.vert).</param>
        /// <param name="fragmentPath">Път до fragment шейдър файла (.frag).</param>
        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexSource = File.ReadAllText(vertexPath, Encoding.UTF8);
            string fragmentSource = File.ReadAllText(fragmentPath, Encoding.UTF8);

            // Компилиране на Vertex Shader
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            CheckCompileErrors(vertexShader, "VERTEX");

            // Компилиране на Fragment Shader
            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            CheckCompileErrors(fragmentShader, "FRAGMENT");

            // Линкване на програмата
            handle = GL.CreateProgram();
            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);
            GL.LinkProgram(handle);

            // Освобождаване на ресурсите
            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        /// <summary>
        /// Активира шейдър програмата.
        /// </summary>
        public void Use()
        {
            GL.UseProgram(handle);
        }

        /// <summary>
        /// Връща ID на uniform променлива по име.
        /// </summary>
        public int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(handle, name);
        }

        /// <summary>
        /// Задава 4x4 матрица към uniform в шейдъра.
        /// </summary>
        public void SetMatrix4(ref Matrix4 matrix, string name)
        {
            int location = GetUniformLocation(name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        /// <summary>
        /// Задава вектор (3D) по име на uniform.
        /// </summary>
        public void SetVector3(string name, Vector3 vector)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
                GL.Uniform3(location, vector);
            else
                Console.WriteLine($"[Shader] Warning: Uniform '{name}' not found.");
        }

        /// <summary>
        /// Задава вектор (3D) по ID на uniform.
        /// </summary>
        public void SetVectorToUniform(Vector3 vector, int location)
        {
            GL.Uniform3(location, vector);
        }

        /// <summary>
        /// Задава вектор (4D) по ID на uniform.
        /// </summary>
        public void SetVectorToUniform(Vector4 vector, int location)
        {
            GL.Uniform4(location, vector);
        }

        /// <summary>
        /// Задава цяло число към uniform по име.
        /// </summary>
        public void SetInt(string name, int value)
        {
            int location = GetUniformLocation(name);
            GL.Uniform1(location, value);
        }

        /// <summary>
        /// Задава float стойност към uniform по име.
        /// </summary>
        public void SetFloat(string name, float value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
                GL.Uniform1(location, value);
        }

        /// <summary>
        /// Проверява за грешки при компилиране на шейдъри.
        /// </summary>
        private void CheckCompileErrors(int shader, string type)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == (int)All.False)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"[Shader Compilation Error] Type: {type}\n{infoLog}");
            }
        }
    }
}