using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using WpfApp.Misc;
using WpfApp.Model.Solver.Searchable;
using WpfApp.Model.TileGenerator;

namespace WpfApp.Model {

    public delegate void GetProgress(Tile t1, Point p1, Tile t2, Point p2);
    public delegate void TilePositionDelegate(int i, int j);

    partial class TileGridModel : INotifyPropertyChanged, ISearchable {

        #region Members
        private int row;
        private int col;
        private Level level;
        private Tile[,] arr;
        private Point spacePoint;

        private Brush borderBrush;
        private Thickness borderThickness;
        private PuzzleProgressTracker tracker;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event GetProgress TileMoved;
        public event TilePositionDelegate WrongTileClicked;
        #endregion

        private void NotifyPropertyChanged(string v) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        #region Properties
        public int Row {
            get {
                return row;
            }

            set {
                this.row = value;
            }
        }

        public int Col {
            get {
                return col;
            }

            set {
                this.col = value;
            }
        }

        internal Tile[,] Arr {
            get {
                return arr;
            }

            set {
                this.arr = value;
            }
        }

        public Brush BorderBrush {
            get {
                return borderBrush;
            }

            set {
                if (value != this.borderBrush) {
                    this.borderBrush = value;
                    NotifyPropertyChanged("BorderBrush");
                }
            }
        }

        public Thickness BorderThickness {
            get {
                return borderThickness;
            }

            set {
                if (value != this.borderThickness) {
                    this.borderThickness = value;
                    NotifyPropertyChanged("BorderThickness");
                }
            }
        }

        public double Progress {
            get { return this.tracker.Progress; }
            set {
                if (value != this.tracker.Progress) {
                    this.tracker.Progress = value;
                    NotifyPropertyChanged("Progress");
                }
            }
        }

        public Level Level {
            get {
                return this.level;
            }
            set {
                if (value != this.level) {
                    this.level = value;
                    NotifyPropertyChanged("Level");
                }
            }
        }
        #endregion

        public void generateMaze() {
            ITileGenerator tileGenerator = new MyTileGenerator(new PuzzleArgs(row, col, level));
            this.arr = tileGenerator.generateNewPuzzle();
            this.spacePoint = findTheSpace();
            this.tracker = new PuzzleProgressTracker(arr);
            TileMoved += this.tracker.updateProgress;
            this.tracker.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged(e.PropertyName);
            };
            this.tracker.getFirstProgress();
            this.goalCalaulated = false;
        }

        public void clicked(int i, int j) {
            Tile clickedTile = this.Arr[i, j];
            Point clickedPoint = new Point(i, j);

            if (isMovePossible(i, j)) {

                if (i == spacePoint.X) {
                    if (j == spacePoint.Y - 1) {
                        swap(arr[i, j], spacePoint, arr[i, j + 1], new Point(i, j));
                        return;
                    }
                    if (j == spacePoint.Y + 1) {
                        swap(arr[i, j], spacePoint, arr[i, j - 1], new Point(i, j));
                        return;
                    }
                }
                if (j == spacePoint.Y) {
                    if (i == spacePoint.X - 1) {
                        swap(arr[i, j], spacePoint, arr[i + 1, j], new Point(i, j));
                        return;
                    }
                    if (i == spacePoint.X + 1) {
                        swap(arr[i, j], spacePoint, arr[i - 1, j], new Point(i, j));
                        return;
                    }
                }

                ///* Check all four sides for the space */
                //for (int l = -1; l < 2; l += 2) {
                //    if (isVaild(i, j + l) && this.Arr[i, j + l].isSpace()) {
                //        spacePoint = new Point(i, j + l);
                //        swap(clickedTile, spacePoint, this.Arr[i, j + l], clickedPoint);
                //        return;
                //    }
                //}

                //for (int k = -1; k < 2; k += 2) {
                //    if (isVaild(i + k, j) && this.Arr[i + k, j].isSpace()) {
                //        spacePoint = new Point(i + k, j);
                //        swap(clickedTile, spacePoint, this.Arr[i + k, j], clickedPoint);
                //        return;
                //    }
                //}
            }
            WrongTileClicked?.Invoke(i, j);
        }

        private bool isMovePossible(int i, int j) {
            return isVaild(i, j) && !this.Arr[i, j].isSpace();
        }

        private Point findTheSpace() {
            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++) {
                    if (this.arr[i, j].isSpace())
                        return new Point(i, j);
                }
            }
            return new Point(-1, -1); // not suppose to get here, but signify error as (-1, -1) 
        }

        private bool isVaild(int i, int j) {
            return i >= 0 && i < this.Row && j >= 0 && j < this.Col;
        }

        private void swap(Tile t1, Point p1, Tile t2, Point p2) {
            Tile temp = new Tile(t1);
            t1.copyValues(t2);
            t2.copyValues(temp);
            TileMoved?.Invoke(t2, p1, t1, p2);
            spacePoint = p2;
        }
    }
}