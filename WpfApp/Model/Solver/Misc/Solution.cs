﻿using System.Collections.Generic;
using WpfApp.Model.Solver.Misc;

namespace WpfApp.Model.Solver {
    public class Solution {

        private List<State<dynamic>> pathOfSolution;
        private int numberOfSteps;

        public Solution(List<State<dynamic>> list, int steps) {
            this.pathOfSolution = list;
            this.NumberOfSteps = steps;
        }

        public List<State<dynamic>> PathOfSolution {
            get {
                return pathOfSolution;
            }
        }

        public int NumberOfSteps {
            get {
                return numberOfSteps;
            }

            set {
                this.numberOfSteps = value;
            }
        }
    }
}