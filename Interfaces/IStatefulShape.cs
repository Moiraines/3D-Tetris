namespace Tetris.Interfaces
{
    using Tetris.enums;

    /// <summary>
    /// Интерфейс за фигури, които имат състояние (например Solid, Empty).
    /// Използва се за проследяване дали дадена фигура е активна, видима и т.н.
    /// </summary>
    public interface IStatefulShape
    {
        /// <summary>
        /// Състояние на фигурата – използва се при рендериране и логика за премахване.
        /// </summary>
        ShapeState State { get; set; }
    }
}
