using WpfApp.Model.Solver.Searchable;

namespace WpfApp.Model.Solver.Searchers {
    interface ISearcher {
        Solution search(ISearchable searchable);
        int getNumberOfNodesEvaluated();
    }
}
