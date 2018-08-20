using System;
using System.Windows;
using WpfApp.Misc;

namespace WpfApp.Model.TileGenerator {

    class MyTileGenerator : ITileGenerator {

        #region Members
        private int rows;
        private int cols;
        private Level level;
        private Tile[,] arr;
        private PuzzleProgressTracker tracker;
        #endregion

        public event GetProgress TilesMoved;

        public MyTileGenerator(PuzzleArgs args) {
            this.rows = args.Rows;
            this.cols = args.Cols;
            this.level = args.Level;
            this.arr = new Tile[rows, cols];
            this.tracker = new PuzzleProgressTracker(this.arr);
            TilesMoved += this.tracker.updateProgress;
        }

        public Tile[,] generateNewPuzzle() {
            createOrderedArr();
            suffleArrByLevel();

            return this.arr;
        }

        private void createOrderedArr() {
            int currentTileValue = 1;
            int lastTileValue = rows * cols;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < cols; ++j) {
                    if (currentTileValue != lastTileValue)
                        arr[i, j] = new Tile(currentTileValue.ToString());
                    else
                        arr[i, j] = new Tile(" ");
                    ++currentTileValue;
                }
        }

        private void suffleArrByLevel() {
            int direction, lastDirection = 0;
            Point pointToSwap;
            Point currentHole = new Point(rows - 1, cols - 1);
            Random rand = new Random();
            double targetProgress = getNumberOfProgressByLevel();

            while (true) {
                do {
                    direction = getRightDirection(currentHole, rand);
                } while (direction == lastDirection);

                pointToSwap = getTileInDirection(currentHole, direction);

                lastDirection = direction;
                swapTiles(currentHole, pointToSwap);

                currentHole.X = pointToSwap.X;
                currentHole.Y = pointToSwap.Y;
                if (this.tracker.Progress <= targetProgress)
                    break;
            }
        }

        private double getNumberOfProgressByLevel() {
            switch (level) {
                case Level.Easy: return 50.0;
                case Level.Medium: return 25.0;
                case Level.Hard: return 0.0;
                default: return 0.0;
            }
        }

        private long getNumberOfShuffels() {
            long cells = rows * cols;
            long cellsFactorial = 1;

            while (cells > 0) {
                cellsFactorial *= cells--;
            }

            cellsFactorial /= 2;

            if (cellsFactorial > 500000) {
                switch (level) {
                    case Level.Easy: return 10000;
                    case Level.Medium: return 100000;
                    case Level.Hard: return 500000;
                    default: return 10000;
                }
            }

            switch (level) {
                case Level.Easy: return cellsFactorial * 1 / 3;
                case Level.Medium: return cellsFactorial * 2 / 3;
                case Level.Hard: return cellsFactorial * 3 / 3;
                default: return cellsFactorial;
            }
        }

        private void swapTiles(Point p1, Point p2) {
            int i1 = (int)p1.X, i2 = (int)p2.X;
            int j1 = (int)p1.Y, j2 = (int)p2.Y;

            Tile temp = this.arr[i1, j1];
            this.arr[i1, j1] = this.arr[i2, j2];
            this.arr[i2, j2] = temp;

            TilesMoved?.Invoke(temp, p2, this.arr[i1, j1], p1);
        }

        private Point getTileInDirection(Point p, int direction) {
            int i = (int)p.X, j = (int)p.Y;

            switch (direction) {
                case 1:
                    return new Point(i - 1, j);
                case 2:
                    return new Point(i, j + 1);
                case 3:
                    return new Point(i + 1, j);
                case 4:
                    return new Point(i, j - 1);
                default:
                    throw new Exception("Direction isn't between 1 - 4");
            }
        }

        private int getRightDirection(Point p, Random rand) {
            int i = (int)p.X, j = (int)p.Y;

            if (i == 0) {
                if (j == 0) /* up left */
                    return rand.Next(2, 4);
                if (j == cols - 1) /* up right*/
                    return rand.Next(3, 5);
                return rand.Next(2, 5); /* middle first row*/
            }

            if (j == 0) {
                if (i == rows - 1) /* buttom left*/
                    return rand.Next(1, 3);
                return rand.Next(1, 4); /* middle first col */
            }

            if (i == rows - 1) {
                if (j == cols - 1) /* buttom right */
                    return rand.Next(1, 3) == 1 ? 1 : 4;
                int n = rand.Next(1, 4);
                return n == 3 ? 4 : n; /* middle last row*/
            }

            if (j == cols - 1) {
                int n = rand.Next(1, 4);
                return n == 1 ? 1 : n + 1; /* middle last col */
            }

            return rand.Next(1, 5); /* any direction */
        }

    }
}
