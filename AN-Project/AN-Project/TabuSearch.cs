using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Tabu search
    /// </summary>
    class TabuSearch
    {
        /// <summary>
        /// Uses Tabu Search to find a solution
        /// </summary>
        /// <returns>The state corresponding to the final solution</returns>
        public State Search()
        {
            State state = BaseSolutionGenerator.Empty();

            double bestTotalScore = state.Tree.Score;
            State bestTotalState = state.Clone();
            
            while (true)
            {
                List<Neighbour> neighbours = new List<Neighbour>();
                foreach ((NeighbourGenerator ng, float f) tuple in NeighbourGeneratorGenerator.usageChance)
                {
                     neighbours.AddRange(tuple.ng.GenerateAll(state));
                }

                double bestNewScore = -1;

                // TODO: parallelise this stuff
                foreach (Neighbour neighbour in neighbours)
                {
                    State newState = neighbour.Result();
                    double newScore = newState.Tree.Score;

                    if (newScore > bestNewScore)
                    {
                        state = newState;
                        bestNewScore = newScore;

                        if (newScore > bestTotalScore)
                        {
                            bestTotalState = newState;
                            bestTotalScore = newScore;
                        }
                    }
                }
            }
        }
    }
}
