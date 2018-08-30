using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using WpfApp.Misc;
using WpfApp.Model.Solver;
using WpfApp.Model.Solver.Misc;
using WpfApp.Model.Solver.Searchable;
using WpfApp.Model.Solver.SearchAlgorithm;
using WpfApp.Model.Solver.Searchers;

namespace WpfApp.Model {
    public partial class TileGridModel : INotifyPropertyChanged, ISearchable {
        #region Member
        private bool goalCalaulated;
        private string[] goalStringArr;
        private Random rand = new Random();
        #endregion

        #region Solve Functions
        internal Solution solve() {
            GC.Collect(); // TODO: consider using or not

            ISearcher searcher = new AStarSearch();
            Solution solution = searcher.search(this);
            State<dynamic>.StatePool.Clear();
            return solution;
        }

        internal void solveBySolution(Solution solution) {
            List<State<dynamic>> list = solution.PathOfSolution;

            foreach (State<dynamic> state in list) {
                applyState(state);
                Thread.Sleep(250); // as a delay between swaps
            }
            this.tracker.updateSolved();
        }

        public State<dynamic> getInitialState() {
            string[] goalStringArr = new string[row * col];
            int index = 0;

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                    goalStringArr[index++] = this.arr[i, j].Value;

            State<dynamic> initState = State<dynamic>.StatePool.GetState(goalStringArr);
            initState.IsInit = true;
            initState.Heuristic = calcHeuristic(goalStringArr);
            initState.Cost = 0;
            initState.Direction = Direction.NO_DIRECTION;
            return initState;
        }

        public bool isGoalState(State<dynamic> state) {
            string[] goalState = goalCalaulated ? goalStringArr : createGoalTile();
            string[] givenState = state.StateProperty;

            return Enumerable.SequenceEqual(goalState, givenState);
        }

        public List<Tuple<State<dynamic>, Direction>> getAllPossibleStates(State<dynamic> state) {
            var list = new List<Tuple<State<dynamic>, Direction>>();

            string[] stringArr = state.StateProperty;
            int stateStringLength = stringArr.Length;
            int spaceIndex = Array.FindIndex(stringArr, s => s.Equals(" "));
            int newCost = state.Cost + 1;
            int oldStateHeuristic = state.Heuristic;

            /* RIGHT MOVE */
            if (state.Direction != Direction.LEFT)
                if (spaceIndex + 1 < stateStringLength && (spaceIndex + 1 + col) % col != 0)
                    addStateToList(list, state, stringArr, spaceIndex, spaceIndex + 1,
                        oldStateHeuristic, Direction.RIGHT, newCost);

            /* LEFT MOVE */
            if (state.Direction != Direction.RIGHT)
                if ((spaceIndex - 1 == 0) || (spaceIndex - 1 >= 0 && (spaceIndex + col) % col != 0))
                    addStateToList(list, state, stringArr, spaceIndex, spaceIndex - 1,
                        oldStateHeuristic, Direction.LEFT, newCost);

            /* UP MOVE */
            if (state.Direction != Direction.DOWN)
                if (spaceIndex - col >= 0)
                    addStateToList(list, state, stringArr, spaceIndex, spaceIndex - col,
                        oldStateHeuristic, Direction.UP, newCost);

            /* DOWN MOVE */
            if (state.Direction != Direction.UP)
                if (spaceIndex + col < stateStringLength)
                    addStateToList(list, state, stringArr, spaceIndex, spaceIndex + col,
                        oldStateHeuristic, Direction.DOWN, newCost);

            list.Sort((x, y) => rand.Next());
            return list;
        }

