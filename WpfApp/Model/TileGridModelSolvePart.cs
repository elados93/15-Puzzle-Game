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
        #endregion

        #region Solve Functions
        internal void solve() {
            ISearcher searcher = new BestFirstSearch();
            Solution solution = searcher.search(this);
            solveBySolution(solution);
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
            return new State<dynamic>(sb.ToString(), Direction.NO_DIRECTION);
        }

        public bool isGoalState(State<dynamic> state) {
            string goalState = goalCalaulated ? goalString : createGoalTile();
            string givenState = state.StateProperty;

            /* O(1) Comparison using interns */
            return (object)string.Intern(goalState) == (object)string.Intern(givenState);
        }

        public List<State<dynamic>> getAllPossibleStates(State<dynamic> state) {
            List<State<dynamic>> list = new List<State<dynamic>>();

            string stateString = state.StateProperty;
            string[] stringArr = stateString.Split(';');
            int stateStringLength = stringArr.Length;
            int spaceIndex = Array.FindIndex(stringArr, s => s.Equals(" "));

            /* RIGHT MOVE */
            if (spaceIndex + 1 < stateStringLength && (spaceIndex + 1 + col) % col != 0) {
                string rightMove = swapIndexesInString(stringArr, spaceIndex, spaceIndex + 1);
                list.Add(new State<dynamic>(rightMove, Direction.RIGHT));
            }
            /* LEFT MOVE */
            if ((spaceIndex - 1 == 0) || (spaceIndex - 1 >= 0 && (spaceIndex + col) % col != 0)) {
                string leftMove = swapIndexesInString(stringArr, spaceIndex, spaceIndex - 1);
                list.Add(new State<dynamic>(leftMove, Direction.LEFT));
            }
            /* UP MOVE */
            if (spaceIndex - col >= 0) {
                string upMove = swapIndexesInString(stringArr, spaceIndex, spaceIndex - col);
                list.Add(new State<dynamic>(upMove, Direction.UP));
            }
            /* DOWN MOVE */
            if (spaceIndex + col < stateStringLength) {
                string downMove = swapIndexesInString(stringArr, spaceIndex, spaceIndex + col);
                list.Add(new State<dynamic>(downMove, Direction.DOWN));
            }
            Random r = new Random();
           
            list.Sort( (x, y) => r.Next());
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

        private void solveBySolution(Solution solution) {
            List<State<dynamic>> list = solution.PathOfSolution;

            foreach (State<dynamic> state in list) {
                applyState(state);
                Thread.Sleep(500);
            }
        }

        private void applyState(State<dynamic> state) {

            Direction move = state.Direction;
            int i = (int)spacePoint.X, j = (int)spacePoint.Y;
            Tile tileSpace = arr[i, j], otherTile;

            switch (move) {
                case Direction.UP: otherTile = arr[i - 1, j];
                    swap(otherTile, spacePoint, tileSpace, new Point(i - 1, j));
                    break;
                case Direction.RIGHT: otherTile = arr[i, j + 1];
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

        private string swapIndexesInString(string[] stateString, int i, int j) {
            StringBuilder sb = new StringBuilder();
            int length = stateString.Length;

            for (int k = 0; k < length; k++) {
                if (k == i)
                    sb.Append(stateString[j]);
                else if (k == j)
                    sb.Append(stateString[i]);
                else
                    sb.Append(stateString[k]);
                if (k != length - 1)
                    sb.Append(";");
            }

            return sb.ToString();
        }

        #endregion

        #endregion
    }
}
