//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Interactivity;

//namespace WpfApp.Behaviors {
//    class ProgressBarTextBehavior : Behavior<TextBlock> {

//        private bool isAnimating = false;

//        protected override void OnAttached() {
//            base.OnAttached();
//            TextBlock textBox = this.AssociatedObject;
//            textBox.chan += textBlockValueChanged;
//        }

//        private void textBlockValueChanged(object sender, TextChangedEventArgs e) {
//            if (isAnimating)
//                return;

//            isAnimating = true;

//            string valueToShow = "Progress: " + " %";
//           ((TextBox)sender).Text = valueToShow;

//            e.Handled = true;
//        }

//        private void Db_Completed(object sender, EventArgs e) {
//            isAnimating = false;
//        }

//        protected override void OnDetaching() {
//            base.OnDetaching();
//            TextBlock textBox = this.AssociatedObject;
//            textBox.TextChanged -= textBlockValueChanged;
//        }

//    }
//}
