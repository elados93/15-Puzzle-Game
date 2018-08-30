using System;
using WpfApp.Misc;
using Priority_Queue;
using System.Collections.Generic;

namespace WpfApp.Model.Solver.Misc {
    public class State<T> : FastPriorityQueueNode, IEquatable<State<T>> {

        private static Dictionary<T, State<T>> statePool = new Dictionary<T, State<T>>();
        //private static Hashtable statePool = new Hashtable();

        public State(T state) {
            this.StateProperty = state;
            this.IsInit = false;
        }

        public bool Equals(State<T> other) {
            // TODO: maybe delete?
            //if (state is string)
            //    return (object)string.Intern(this.state.ToString()) == 
            //        (object)string.Intern(other.state.ToString());

            if(StateProperty is string[]) {
                return Array.Equals(other.StateProperty, StateProperty);
            }
            return this.StateProperty.Equals(other.StateProperty);
        }

        public override int GetHashCode() {
            return this.StateProperty.GetHashCode();
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

                //int code = state.GetHashCode();

                if (statePool.ContainsKey(state)) {
                    State<T> fromTable = statePool[state];
                    int code2 = fromTable.Priority.GetHashCode();
                    return fromTable;
                } else {
                    State<T> temp = new State<T>(state);

                    statePool.Add(state, temp);
                    return temp;
                }
            }

            public static void Clear() {
                statePool.Clear();
            }
        }
    }
}