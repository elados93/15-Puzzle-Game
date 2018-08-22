using System;
using WpfApp.Misc;
using Priority_Queue;
using System.Collections.Generic;

namespace WpfApp.Model.Solver.Misc {
    public class State<T> : FastPriorityQueueNode, IEquatable<State<T>> {

        private static Dictionary<int, State<T>> statePool = new Dictionary<int, State<T>>();

        public State(T state) {
            this.StateProperty = state;
            this.IsInit = false;
        }

        public bool Equals(State<T> other) {
            // TODO: maybe delete?
            //if (state is string)
            //    return (object)string.Intern(this.state.ToString()) == 
            //        (object)string.Intern(other.state.ToString());
            return this.StateProperty.Equals(other.StateProperty);
        }

        public override int GetHashCode() {
            return this.StateProperty.GetHashCode();
        }

        public void updatePriority() {
            this.Priority = Heuristic + Cost;
        }

        #region Properties
        public State<T> CameFrom { get; set; }

        public T StateProperty { get; }

        public Direction Direction { get; set; }

        public int Cost { get; set; }

        public int Heuristic { get; set; }

        public bool IsInit { get; set; }
        #endregion

        public static class StatePool {

            public static State<T> GetState(T state) {

                int code = state.ToString().GetHashCode();

                if (statePool.ContainsKey(code))
                    return statePool[code];
                else {
                    State<T> newState = new State<T>(state);
                    statePool.Add(code, newState);
                    return newState;
                }
            }

            public static void Clear() {
                statePool.Clear();
            }
        }
    }
}