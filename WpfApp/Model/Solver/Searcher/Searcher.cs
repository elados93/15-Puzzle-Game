using WpfApp.Model.Solver.Searchable;
using WpfApp.Model.Solver.Misc;
using Priority_Queue;

namespace WpfApp.Model.Solver.Searchers {
    public abstract class Searcher<dynamic> : ISearcher {

        private FastPriorityQueue<State<dynamic>> queue;
        private int evaluatedNodes;
        private int MAX_VALUE = 100000000;

        public Searcher() {
            // TODO: Priority Queue max capacity?
            this.queue = new FastPriorityQueue<State<dynamic>>(MAX_VALUE);
            this.evaluatedNodes = 0;
        }

        protected State<dynamic> popPriorityQueue() {
            ++evaluatedNodes;
            return queue.Dequeue();
        }

        public FastPriorityQueue<State<dynamic>> Queue {
            get {
                return queue;
            }
        }

        public int getNumberOfNodesEvaluated() {
            return this.evaluatedNodes;
        }

        public abstract Solution search(ISearchable searchable);
    }
}