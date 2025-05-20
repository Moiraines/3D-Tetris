namespace Tetris.Serializable
{
    using System;
    using Tetris.enums;

    /// <summary>
    /// DTO (Data Transfer Object) клас за сериализация на обекти от тип Shape.
    /// Използва се при запазване/зареждане на игралното поле във/от файл.
    /// </summary>
    [Serializable]
    public class SerializableShapeData
    {
        /// <summary>
        /// Тип на формата – Cube или Sphere.
        /// </summary>
        public ShapeType ShapeType { get; set; }

        /// <summary>
        /// Позиция X на формата в 3D игралното поле.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Позиция Y на формата в 3D игралното поле.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Позиция Z на формата в 3D игралното поле.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Състояние на формата – дали е Solid или Empty.
        /// </summary>
        public ShapeState State { get; set; }

        /// <summary>
        /// Цветът на формата, използван при рендериране.
        /// </summary>
        public TetrominoColor Color { get; set; }
    }
}