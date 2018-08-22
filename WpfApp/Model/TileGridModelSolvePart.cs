using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string goalString;
        private Random r = new Random();
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
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++) {
                    sb.Append(this.arr[i, j].Value);
                    if (i != row - 1 || j != col - 1)
                        sb.Append(";");
                }
            }
            string initStateString = sb.ToString();
            State<dynamic> initState = State<dynamic>.StatePool.GetState(initStateString);
            initState.IsInit = true;
            initState.Heuristic = calcHeuristic(initStateString);
            initState.Cost = 0;
            initState.Direction = Direction.NO_DIRECTION;
            initState.updatePriority();
            return initState;
        }

        public bool isGoalState(State<dynamic> state) {
            string goalState = goalCalaulated ? goalString : createGoalTile();
            string givenState = state.StateProperty;

            return goalState.Equals(givenState);
        }

        public List<Tuple<State<dynamic>, Direction>> getAllPossibleStates(State<dynamic> state) {
            var list = new List<Tuple<State<dynamic>, Direction>>();

            string stateString = state.StateProperty;
            string[] stringArr = stateString.Split(';');
            int stateStringLength = stringArr.Length;
            int spaceIndex = Array.FindIndex(stringArr, s => s.Equals(" "));
            int newCost = state.Cost + 1;
            int oldStateHeuristic = state.Heuristic;

            /* RIGHT MOVE */
            if (spaceIndex + 1 < stateStringLength && (spaceIndex + 1 + col) % col != 0) {
                string rightMove = swapIndexesInString(stringArr, spaceIndex, spaceIndex + 1);
                State<dynamic> rightState = State<dynamic>.StatePool.GetState(rightMove);
                // if the state is new calc the heuristic smartly
                if (rightState.CameFrom == null) {
                    rightState.Direction = Direction.RIGHT;
                    rightState.Cost = newCost;
                    rightState.Heuristic = calcHeuristicByMove(oldStateHeuristic, stringArr, spaceIndex, spaceIndex + 1);
                    rightState.updatePriority();
                    rightState.CameFrom = state;
                }
                list.Add(new Tuple<State<dynamic>, Direction>(rightState, Direction.RIGHT));
            }
            /* LEFT MOVE */
            if ((spaceIndex - 1 == 0) || (spaceIndex - 1 >= 0 && (spaceIndex + col) % col != 0)) {
                string leftMove = swapIndexesInString(stringArr, spaceIndex, spaceIndex - 1);
                State<dynamic> leftState = State<dynamic>.StatePool.GetState(leftMove);
                if (leftState.CameFrom == null) {
                    leftState.Direction = Direction.LEFT;
                    leftState.Cost = newCost;
                    leftState.Heuristic = calcHeuristicByMove(oldStateHeuristic, stringArr, spaceIndex, spaceIndex - 1);
                    leftState.updatePriority();
                    leftState.CameFrom = state;
                }
                list.Add(new Tuple<State<dynamic>, Direction>(leftState, Direction.LEFT));
            }
            /* UP MOVE */
            if (spaceIndex - col >= 0) {
                string upMove = swapIndexesInString(stringArr, spaceIndex, spaceIndex - col);
                State<dynamic> upState = State<dynamic>.StatePool.GetState(upMove);
                if (upState.CameFrom == null) {
                    upState.Direction = Direction.UP;
                    upState.Cost = newCost;
                    upState.Heuristic = calcHeuristicByMove(oldStateHeuristic, stringArr, spaceIndex, spaceIndex - col);
                    upState.updatePriority();
                    upState.CameFrom = state;
                }
                list.Add(new Tuple<State<dynamic>, Direction>(upState, Direction.UP));
            }
            /* DOWN MOVE */
            if (spaceIndex + col < stateStringLength) {
                string downMove = swapIndexesInString(stringArr, spaceIndex, spaceIndex + col);
                State<dynamic> downState = State<dynamic>.StatePool.GetState(downMove);
                if (downState.CameFrom == null) {
                    downState.Direction = Direction.DOWN;
                    downState.Cost = newCost;
                    downState.Heuristic = calcHeuristicByMove(oldStateHeuristic, stringArr, spaceIndex, spaceIndex + col);
                    downState.updatePriority();
                    downState.CameFrom = state;
                }
                list.Add(new Tuple<State<dynamic>, Direction>(downState, Direction.DOWN));
            }

            return list;
        }

        #region Helpers
        private string createGoalTile() {
            StringBuilder sb = new StringBuilder();
            int count = 1;

            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++)
                    if (i != row - 1 || j != col - 1) {
                        sb.Append(count.ToString());
                        sb.Append(";");
                        ++count;
                    } else
                        sb.Append(" ");
            }
            goalCalaulated = true;
            goalString = sb.ToString();
            return goalString;
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

        private string swapIndexesInString(string[] stringArr, int i, int j) {

            stringArr[i] = stringArr[j];
            stringArr[j] = " ";
            string rightMove = string.Join(";", stringArr);

            stringArr[j] = stringArr[i];
            stringArr[i] = " ";

            return rightMove;
            //StringBuilder sb = new StringBuilder();
            //int length = stateString.Length;

            //for (int k = 0; k < length; k++) {
            //    if (k == i)
            //        sb.Append(stateString[j]);
            //    else if (k == j)
            //        sb.Append(stateString[i]);
            //    else
            //        sb.Append(stateString[k]);
            //    if (k != length - 1)
            //        sb.Append(";");
            //}

            //return sb.ToString();
        }

        public int calcHeuristic(string stateString) {
            int sum = 0;
            string[] stringArr = stateString.Split(';');
            int index = 0;

            for (int i = 0; i < row; ++i) {
                for (int j = 0; j < col; j++) {

                    int correctX, correctY;
                    string stringValue = stringArr[index++];

                    if (!stringValue.Equals(" ")) {
                        int integerValue = int.Parse(stringValue);

                        correctX = (integerValue - 1) / col;
                        correctY = (integerValue - 1) % col;
                    } else {
                        correctX = row - 1;
                        correctY = col - 1;
                    }

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

            int heuristicBeforeMove = calcHeuristicByPlace(" ", xPosBefore, yPosBefore) +
                calcHeuristicByPlace(valueToSwapWithSpace, xPosAfter, yPosAfter);

            int heuristicAfterMove = calcHeuristicByPlace(" ", xPosAfter, yPosAfter) +
                calcHeuristicByPlace(valueToSwapWithSpace, xPosBefore, yPosBefore);

            return heuristic - heuristicBeforeMove + heuristicAfterMove;
        }

        private int calcHeuristicByPlace(string value, int i, int j) {
            int correctY, correctX;

            if (!value.Equals(" ")) {
                int integerValue = int.Parse(value);

                correctX = (integerValue - 1) / col;
                correctY = (integerValue - 1) % col;
            } else {
                correctX = row - 1;
                correctY = col - 1;
            }

            return calcDifference(i, j, correctX, correctY);
        }

        private int calcDifference(int i, int j, int correctX, int correctY) {
            return Math.Abs(i - correctX) + Math.Abs(j - correctY);
        }
        #endregion

        #endregion
    }
}
