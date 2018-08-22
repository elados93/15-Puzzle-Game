using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfApp.Misc;
using WpfApp.Model;
using WpfApp.Model.Solver;
using WpfApp.ViewModel;

namespace WpfApp.Controls {
    /// <summary>
    /// Interaction logic for TilePuzzleUserControl.xaml
    /// </summary>
    public partial class TilePuzzleUserControl : UserControl {

        private TilePuzzleViewModel vm;

        public TilePuzzleUserControl() {
            InitializeComponent();
            vm = new TilePuzzleViewModel();
            this.DataContext = this.vm;
            this.vm.WrongTileClicked += wrongTileAnimation;
        }

        private void wrongTileAnimation(int i, int j) {
            foreach (UIElement label in BaseGrid.Children) {
                if(Grid.GetRow(label) == i && Grid.GetColumn(label) == j) {
                    Label wrongLabel = label as Label;

                    SolidColorBrush color;
                    if (isLabelEmpty(wrongLabel))
                        wrongLabel.Background = color = new SolidColorBrush(Colors.Gray);
                    else
                        wrongLabel.Background = color = new SolidColorBrush(Colors.White);

                    ColorAnimation ca = new ColorAnimation(Colors.Red, new Duration(TimeSpan.FromSeconds(0.2)));
                    ca.EasingFunction = new QuadraticEase();
                    ca.AutoReverse = true;
                    wrongLabel.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                }
            }
        }

        private bool isLabelEmpty(Label l) {
            Viewbox vb = l.Content as Viewbox;
            TextBlock tb = vb.Child as TextBlock;
            return tb.Text.Equals(" ");
        }

        public void createGrid(PuzzleArgs args) {
            /* Clear all the grid from prev elements */
            this.BaseGrid.Children.Clear();
            this.BaseGrid.RowDefinitions.Clear();
            this.BaseGrid.ColumnDefinitions.Clear();

            this.vm.setMazeValues(args);
            int rows = args.Rows;
            int cols = args.Cols;
            Level level = args.Level;

            for (int b = 0; b < rows; ++b) {
                RowDefinition r = new RowDefinition();
                this.BaseGrid.RowDefinitions.Add(r);
                r.Height = new GridLength(1, GridUnitType.Star);
            }

            for (int a = 0; a < cols; ++a) {
                ColumnDefinition c = new ColumnDefinition();
                this.BaseGrid.ColumnDefinitions.Add(c);
                c.Width = new GridLength(1, GridUnitType.Star);
            }

            createLabels();
        }

        public string getStringPuzzle() {
            StringBuilder sb = new StringBuilder();
            foreach (Tile tile in this.vm.VM_arr)
                sb.Append(tile.Value);

            return sb.ToString();
        }

        private void createLabels() {
            int i = 0, j = 0, numberToTile = 1;
            int last = BaseGrid.RowDefinitions.Count * BaseGrid.ColumnDefinitions.Count;

            foreach (RowDefinition r in BaseGrid.RowDefinitions) {
                j = 0;
                foreach (ColumnDefinition c in BaseGrid.ColumnDefinitions) {
                    Label label = new Label();

                    /* Using ViewBox in order to max the text size */
                    Viewbox viewBox = new Viewbox();
                    TextBlock textBlock = new TextBlock();
                    viewBox.Child = textBlock;
                    label.Content = viewBox;

                    Binding valueBind = new Binding("Value");
                    valueBind.Source = this.vm.VM_arr[i, j];
                    textBlock.SetBinding(TextBlock.TextProperty, valueBind);

                    Binding backgroundBind = new Binding("BackgroundColor");
                    backgroundBind.Source = this.vm.VM_arr[i, j];
                    backgroundBind.Mode = BindingMode.TwoWay;
                    label.SetBinding(BackgroundProperty, backgroundBind);

                    Binding borderThicknessBind = new Binding("VM_BorderThickness");
                    borderThicknessBind.Source = this.vm;
                    borderThicknessBind.Mode = BindingMode.TwoWay;
                    label.SetBinding(BorderThicknessProperty, borderThicknessBind);

                    Binding borderBrushBind = new Binding("VM_BorderBrush");
                    borderBrushBind.Source = this.vm;
                    borderBrushBind.Mode = BindingMode.TwoWay;
                    label.SetBinding(BorderBrushProperty, borderBrushBind);

                    label.HorizontalContentAlignment = HorizontalAlignment.Center;
                    label.VerticalContentAlignment = VerticalAlignment.Center;

                    Grid.SetColumn(label, j);
                    Grid.SetRow(label, i);
                    label.MouseDown += vm.clickOnTile;

                    this.BaseGrid.Children.Add(label);
                    ++j;
                    ++numberToTile;
                }
                ++i;
            }
        }

        public async void solve() {
            
            Task solver = new Task(() => {
                this.vm.solve();
            });
            solver.Start();
            await solver;
        }

    }
}
