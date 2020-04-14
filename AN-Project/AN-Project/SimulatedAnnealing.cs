using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;

namespace AN_Project
{
    class SimulatedAnnealing<T, D> where T : State<D>
    {
        private bool SigtermReceived { get; set; }

        public SimulatedAnnealing()
        {
            SigtermReceived = false;
            AssemblyLoadContext.Default.Unloading += OnSigTermReceived;
        }

        private void OnSigTermReceived(AssemblyLoadContext obj)
        {
            SigtermReceived = true;
        }

        /// <summary>
        /// Uses Tabu Search to find a solution
        /// </summary>
        /// <returns>The state corresponding to the final solution</returns>
        public string Search(T initialState)
        {
            T state = initialState;

            double bestTotalScore = initialState.Score;
            string bestTotalState = initialState.Data.ToString();

            int i = 0;

            return bestTotalState; // TODO: test only

            while (!SigtermReceived)
            {
                i++;

                Neighbour<D> neighbour = state.GetRandomNeighbour();

                //double bestNewScore = state.Score;
                state.ApplyNeighbour(neighbour);
                double newScore = state.Score;

                //if (newScore > bestNewScore)
                //{
                if (newScore <= bestTotalScore)
                {
                    bestTotalState = state.Data.ToString();
                    bestTotalScore = newScore;
                }
                //}
                else
                {
                    state.RevertNeighbour(neighbour);
                }

                if (i == 100)
                {
                    return bestTotalState;
                }
            }
            return bestTotalState;
        }
    }
}
