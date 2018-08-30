using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfApp.Misc;
using WpfApp.Model;
using WpfApp.Model.Solver;

namespace WpfApp.ViewModel {
    class TilePuzzleViewModel : INotifyPropertyChanged {

        private TileGridModel model;
        private Solution lastSolution;

        #region events
        public event PropertyChangedEventHandler PropertyChanged;
        public event TilePositionDelegate WrongTileClicked;
        #endregion

        public TilePuzzleViewModel() {
            this.model = new TileGridModel();

            this.model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
            this.model.WrongTileClicked += delegate (int i, int j) {
                WrongTileClicked?.Invoke(i, j);
            };

            this.model.BorderBrush = Brushes.Black;
            this.model.BorderThickness = new Thickness(2);
        }

        public void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void setMazeValues(PuzzleArgs args) {
            VM_row = args.Rows;
            VM_col = args.Cols;
            VM_level = args.Level;
            this.model.generateMaze();
        }

        public void createMaze() {
            this.model.generateMaze();
        }

        public void clickOnTile(object sender, MouseButtonEventArgs e) {
            Label l = sender as Label;

            int rowTile = Grid.GetRow(l);
            int colTile = Grid.GetColumn(l);

            this.model.clicked(rowTile, colTile);
        }

        #region Properies
        public int VM_col {
            get {
                return this.model.Col;
            }
            set {
                if (value != this.model.Col) {
                    this.model.Col = value;

                }
            }
        }

        public int VM_row {
            get {
                return this.model.Row;
            }
            set {
                if (value != this.model.Row) {
                    this.model.Row = value;
                }
            }
        }

        internal Tile[,] VM_arr {
            get {
                return this.model.Arr;
            }
            set {
                this.model.Arr = value;
            }
        }

        public Brush VM_BorderBrush {
            get {
                return model.BorderBrush;
            }

            set {
                if (value != this.model.BorderBrush) {
                    this.model.BorderBrush = value;
                    NotifyPropertyChanged("BorderBrush");
                }
            }
        }

        public Thickness VM_BorderThickness {
            get {
                return this.model.BorderThickness;
            }

            set {
                if (value != this.model.BorderThickness) {
                    this.model.BorderThickness = value;
                    NotifyPropertyChanged("BorderThickness");
                }
            }
        }

        public double VM_Progress {
            get { return this.model.Progress; }

        }

        public Level VM_level {
            get { return this.model.Level; }
            set {
                if (value != this.model.Level) {
                    this.model.Level = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }
        #endregion

        internal void solve() {
            lastSolution = this.model.solve();
            int steps = lastSolution.NumberOfSteps;
            if (steps > 1)
                MessageBox.Show("Solved with: " + lastSolution.NumberOfSteps + " steps!");
            else if (steps == 1)
                MessageBox.Show("Solved with one step!");
            else
                MessageBox.Show("Already solved! nice try..");
        }

        internal void solveByLastSolution() {
            this.model.solveBySolution(this.lastSolution);
        }
    }
}
