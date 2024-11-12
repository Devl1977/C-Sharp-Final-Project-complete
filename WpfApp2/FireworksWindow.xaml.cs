using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MemoryGame
{
    public partial class FireworksWindow : Window
    {
        private Random _random = new Random();
        private DispatcherTimer _timer;

        public FireworksWindow()
        {
            InitializeComponent();
            StartFireworks();
        }

        private void StartFireworks()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _timer.Tick += (sender, args) => Close();  // Close after 2 seconds
            _timer.Start();

            var fireworkTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            fireworkTimer.Tick += (sender, args) => LaunchFirework();
            fireworkTimer.Start();
        }

        private void LaunchFirework()
        {
            var spark = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(GetRandomColor())
            };

            Canvas.SetLeft(spark, (int)(FireworksCanvas.ActualWidth / 2));  // Cast to int
            Canvas.SetTop(spark, (int)(FireworksCanvas.ActualHeight / 2));  // Cast to int
            FireworksCanvas.Children.Add(spark);

            var xAnimation = new DoubleAnimation
            {
                From = (int)(FireworksCanvas.ActualWidth / 2),  // Cast to int
                To = _random.Next((int)FireworksCanvas.ActualWidth),  // Cast to int
                Duration = TimeSpan.FromSeconds(1)
            };

            var yAnimation = new DoubleAnimation
            {
                From = (int)(FireworksCanvas.ActualHeight / 2),  // Cast to int
                To = _random.Next((int)FireworksCanvas.ActualHeight),  // Cast to int
                Duration = TimeSpan.FromSeconds(1)
            };

            spark.BeginAnimation(Canvas.LeftProperty, xAnimation);
            spark.BeginAnimation(Canvas.TopProperty, yAnimation);

            var sizeAnimation = new DoubleAnimation
            {
                From = 10,
                To = _random.Next(15, 25),
                Duration = TimeSpan.FromSeconds(1)
            };

            spark.BeginAnimation(WidthProperty, sizeAnimation);
            spark.BeginAnimation(HeightProperty, sizeAnimation);

            xAnimation.Completed += (sender, args) =>
            {
                FireworksCanvas.Children.Remove(spark);
            };
        }

        private Color GetRandomColor()
        {
            return Color.FromRgb((byte)_random.Next(256), (byte)_random.Next(256), (byte)_random.Next(256));
        }
    }
}