        private void addStateToList(List<Tuple<State<dynamic>, Direction>> list, State<dynamic> cameFrom, string[] stringArr, int spaceIndex, int otherTileIndex,
            int oldStateHeuristic, Direction direction, int newCost) {
            string[] swappedStringArr = swapIndexesInString(stringArr, spaceIndex, otherTileIndex);
            State<dynamic> state = State<dynamic>.StatePool.GetState(swappedStringArr);
            // if the state is new, calc the heuristic smartly
            if (state.CameFrom == null) {
                state.CameFrom = cameFrom;
                state.Direction = direction;
                state.Cost = newCost;
                state.Heuristic = calcHeuristicByMove(oldStateHeuristic, stringArr, spaceIndex, otherTileIndex);
            }
            list.Add(new Tuple<State<dynamic>, Direction>(state, direction));
        }

        #region Helpers
        private string[] createGoalTile() {
            goalStringArr = new string[row * col];
            int count = 1, index = 0;

            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++)
                    if (i != row - 1 || j != col - 1) {
                        goalStringArr[index++] = count.ToString();
                        ++count;
                    } else
                        goalStringArr[index++] = " ";
            }
            goalCalaulated = true;
            return goalStringArr;
        }

        private void applyState(State<dynamic> state) {

            Direction move = state.Direction;
            int i = (int)spacePoint.X, j = (int)spacePoint.Y;
            Tile tileSpace = arr[i, j], otherTile;

            switch (move) {
                case Direction.UP:
                    otherTile = arr[i - 1, j];
                    swap(otherTile, spacePoint, tileSpace, new Point(i - 1, j));
                    break;
                case Direction.RIGHT:
                    otherTile = arr[i, j + 1];
                    swap(otherTile, spacePoint, tileSpace, new Point(i, j + 1));
                    break;
                case Direction.DOWN:
                    otherTile = arr[i + 1, j];
                    swap(otherTile, spacePoint, tileSpace, new Point(i + 1, j));
                    break;
                case Direction.LEFT:
                    otherTile = arr[i, j - 1];
                    swap(otherTile, spacePoint, tileSpace, new Point(i, j - 1));
                    break;
                default:
                    break;
            }
        }

        private string[] swapIndexesInString(string[] stringArr, int i, int j) {
            string[] swapped = new string[stringArr.Length];
            Array.Copy(stringArr, swapped, stringArr.Length);
            swapped[i] = swapped[j];
            swapped[j] = " ";

            return swapped;
        }

        public int calcHeuristic(string[] stringArr) {
            int sum = 0, index = 0; ;

            for (int i = 0; i < row; ++i) {
                for (int j = 0; j < col; j++) {

                    int correctX, correctY;
                    string stringValue = stringArr[index++];

                    if (!stringValue.Equals(" ")) {
                        int integerValue = int.Parse(stringValue);

                        correctX = (integerValue - 1) / col;
                        correctY = (integerValue - 1) % col;
                    } else
                        continue;

                    sum += calcDifference(i, j, correctX, correctY);
                }
            }
            return sum;
        }

        private int calcHeuristicByMove(int heuristic, string[] stateString, int spaceIndex, int newSpaceIndex) {
            string valueToSwapWithSpace = stateString[newSpaceIndex];

            int xPosBefore = spaceIndex / col;
            int yPosBefore = (spaceIndex + col) % col;

            int xPosAfter = newSpaceIndex / col;
            int yPosAfter = (newSpaceIndex + col) % col;

            int heuristicBeforeMove = calcHeuristicByPlace(valueToSwapWithSpace, xPosAfter, yPosAfter);

            int heuristicAfterMove = calcHeuristicByPlace(valueToSwapWithSpace, xPosBefore, yPosBefore);

            return heuristic - heuristicBeforeMove + heuristicAfterMove;
        }

        private int calcHeuristicByPlace(string value, int i, int j) {
            int correctY, correctX;

            if (!value.Equals(" ")) {
                int integerValue = int.Parse(value);

                correctX = (integerValue - 1) / col;
                if (integerValue - 1 > 0)
                    correctY = (integerValue - 1) % col;
                else
                    correctY = 0;
            } else
                return 0;

            return calcDifference(i, j, correctX, correctY);
        }

        private int calcDifference(int i, int j, int correctX, int correctY) {
            return Math.Abs(i - correctX) + Math.Abs(j - correctY);
        }
        #endregion

        #endregion
    }
}
