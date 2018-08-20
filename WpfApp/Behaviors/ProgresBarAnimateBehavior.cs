using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;

namespace WpfApp.Behaviors {
    class ProgresBarAnimateBehavior : Behavior<ProgressBar> {
        private bool isAnimating = false;

        protected override void OnAttached() {
            base.OnAttached();
            ProgressBar progressBar = this.AssociatedObject;
            progressBar.ValueChanged += ProgressBar_ValueChanged;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (isAnimating)
                return;

            isAnimating = true;

            DoubleAnimation doubleAnimation = new DoubleAnimation
                (e.OldValue, e.NewValue, new Duration(TimeSpan.FromSeconds(0.3)), FillBehavior.Stop);
            doubleAnimation.Completed += Db_Completed;
            doubleAnimation.EasingFunction = new QuadraticEase();

            ((ProgressBar)sender).BeginAnimation(ProgressBar.ValueProperty, doubleAnimation);

            e.Handled = true;
        }

        private void Db_Completed(object sender, EventArgs e) {
            isAnimating = false;
        }

        protected override void OnDetaching() {
            base.OnDetaching();
            ProgressBar progressBar = this.AssociatedObject;
            progressBar.ValueChanged -= ProgressBar_ValueChanged;
        }
    }
}
