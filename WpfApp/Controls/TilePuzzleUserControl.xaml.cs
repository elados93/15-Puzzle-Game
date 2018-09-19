using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfApp.Misc;
using WpfApp.Model;
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

        public void solve() {
            this.vm.solve();
        }

        public void solveBySolution() {
            this.vm.solveByLastSolution();
        }

    }
}
