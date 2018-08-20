namespace WpfApp.Misc {
    public class PuzzleArgs {

        private int rows;
        private int cols;
        private Level level;

        public PuzzleArgs(int rows, int cols, Level level) {
            this.Rows = rows;
            this.Cols = cols;
            this.Level = level;
        }

        public int Rows {
            get {
                return rows;
            }

            set {
                this.rows = value;
            }
        }

        public int Cols {
            get {
                return cols;
            }

            set {
                this.cols = value;
            }
        }

        internal Level Level {
            get {
                return level;
            }

            set {
                this.level = value;
            }
        }
    }
}
