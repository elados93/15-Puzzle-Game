using System;
using System.ComponentModel;
using System.Windows;

namespace WpfApp.Model {


    class PuzzleProgressTracker : INotifyPropertyChanged {

        #region Members
        private int row;
        private int col;
        private Tile[,] arr;
        private int corrects;
        private int totalCells;
        private double progress;
        private bool wasCalc;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public PuzzleProgressTracker(Tile[,] arr) {
            this.arr = arr;
            this.row = arr.GetLength(0);
            this.col = arr.GetLength(1);
            this.wasCalc = false;
        }

        private void NotifyPropertyChanged(string v) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public void getFirstProgress() {
            int count = 1;
            corrects = 0;
            totalCells = (row * col);

            for (int i = 0; i < row; ++i)
                for (int j = 0; j < col; ++j) {
                    if (arr[i, j].Value.Equals(count.ToString()))
                        ++corrects;
                    ++count;
                }

            if (arr[row - 1, col - 1].isSpace())
                ++corrects;

            wasCalc = true;
            Progress = ((double)corrects / totalCells) * 100.0;
            NotifyPropertyChanged("Progress"); // Update about the new progress even if it's the same
        }

        public void updateProgress(Tile t1, Point p1, Tile t2, Point p2) {
            int i1 = (int)p1.X, j1 = (int)p1.Y, i2 = (int)p2.X, j2 = (int)p2.Y;
            string value1 = i1 == row - 1 && j1 == col - 1 ? " " : (i1 * col + j1 + 1).ToString();
            string value2 = i2 == row - 1 && j2 == col - 1 ? " " : (i2 * col + j2 + 1).ToString();

            /* Check values before switch*/
            if (value1.Equals(t2.Value))
                --corrects;
            if (value2.Equals(t1.Value))
                --corrects;

            /* Check values after switch*/
            if (value1.Equals(t1.Value))
                ++corrects;

            if (value2.Equals(t2.Value))
                ++corrects;

            Progress = ((double)corrects / totalCells) * 100.0;
        }

        public void updateSolved() {
            this.corrects = totalCells;
            this.wasCalc = true;
            Progress = 100.0;
            NotifyPropertyChanged("Progress");
        }

        public double Progress {
            get {
                if (!this.wasCalc)
                    getFirstProgress();
                return this.progress;
            }
            set {
                if (value != this.progress) {
                    this.progress = value;
                    NotifyPropertyChanged("Progress");
                }
            }
        }

    }
}
