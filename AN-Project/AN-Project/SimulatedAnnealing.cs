using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    class SimulatedAnnealing
    {
        /// <summary>
        /// Uses Tabu Search to find a solution
        /// </summary>
        /// <returns>The state corresponding to the final solution</returns>
        public State Search()
        {
            RecursiveTree<Node> recTree = BaseSolutionGenerator.EmptyRecursiveTree();
            State state = new State(Program.allNodes);
            state.RecTree = recTree;

            double bestTotalScore = state.RecTree.ScoreKeeper.CurrentScore;
            State bestTotalState = state.Clone();

            int i = 0;
            while (true)
            {
                i++;

                List<Neighbour> neighbours = new List<Neighbour>();
                foreach ((NeighbourGenerator ng, double chance) in NeighbourGeneratorGenerator.usageChance)
                {
                    neighbours.AddRange(ng.GenerateAll(state));
                }

                double bestNewScore = -1;

                // TODO: parallelise this stuff
                foreach (Neighbour neighbour in neighbours)
                {
                    State newState = neighbour.Result();
                    double newScore = newState.RecTree.ScoreKeeper.CurrentScore;

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
