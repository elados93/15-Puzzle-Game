using WpfApp.Model.Solver.Searchable;
using WpfApp.Model.Solver.Misc;
using System.Collections.Generic;

namespace WpfApp.Model.Solver.Searchers {
    public abstract class Searcher<dynamic> : ISearcher {

        private Queue<State<dynamic>> queue;
        private int evaluatedNodes;

        public Searcher() {
            // TODO: Priority Queue?
            this.queue = new Queue<State<dynamic>>();
            this.evaluatedNodes = 0;
        }

        protected State<dynamic> popPriorityQueue() {
            ++evaluatedNodes;
            return queue.Dequeue();
        }

        public Queue<State<dynamic>> Queue {
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