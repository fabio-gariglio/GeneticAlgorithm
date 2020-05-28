using System.Windows;

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

            _target = new DrawableTarget(Canvas, new Position(30, 30));
            _population = new Population<DrawableCreature>(10,
                position => new DrawableCreature(Canvas, position)
            );

            _target.Render();
            _population.ForEach(creature => creature.Render());
        }
    }
}
