using System;
using System.Collections.Generic;
using WpfApp.Misc;
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
                    return new Solution(list, list.Count);
                }

                /* List of all successors of the current state*/
                List<Tuple<State<dynamic>, Direction>> successors = searchable.getAllPossibleStates(currentState);

                foreach (Tuple<State<dynamic>, Direction> tuple in successors) {
                    State<dynamic> neighbor = tuple.Item1;
                    if (!closedHashSet.Contains(neighbor) && !Queue.Contains(neighbor)) {

                        Queue.Enqueue(neighbor, neighbor.Priority);

                    } else {
                        if (neighbor.Priority > currentState.Cost + 1 + neighbor.Heuristic) {
                            neighbor.CameFrom = currentState;
                            neighbor.Direction = tuple.Item2;
                            neighbor.Cost = currentState.Cost + 1;
                            neighbor.updatePriority();

                            if (!Queue.Contains(neighbor))
                                Queue.Enqueue(neighbor, neighbor.Priority);
                        }
                    }
                } // end successors for loop
            } // end while loop

            throw new System.Exception("Cannot be solved with A* algorithm!");
        }

        private List<State<dynamic>> backtrace(State<dynamic> goalState) {
            List<State<dynamic>> list = new List<State<dynamic>>();

            State<dynamic> runnerState = goalState;
            while (!runnerState.IsInit) {
                list.Insert(0, runnerState);
                runnerState = runnerState.CameFrom;
            }
            return list;
        }

    }
}
