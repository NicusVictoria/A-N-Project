using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Tabu search
    /// </summary>
    class TabuSearcher
    {
        /// <summary>
        /// Uses Tabu Search to find a solution
        /// </summary>
        /// <returns>The state corresponding to the final solution</returns>
        public OldState Search()
        {
            OldState state = BaseSolutionGenerator.EmptyState();

            double bestTotalScore = state.Tree.Score;
            OldState bestTotalState = state.Clone();

            int i = 0;
            while (true)
            {
                i++;
                 
                List<OldNeighbour> neighbours = new List<OldNeighbour>();
                foreach ((NeighbourGenerator ng, double chance) in NeighbourGeneratorGenerator.usageChance)
                {
                     neighbours.AddRange(ng.GenerateAll(state));
                }

                double bestNewScore = -1;

                // TODO: parallelise this stuff
                foreach (OldNeighbour neighbour in neighbours)
                {
                    OldState newState = neighbour.Result();
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

                if (i == 100)
                {
                    return bestTotalState;
                }
            }
        }
    }
}
