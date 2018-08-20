using System.Collections.Generic;
using WpfApp.Model.Solver.Misc;

namespace WpfApp.Model.Solver.Searchable {
    public interface ISearchable {
        State<dynamic> getInitialState();
        bool isGoalState(State<dynamic> state);
        List<State<dynamic>> getAllPossibleStates(State<dynamic> s);
    }
}
