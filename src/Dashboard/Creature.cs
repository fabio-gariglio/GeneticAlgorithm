using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dashboard
{
  
    public enum Direction
    {
        North,
        East,
        South,
        West,
    }

    public class Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Creature
    {
        public Position Position { get; set; }

        public Creature(Position position)
        {
            Position = position;
        }

        public void Move(Target target)
        {
            var targetDirection = GetTargetDirection(target.Position);
            Position = CalculateNewPosition(targetDirection);
        }

        public Direction GetTargetDirection(Position targetPosition)
        {
            var angle = (Math.Atan2(targetPosition.Y - Position.Y, targetPosition.X - Position.X) * 180 / Math.PI)+180;

            if (angle >= 45 && angle < 135) return Direction.North;
            if (angle >= 135 && angle < 225) return Direction.East;
            if (angle >= 225 && angle < 315) return Direction.South;
            return Direction.West;
        }

        private Position CalculateNewPosition(Direction targetDirection)
        {
            switch (targetDirection)
            {
                case Direction.East:
                    return new Position(Position.X + 1, Position.Y);
                case Direction.South:
                    return new Position(Position.X, Position.Y + 1);
                case Direction.West:
                    return new Position(Position.X - 1, Position.Y);
                default:
                    return new Position(Position.X, Position.Y - 1);
            }
        }
    }

    public class Target
    {
        public Position Position { get; private set; }
        private int angle = 0;

        public Target(Position position)
        {
            Position = position;
        }

        public void Move()
        {
            angle += 10;
            if (angle > 360) angle = 0;

            var x = 30 - (int)(Math.Cos(angle * Math.PI / 180) * 20);
            var y = 30 - (int)(Math.Sin(angle * Math.PI / 180) * 20);
            Position = new Position(x, y);
        }
    }

    public class DrawableCreature : Creature
    {
        private readonly Rectangle _appearance;

        public DrawableCreature(Canvas canvas, Position position) : base(position)
        {
            _appearance = new Rectangle
            {
                Height = 10,
                Width = 10,
                Fill = new SolidColorBrush(Colors.DarkCyan)
            };

            canvas.Children.Add(_appearance);
        }

        public void Render()
        {
            Canvas.SetTop(_appearance, (Position.Y - 1) * 10);
            Canvas.SetLeft(_appearance, (Position.X - 1) * 10);
        }
    }

    public class DrawableTarget : Target
    {
        private readonly Rectangle _appearance;

        public DrawableTarget(Canvas canvas, Position position) : base(position)
        {
            _appearance = new Rectangle
            {
                Height = 10,
                Width = 10,
                Fill = new SolidColorBrush(Colors.Red)
            };

            canvas.Children.Add(_appearance);
        }

        public void Render()
        {
            Canvas.SetTop(_appearance, (Position.Y - 1) * 10);
            Canvas.SetLeft(_appearance, (Position.X - 1) * 10);
        }
    }

    public class Population<T> : IEnumerable<T>
        where T : Creature
    {
        private readonly Func<Position, T> _creatureFactory;
        private readonly Lazy<List<T>> _population;

        public int Size { get; }

        public Population(int size, Func<Position, T> creatureFactory)
        {
            Size = size;
            _creatureFactory = creatureFactory;
            _population = new Lazy<List<T>>(() => GeneratePopulation().ToList());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _population.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _population.Value.GetEnumerator();
        }

        private IEnumerable<T> GeneratePopulation()
        {
            var positionHashes = new HashSet<string>(Size);
            var random = new Random();

            for (var i = 0; i < Size; i++)
            {
                int x, y;
                string positionHash;
                
                do
                {
                    x = random.Next(1, 60);
                    y = random.Next(1, 60);
                    positionHash = $"{x}.{y}";
                } while (positionHashes.Contains(positionHash));

                positionHashes.Add(positionHash);

                yield return _creatureFactory(new Position(x, y));
            }
        }
    }
}
