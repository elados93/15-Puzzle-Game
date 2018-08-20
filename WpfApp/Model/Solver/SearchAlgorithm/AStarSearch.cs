using System.Collections.Generic;
using WpfApp.Model.Solver.Misc;
using WpfApp.Model.Solver.Searchable;
using WpfApp.Model.Solver.Searchers;

namespace WpfApp.Model.Solver.SearchAlgorithm {
    class AStarSearch : Searcher<dynamic> {

        private HashSet<State<dynamic>> closedHashSet;

        public AStarSearch() {
            this.closedHashSet = new HashSet<State<dynamic>>();
        }

        public override Solution search(ISearchable searchable) {
            List<State<dynamic>> list = null;

            State<dynamic> initState = searchable.getInitialState();
            Queue.Enqueue(initState, initState.Priority);

            State<dynamic> currentState;
            while (Queue.Count > 0) {
                currentState = popPriorityQueue(); // Pop the first state wer'e in
                closedHashSet.Add(currentState); // Mark as visited

                if (searchable.isGoalState(currentState)) {
                    list = backtrace(currentState);
                    break;
                } else {
                    /* List of all successors of the current state*/
                    List<State<dynamic>> successors = searchable.getAllPossibleStates(currentState);

                    foreach (State<dynamic> s in successors) {
                        if (!closedHashSet.Contains(s) && !Queue.Contains(s)) {
                            s.CameFrom = currentState;
                            Queue.Enqueue(s, s.Priority);
                        }
                    }
                }
            } // end while loop
            
            return new Solution(list, this.getNumberOfNodesEvaluated());
        }

        private List<State<dynamic>> backtrace(State<dynamic> goalState) {
            List<State<dynamic>> list = new List<State<dynamic>>();

            State<dynamic> runnerState = goalState;
            while (runnerState != null) {
                list.Insert(0, runnerState);
                runnerState = runnerState.CameFrom;
            }
            list.RemoveAt(0); // the first is the init state, doesn't really need it.
            return list;
        }

    }
}
