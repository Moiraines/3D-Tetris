namespace Tetris
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using OpenTK.Graphics.OpenGL;
    using OpenTK.Mathematics;
    using Tetris.enums;
    using Tetris.Interfaces;
    using Tetris.Serializable;

    /// <summary>
    /// Основен клас, който управлява 3D полето на играта – създаване, логика, рендериране, сериализация и статистика.
    /// </summary>
    public class Playground
    {
        // === Полета ===

        /// <summary>Генератор на произволни стойности (цветове, видове фигури).</summary>
        private static readonly Random rnd = new();

        /// <summary>3D масив от фигури (Shape), представляващ игралното поле.</summary>
        public static Shape[,,] map;

        /// <summary>Текущо избран вид фигури за играта (Cube или Sphere).</summary>
        public static ShapeType CurrentShapeType = ShapeType.Cube;

        /// <summary>Текущата активна фигура.</summary>
        public static Tetromino tetromino;

        /// <summary>Флаг дали фигурата е завършила унищожението си.</summary>
        public static bool NotDestroyed;

        /// <summary>Позиции на следващата фигура (за визуализация).</summary>
        public static Position[] NextPositions = new Position[4];

        /// <summary>Вид и цвят на следващата фигура.</summary>
        public static TetrominoKind NextTetrominoKind;
        public static TetrominoColor NextTetrominoColor;

        /// <summary>Флаг за край на играта.</summary>
        public static bool gameover;

        private int vbo, vao;

        /// <summary>Връща текущия Vertex Array Object (VAO).</summary>
        public int GetVAO() => vao;

        /// <summary>Текстов елемент за надпис "GAME OVER!".</summary>
        public Text GameStatus = new Text("GAME OVER!", new Vector3(6.3f, 0.5f, 0), new Vector3(0.2f, 0.4f, 0.3f), new Vector3(35f, 35f, 2.0f));

        // === Конструктор ===

        /// <summary>
        /// Създава игралното поле, инициализира буфери, карта и текстове, зарежда първата и следващата фигура.
        /// </summary>
        public Playground()
        {
            InitializeBuffersForAllShapes();

            map = new Shape[10, 20, 10];

            Game.points = 0;
            Game.level = new Text(Game.currentlevel.ToString(), new Vector3(-2.5f, 8, 0), new Vector3(1f, 1f, 1f));
            Game.lines = new Text(Game.linesCleared.ToString(), new Vector3(-2.5f, 6, 0), new Vector3(1f, 1f, 1f));
            Game.score = new Text(Game.points.ToString(), new Vector3(-2.5f, 4, 0), new Vector3(1f, 1f, 1f));

            Game.currentlevel = 1;
            Game.linesCleared = 0;
            Game.score.ChangeText(Game.points.ToString());
            Game.level.ChangeText(Game.currentlevel.ToString());
            Game.lines.ChangeText(Game.linesCleared.ToString());

            NotDestroyed = true;
            gameover = false;

            for (int y = 0; y < 20; y++)
                for (int x = 0; x < 10; x++)
                    for (int z = 0; z < 10; z++)
                        map[x, y, z] = CreateShape(x, y, z);

            NextTetrominoColor = RandomTetrominoColor();
            NextTetrominoKind = RandomTetrominoKind();
            NextPositions = Tetromino.GetPositions(NextTetrominoKind);
            tetromino = RandomTetromino();
            Game.showNextTetromino = true;
        }

        /// <summary>
        /// Основна логика на играта – проверява дали фигурата е кацнала, дали има пълни редове и създава нова фигура.
        /// </summary>
        public void Update()
        {
            bool[] IsSolidRow = new bool[20];

            if (tetromino.IsDown && !gameover)
            {
                GameEvents.RaiseTetrominoLanded(tetromino);
                tetromino.IsDown = false;

                for (int y = 0; y < 20; y++)
                {
                    if (SolidRow(y))
                    {
                        IsSolidRow[y] = true;
                        Game.points += 100;
                        Game.score.ChangeText(Game.points.ToString());
                        Game.linesCleared += 1;
                        Game.lines.ChangeText(Game.linesCleared.ToString());
                    }
                }

                CheckForStatus();
                MoveDownRow(IsSolidRow);

                if (EmptyRows(IsSolidRow))
                {
                    tetromino = new Tetromino(NextTetrominoKind, NextTetrominoColor);
                    NextTetrominoColor = RandomTetrominoColor();
                    NextTetrominoKind = RandomTetrominoKind();
                    NextPositions = Tetromino.GetPositions(NextTetrominoKind);
                    Game.showNextTetromino = true;
                }
            }

            if (!NotDestroyed)
            {
                NotDestroyed = true;
                MoveDownRow(IsSolidRow);
                tetromino = new Tetromino(NextTetrominoKind, NextTetrominoColor);
                NextTetrominoColor = RandomTetrominoColor();
                NextTetrominoKind = RandomTetrominoKind();
                NextPositions = Tetromino.GetPositions(NextTetrominoKind);
                Game.showNextTetromino = true;
            }
        }

        /// <summary>
        /// Рендерира всички елементи в игралното поле + текущата фигура.
        /// </summary>
        public void Render(float deltaTime)
        {
            GL.BindVertexArray(vao);

            if (gameover)
            {
                GameStatus.Render();
                return;
            }

            Shape.GlobalShader.Use();
            Shape.GlobalShader.SetMatrix4(ref Camera.view, "view");
            Shape.GlobalShader.SetMatrix4(ref Camera.projection, "projection");

            for (int y = 0; y < 20; y++)
                for (int x = 0; x < 10; x++)
                    for (int z = 0; z < 10; z++)
                        map[x, y, z].Render(deltaTime);

            tetromino.Render(deltaTime);
        }

        /// <summary>
        /// Записва състоянието на игралното поле във файл (JSON сериализация).
        /// </summary>
        public void SaveToFile(string path)
        {
            var shapes = new List<SerializableShapeData>();

            for (int y = 0; y < 20; y++)
                for (int x = 0; x < 10; x++)
                    for (int z = 0; z < 10; z++)
                    {
                        var shape = map[x, y, z];
                        if (shape.State == ShapeState.Empty) continue;

                        shapes.Add(new SerializableShapeData
                        {
                            ShapeType = shape is Cube ? ShapeType.Cube : ShapeType.Sphere,
                            X = shape.Position.x,
                            Y = shape.Position.y,
                            Z = shape.Position.z,
                            State = shape.State,
                            Color = shape.AssignedColor
                        });
                    }

            var json = System.Text.Json.JsonSerializer.Serialize(shapes, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Зарежда състоянието на игралното поле от файл.
        /// </summary>
        public void LoadFromFile(string path)
        {
            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var shapes = System.Text.Json.JsonSerializer.Deserialize<List<SerializableShapeData>>(json);

            for (int y = 0; y < 20; y++)
                for (int x = 0; x < 10; x++)
                    for (int z = 0; z < 10; z++)
                        map[x, y, z].State = ShapeState.Empty;

            foreach (var data in shapes)
            {
                Shape shape = data.ShapeType switch
                {
                    ShapeType.Cube => new Cube(data.X, data.Y, data.Z, "element_blue_square_glossy.png"),
                    ShapeType.Sphere => new Sphere(data.X, data.Y, data.Z),
                    _ => null
                };

                if (shape != null)
                {
                    shape.ApplyAppearance(data.Color);
                    shape.SetVisible();
                    map[(int)data.X, (int)data.Y, (int)data.Z] = shape;
                }
            }

            tetromino = new Tetromino(NextTetrominoKind, NextTetrominoColor);
        }

        /// <summary>
        /// Извежда статистика за състоянието на игралното поле чрез LINQ:
        /// - Колко фигури са на пода
        /// - Колко фигури от всеки вид (по State)
        /// - По колко фигури има на всеки ред
        /// - Колко фигури от всеки цвят
        /// </summary>
        public void PrintLinqStats()
        {
            var allShapes = map.Cast<Shape>().Where(s => s != null && s.State != ShapeState.Empty).ToList();

            int onFloor = allShapes.Count(s => s.Position.y == 0);
            Console.WriteLine($"1. Shapes on floor (y == 0): {onFloor}");

            var groupedStates = map.Cast<Shape>()
                           .GroupBy(shape => shape.State)
                           .Select(g => new { State = g.Key, Count = g.Count() });

            Console.WriteLine("2. == Shape State Breakdown ==");
            foreach (var entry in groupedStates)
                Console.WriteLine($"{entry.State}: {entry.Count}");

            var shapesPerRow = allShapes
                    .Where(s => s.State == ShapeState.Solid)
                    .GroupBy(s => s.Position.y)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Row = g.Key, Count = g.Count() });

            Console.WriteLine("3. == Solid Shapes per Y row ==");
            foreach (var row in shapesPerRow)
                Console.WriteLine($"Row {row.Row}: {row.Count} blocks");

            var colorGroups = allShapes.GroupBy(s => s.AssignedColor);
            Console.WriteLine("4. Colors:");
            foreach (var group in colorGroups)
                Console.WriteLine($"   {group.Key}: {group.Count()}");
        }

        // === Помощни методи ===

        /// <summary>Проверява дали има блок в горния ред, което означава Game Over.</summary>
        private void CheckForStatus()
        {
            if (map[6, 19, 5].State == ShapeState.Solid)
                gameover = true;
        }

        /// <summary>Проверява дали няма нито един пълен ред за унищожаване.</summary>
        private bool EmptyRows(bool[] IsSolidRow)
        {
            for (int i = 0; i < IsSolidRow.Length; i++)
                if (IsSolidRow[i]) return false;
            return true;
        }

        /// <summary>Унищожава пълните редове и придвижва останалите надолу.</summary>
        private void MoveDownRow(bool[] IsSolidRow)
        {
            for (int y = 0; y < 20; y++)
                if (IsSolidRow[y]) DestroyRow(y);

            for (int y = 0; y < 20; y++)
                for (int z = 0; z < 10; z++)
                    if (map[0, y, z].Destroyed) return;

            MoveAllRowsDown();
        }

        /// <summary>Придвижва всички редове надолу с една позиция.</summary>
        private void MoveAllRowsDown()
        {
            for (int y = 0; y < 19; y++)
                MoveDown(y);
        }

        /// <summary>Придвижва един ред надолу, ако е възможно.</summary>
        private void MoveDown(int y)
        {
            for (; y > 0; y--)
            {
                if (CanMoveDown(y))
                {
                    for (int x = 0; x < 10; x++)
                    {
                        for (int z = 0; z < 10; z++)
                        {
                            if (map[x, y, z].State == ShapeState.Empty) continue;

                            var temp = map[x, y, z];
                            map[x, y, z].State = ShapeState.Empty;
                            map[x, y - 1, z].CopyAppearanceFrom(temp);
                            map[x, y - 1, z].SetVisible();
                        }
                    }
                }
            }
        }

        /// <summary>Проверява дали даден ред може да бъде преместен надолу.</summary>
        private bool CanMoveDown(int y)
        {
            for (int x = 0; x < 10; x++)
                for (int z = 0; z < 10; z++)
                    if (y - 1 < 0 || map[x, y - 1, z].State != ShapeState.Empty)
                        return false;

            for (int x = 0; x < 10; x++)
                for (int z = 0; z < 10; z++)
                    if (map[x, y, z].State != ShapeState.Empty)
                        return true;

            return false;
        }

        /// <summary>Маркира цял ред като унищожен.</summary>
        private void DestroyRow(int y)
        {
            for (int x = 0; x < 10; x++)
                for (int z = 0; z < 10; z++)
                    map[x, y, z].Destroyed = true;
        }

        /// <summary>Проверява дали даден ред е изцяло запълнен със Solid блокчета.</summary>
        private bool SolidRow(int y)
        {
            for (int x = 0; x < 10; x++)
                for (int z = 0; z < 10; z++)
                    if (map[x, y, z].State != ShapeState.Solid)
                        return false;
            return true;
        }

        /// <summary>
        /// Генерира нова фигура със случаен цвят и тип.
        /// </summary>
        private Tetromino RandomTetromino()
        {
            TetrominoColor color = RandomTetrominoColor();
            TetrominoKind kind = RandomTetrominoKind();
            return new Tetromino(kind, color);
        }

        /// <summary>
        /// Създава буферите (VAO и VBO) и конфигурира атрибутите според типа на формата.
        /// </summary>
        private void InitializeBuffersForAllShapes()
        {
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            Shape tempShape = CreateShape(0, 0, 0);
            float[] vertices = ((IVertexDataProvider)tempShape).GetVertices();
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            if (CurrentShapeType == ShapeType.Cube)
            {
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }
            else if (CurrentShapeType == ShapeType.Sphere)
            {
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }
        }

        /// <summary>
        /// Задава дадена позиция като видима, копирайки външния вид от друга фигура.
        /// </summary>
        public static Shape VisibleShape(int x, int y, int z, Shape from = null)
        {
            if (from != null)
                map[x, y, z].CopyAppearanceFrom(from);

            map[x, y, z].SetVisible();
            return map[x, y, z];
        }

        /// <summary>
        /// Създава нова Shape фигура (Cube или Sphere) на зададени координати.
        /// </summary>
        public static Shape CreateShape(float x, float y, float z)
        {
            return CurrentShapeType switch
            {
                ShapeType.Cube => new Cube(x, y, z, "element_blue_square_glossy.png"),
                ShapeType.Sphere => new Sphere(x, y, z),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Връща случаен цвят за тетромино фигура.
        /// </summary>
        public TetrominoColor RandomTetrominoColor()
        {
            return rnd.Next(0, 5) switch
            {
                0 => TetrominoColor.Blue,
                1 => TetrominoColor.Red,
                2 => TetrominoColor.Green,
                3 => TetrominoColor.Purple,
                4 => TetrominoColor.Yellow,
                _ => TetrominoColor.Blue
            };
        }

        /// <summary>
        /// Връща случаен тип фигура (I, O, L, T и т.н.).
        /// </summary>
        public TetrominoKind RandomTetrominoKind()
        {
            return rnd.Next(0, 7) switch
            {
                0 => TetrominoKind.ITetromino,
                1 => TetrominoKind.STetromino,
                2 => TetrominoKind.ZTetromino,
                3 => TetrominoKind.JTetromino,
                4 => TetrominoKind.LTetromino,
                5 => TetrominoKind.OTetromino,
                6 => TetrominoKind.TTetromino,
                _ => TetrominoKind.TTetromino
            };
        }
    }
}
