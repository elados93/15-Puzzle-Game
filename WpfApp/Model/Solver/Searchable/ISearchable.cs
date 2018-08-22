using System;
using System.Collections.Generic;
using WpfApp.Misc;
using WpfApp.Model.Solver.Misc;

namespace WpfApp.Model.Solver.Searchable {
    public interface ISearchable {
        State<dynamic> getInitialState();
        bool isGoalState(State<dynamic> state);
        List<Tuple<State<dynamic>, Direction>> getAllPossibleStates(State<dynamic> s);

        int calcHeuristic(string stateString);
    }
}
