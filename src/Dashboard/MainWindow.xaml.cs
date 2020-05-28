using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Population<DrawableCreature> _population;
        private readonly DrawableTarget _target;

        public MainWindow()
        {
            InitializeComponent();

            _target = new DrawableTarget(Canvas, new Position(0, 0));
            _population = new Population<DrawableCreature>(10,
                position => new DrawableCreature(Canvas, position)
            );

            var timer = new DispatcherTimer();

            timer.Tick += ActAndRender;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }

        private void ActAndRender(object sender, EventArgs e)
        {
            _target.Move();
            _population.ForEach(creature => creature.Move(_target));

            _target.Render();
            _population.ForEach(creature => creature.Render());
        }
    }
}
