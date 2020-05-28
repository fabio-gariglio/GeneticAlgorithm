using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dashboard
{
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
        public Position Position { get; }

        public Creature(Position position)
        {
            Position = position;
        }
    }

    public class Target
    {
        public Position Position { get; set; }

        public Target(Position position)
        {
            Position = position;
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
        private readonly double Radiant = Math.PI / 180;
        private int angle;

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
            angle += 10;
            if (angle > 360) angle = 0;

            var x = 30 - (int) (Math.Cos(angle * Radiant) * 20);
            var y = 30 - (int) (Math.Sin(angle * Radiant) * 20);
            Position = new Position(x, y);

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
