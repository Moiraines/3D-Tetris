namespace Tetris
{
    using System;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using Tetris.enums;

    /// <summary>
    /// Клас Tetromino представя текущата активна фигура в играта.
    /// Управлява позицията, въртенето, движението и състоянието на фигурата.
    /// </summary>
    public class Tetromino
    {
        // Централна начална позиция за създаване на фигура
        private const int x = 6, y = 19, z = 5;

        private Shape initialPosition;
        private Matrix3 rotationMatrix;
        private float currentTime;

        /// <summary>Масив от четири Shape обекта, съставящи тетроминото.</summary>
        public Shape[] Shapes { get; } = new Shape[4];

        /// <summary>Типът на тетроминото (I, L, O, T и др.).</summary>
        public TetrominoKind Kind { get; }

        /// <summary>Дали фигурата е достигнала дъното или друга фигура.</summary>
        public bool IsDown { get; set; } = false;

        /// <summary>Време за падане между две стъпки (динамично според нивото).</summary>
        public static float TimeDown;

        /// <summary>
        /// Конструктор за тетромино. Избира позиция и тип, поставя я на полето.
        /// </summary>
        public Tetromino(TetrominoKind kind, TetrominoColor color)
        {
            Kind = kind;
            Shapes[0] = Playground.map[x, y, z];
            SetTetrominoKind();
            SetTetrominosVisible(color);
            TimeDown = LevelTimeDown();
        }

        /// <summary>
        /// Инициализира останалите 3 блока на фигурата според вида ѝ.
        /// </summary>
        private void SetTetrominoKind()
        {
            var positions = GetPositions(Kind);
            for (int i = 1; i < Shapes.Length; i++)
                Shapes[i] = Playground.map[
                    (int)Math.Round(positions[i].x),
                    (int)Math.Round(positions[i].y),
                    (int)Math.Round(positions[i].z)];

            initialPosition = Kind switch
            {
                TetrominoKind.ITetromino => Shapes[2],
                TetrominoKind.LTetromino => Shapes[1],
                TetrominoKind.TTetromino => Shapes[1],
                TetrominoKind.JTetromino => Shapes[1],
                TetrominoKind.ZTetromino => Shapes[0],
                TetrominoKind.STetromino => Shapes[0],
                _ => null
            };
        }

        /// <summary>
        /// Проверява дали въртенето е възможно – всички нови позиции са валидни и незаети.
        /// </summary>
        private bool CanRotateTetromino(Vector3[] positions)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                if (Shapes[i] == initialPosition) continue;

                int rx = (int)MathF.Round(positions[i].X);
                int ry = (int)MathF.Round(positions[i].Y);
                int rz = (int)MathF.Round(positions[i].Z);

                if (rx < 0 || rx >= 10 || ry < 0 || ry >= 20 || rz < 0 || rz >= 10)
                    return false;

                if (!IsInsideTetromino(rx, ry, rz) && Playground.map[rx, ry, rz].State == ShapeState.Solid)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Проверява дали дадена позиция е достъпна за движение.
        /// </summary>
        private bool CanMove(int x, int y, int z)
        {
            return y >= 0 && (IsInsideTetromino(x, y, z) || Playground.map[x, y, z].State != ShapeState.Solid);
        }

        /// <summary>
        /// Проверява дали даден Shape е част от текущото тетромино.
        /// </summary>
        private bool IsInsideTetromino(int x, int y, int z)
        {
            foreach (var shape in Shapes)
                if (Playground.map[x, y, z] == shape) return true;

            return false;
        }

        /// <summary>
        /// Извършва движението надолу. Ако не може да падне – IsDown = true.
        /// </summary>
        private void MoveDown()
        {
            if (currentTime < TimeDown) return;
            currentTime = 0.0f;

            if (Shapes[3].Position.y == 0)
            {
                IsDown = true;
                return;
            }

            foreach (var shape in Shapes)
            {
                var pos = shape.Position;
                if (!CanMove((int)pos.x, (int)(pos.y - 1), (int)pos.z))
                {
                    IsDown = true;
                    return;
                }
            }

            foreach (var shape in Shapes)
                shape.State = ShapeState.Empty;

            for (int i = 0; i < Shapes.Length; i++)
                Shapes[i] = Playground.VisibleShape(
                    (int)Shapes[i].Position.x,
                    (int)(Shapes[i].Position.y - 1),
                    (int)Shapes[i].Position.z,
                    Shapes[i]);

            if (initialPosition != null)
                initialPosition = Playground.map[
                    (int)initialPosition.Position.x,
                    (int)(initialPosition.Position.y - 1),
                    (int)initialPosition.Position.z];
        }

        /// <summary>
        /// Контролира страничното движение наляво/надясно или напред/назад.
        /// </summary>
        private void Controll(int dirX, int dirZ)
        {
            foreach (var shape in Shapes)
            {
                var pos = shape.Position;
                if ((dirX != 0 && pos.x == (dirX == -1 ? 0 : 9)) ||
                    (dirZ != 0 && pos.z == (dirZ == -1 ? 0 : 9)) ||
                    !CanMove((int)(pos.x + dirX), (int)pos.y, (int)(pos.z + dirZ)))
                    return;
            }

            foreach (var shape in Shapes)
                shape.State = ShapeState.Empty;

            for (int i = 0; i < Shapes.Length; i++)
                Shapes[i] = Playground.VisibleShape(
                    (int)(Shapes[i].Position.x + dirX),
                    (int)Shapes[i].Position.y,
                    (int)(Shapes[i].Position.z + dirZ),
                    Shapes[i]);

            if (initialPosition != null)
                initialPosition = Playground.map[
                    (int)(initialPosition.Position.x + dirX),
                    (int)initialPosition.Position.y,
                    (int)(initialPosition.Position.z + dirZ)];
        }

        /// <summary>
        /// Рендира фигурата като "следваща" в панела Next Up.
        /// </summary>
        public void ShowAsNextUp(float deltaTime)
        {
            foreach (var shape in Shapes)
                shape.Render();
        }

        /// <summary>
        /// Прави всички клетки на тетроминото видими и им прилага вид(при куба - текстура, при сферата - цвят).
        /// </summary>
        public void SetTetrominosVisible(TetrominoColor color)
        {
            foreach (var shape in Shapes)
            {
                shape.ApplyAppearance(color);
                shape.SetVisible();
            }
        }

        /// <summary>
        /// Завърта фигурата около дадена ос с определен ъгъл, ако е възможно.
        /// </summary>
        public void Rotate(float degree, Axis axis)
        {
            if (initialPosition == null) return;

            rotationMatrix = axis switch
            {
                Axis.X => Matrix3.CreateRotationX(degree),
                Axis.Y => Matrix3.CreateRotationY(degree),
                Axis.Z => Matrix3.CreateRotationZ(degree),
                _ => rotationMatrix
            };

            Vector3[] newPositions = new Vector3[4];

            for (int i = 0; i < Shapes.Length; i++)
            {
                if (Shapes[i] == initialPosition) continue;

                var relative = new Vector3(
                    Shapes[i].Position.x - initialPosition.Position.x,
                    Shapes[i].Position.y - initialPosition.Position.y,
                    Shapes[i].Position.z - initialPosition.Position.z);

                var transformed = Vector3.TransformRow(relative, rotationMatrix);

                newPositions[i] = new Vector3(
                    transformed.X + initialPosition.Position.x,
                    transformed.Y + initialPosition.Position.y,
                    transformed.Z + initialPosition.Position.z);
            }

            if (!CanRotateTetromino(newPositions)) return;

            foreach (var shape in Shapes)
            {
                if (shape == initialPosition) continue;
                shape.State = ShapeState.Empty;
            }

            for (int i = 0; i < Shapes.Length; i++)
            {
                if (Shapes[i] == initialPosition) continue;

                int nx = (int)MathF.Round(newPositions[i].X);
                int ny = (int)MathF.Round(newPositions[i].Y);
                int nz = (int)MathF.Round(newPositions[i].Z);

                Shapes[i] = Playground.VisibleShape(nx, ny, nz, Shapes[i]);
            }
        }

        /// <summary>
        /// Обработва натискане на клавиш за движение.
        /// </summary>
        public void Move(Keys key)
        {
            if (key == Keys.Left) Controll(-1, 0);
            if (key == Keys.Right) Controll(1, 0);
            if (key == Keys.Up) Controll(0, -1);
            if (key == Keys.Down) Controll(0, 1);
        }

        /// <summary>
        /// Основен рендер метод – увеличава таймера и при нужда мести фигурата надолу.
        /// </summary>
        public void Render(float deltaTime)
        {
            currentTime += deltaTime;
            MoveDown();
        }

        /// <summary>
        /// Рендерира всички 4 Shape блока.
        /// </summary>
        public void Render()
        {
            foreach (var shape in Shapes)
                shape.Render();
        }

        /// <summary>
        /// Връща времето за падане на фигурата в зависимост от текущите точки (ниво).
        /// Колкото повече точки – толкова по-бързо пада.
        /// Подава на метод IsGivenLevel, който преценява дали да вдигне нивото.
        /// </summary>
        public static float LevelTimeDown()
        {
            if (Game.points < 600) return 1.0f;
            if (Game.IsGivenLevel(600, 0)) return 0.8f;
            if (Game.IsGivenLevel(1800, 1)) return 0.72f;
            if (Game.IsGivenLevel(2800, 2)) return 0.63f;
            if (Game.IsGivenLevel(3600, 3)) return 0.55f;
            if (Game.IsGivenLevel(5800, 4)) return 0.47f;
            if (Game.IsGivenLevel(6400, 5)) return 0.38f;
            if (Game.IsGivenLevel(9800, 6)) return 0.3f;
            if (Game.IsGivenLevel(15200, 7)) return 0.22f;
            if (Game.IsGivenLevel(18800, 8)) return 0.13f;
            if (Game.IsGivenLevel(21000, 9)) return 0.1f;
            if (Game.IsGivenLevel(24800, 10)) return 0.08f;
            if (Game.IsGivenLevel(28800, 11)) return 0.08f;
            if (Game.IsGivenLevel(32800, 12)) return 0.08f;
            if (Game.IsGivenLevel(67600, 13)) return 0.07f;
            if (Game.IsGivenLevel(82800, 14)) return 0.07f;
            if (Game.IsGivenLevel(122400, 15)) return 0.07f;
            if (Game.IsGivenLevel(200800, 16)) return 0.05f;
            if (Game.IsGivenLevel(329200, 17)) return 0.05f;
            if (Game.IsGivenLevel(498800, 18)) return 0.05f;
            if (Game.IsGivenLevel(648000, 19)) return 0.03f;
            if (Game.IsGivenLevel(724800, 20)) return 0.03f;
            if (Game.IsGivenLevel(778800, 21)) return 0.03f;
            if (Game.IsGivenLevel(862800, 22)) return 0.03f;
            if (Game.IsGivenLevel(921600, 23)) return 0.03f;
            if (Game.IsGivenLevel(999999, 24)) return 0.03f;
            if (Game.IsGivenLevel(999999, 25)) return 0.03f;
            if (Game.IsGivenLevel(999999, 26)) return 0.03f;
            if (Game.IsGivenLevel(999999, 27)) return 0.03f;
            if (Game.IsGivenLevel(999999, 28)) return 0.03f;
            return 1.0f;
        }

        /// <summary>
        /// Връща начални позиции за конкретен тип тетромино фигура.
        /// </summary>
        public static Position[] GetPositions(TetrominoKind kind)
        {
            var positions = new Position[4];
            positions[0] = new Position(x, y, z);

            switch (kind)
            {
                case TetrominoKind.ITetromino:
                    for (int i = 1; i < 4; i++) positions[i] = new Position(x - i, y, z);
                    break;
                case TetrominoKind.LTetromino:
                    positions[1] = new Position(x - 1, y, z);
                    positions[2] = new Position(x - 2, y, z);
                    positions[3] = new Position(x - 2, y - 1, z);
                    break;
                case TetrominoKind.OTetromino:
                    positions[1] = new Position(x + 1, y, z);
                    positions[2] = new Position(x, y - 1, z);
                    positions[3] = new Position(x + 1, y - 1, z);
                    break;
                case TetrominoKind.TTetromino:
                    positions[1] = new Position(x - 1, y, z);
                    positions[2] = new Position(x - 2, y, z);
                    positions[3] = new Position(x - 1, y - 1, z);
                    break;
                case TetrominoKind.JTetromino:
                    positions[1] = new Position(x - 1, y, z);
                    positions[2] = new Position(x - 2, y, z);
                    positions[3] = new Position(x, y - 1, z);
                    break;
                case TetrominoKind.ZTetromino:
                    positions[1] = new Position(x - 1, y, z);
                    positions[2] = new Position(x, y - 1, z);
                    positions[3] = new Position(x + 1, y - 1, z);
                    break;
                case TetrominoKind.STetromino:
                    positions[1] = new Position(x + 1, y, z);
                    positions[2] = new Position(x, y - 1, z);
                    positions[3] = new Position(x - 1, y - 1, z);
                    break;
            }
            return positions;
        }
    }
}