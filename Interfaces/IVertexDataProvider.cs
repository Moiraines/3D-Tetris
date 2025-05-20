namespace Tetris.Interfaces
{
    /// <summary>
    /// Интерфейс за обекти, които могат да предоставят масив от върхове (vertex данни) за OpenGL рендериране.
    /// </summary>
    public interface IVertexDataProvider
    {
        /// <summary>
        /// Връща масив от float стойности, съдържащи координатите и евентуално UV текстурни координати на върховете.
        /// </summary>
        /// <returns>Масив от върхове за рендериране.</returns>
        float[] GetVertices();
    }
}