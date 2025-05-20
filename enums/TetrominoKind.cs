namespace Tetris.enums
{
    /// <summary>
    /// Изброим тип, който определя формата на тетроминото.
    /// Използва се при създаване на нова фигура.
    /// </summary>
    public enum TetrominoKind
    {
        /// <summary>Прав тетромино (I форма)</summary>
        ITetromino,

        /// <summary>L-образен тетромино</summary>
        LTetromino,

        /// <summary>Квадратен тетромино (O форма)</summary>
        OTetromino,

        /// <summary>Т-образен тетромино</summary>
        TTetromino,

        /// <summary>Огледален на L тетромино (J форма)</summary>
        JTetromino,

        /// <summary>Зиг-заг форма (Z тетромино)</summary>
        ZTetromino,

        /// <summary>Огледална зиг-заг форма (S тетромино)</summary>
        STetromino
    }
}