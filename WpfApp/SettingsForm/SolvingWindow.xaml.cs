using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace WpfApp.SettingsForm {
    /// <summary>
    /// Interaction logic for SolvingWindow.xaml
    /// </summary>
    public partial class SolvingWindow : Window, IDisposable {

        Action Worker { get; set; }

        public SolvingWindow(Action worker) {
            InitializeComponent();
            Worker = worker;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            Task.Factory.StartNew(Worker).ContinueWith(t => Close(), TaskScheduler.FromCurrentSynchronizationContext());
            DoubleAnimation animation = new DoubleAnimation(1, 0, Duration.Automatic);
            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.EasingFunction = new QuadraticEase();
            this.textBlock.BeginAnimation(OpacityProperty, animation);
        }

        public void Dispose() {
        }
    }
}
