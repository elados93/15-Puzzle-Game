using System;
using WpfApp.Misc;
using Priority_Queue;

namespace WpfApp.Model.Solver.Misc {
    public class State<T> : FastPriorityQueueNode, IEquatable<State<T>> {

        private T state;
        private State<T> cameFrom;
        private float cost;
        private float heuristic;
        private Direction direction;

        public State(T s, Direction directionOfState, float heuristic, float cost) {
            this.state = s;
            this.direction = directionOfState;
            this.Cost = cost;
            this.heuristic = heuristic;
            this.Priority = cost + heuristic;
        }

        public bool Equals(State<T> other) {
            //if (state is string)
            //    return (object)string.Intern(this.state.ToString()) == 
            //        (object)string.Intern(other.state.ToString());
            return this.state.Equals(other.state);
        }

        public override int GetHashCode() {
            return this.state.GetHashCode();
        }

        public State<T> CameFrom {
            get {
                return cameFrom;
            }

            set {
                this.cameFrom = value;
            }
        }

        public T StateProperty {
            get {
                return state;
            }
        }

        public Direction Direction {
            get {
                return direction;
            }

            set {
                this.direction = value;
            }
        }

        public float Cost {
            get {
                return cost;
            }

            set {
                this.cost = value;
            }
        }
    }
}
