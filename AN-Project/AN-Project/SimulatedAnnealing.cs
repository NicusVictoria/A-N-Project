using System.IO;
using System;
using System.Diagnostics;

namespace AN_Project
{
    class SimulatedAnnealing<T, D> where T : State<D>
    {
        /// <summary>
        /// Uses Tabu Search to find a solution
        /// </summary>
        /// <returns>The state corresponding to the final solution</returns>
        public string Search(T initialState, Stopwatch timer)
        {
            T state = initialState;

            double bestTotalScore = initialState.Score;
            Program.bestStateSoFar = initialState.Data.ToString();

            int i = 0;
            double prevScore = bestTotalScore;

            double Temp = Program.START_TEMP;

            //return Program.bestStateSoFar; // TODO: test only

            while (Program.MAX_TIME_TOTAL_SECONDS > timer.Elapsed.TotalSeconds)
            {
                i++;

                Neighbour<D> neighbour = state.GetRandomNeighbour();

                //double bestNewScore = state.Score;
                state.ApplyNeighbour(neighbour);
                double newScore = state.Score;

                //if (newScore > bestNewScore)
                //{
                if (newScore <= prevScore)
                {
                    if(newScore <= bestTotalScore)
                    {
                        Program.bestStateSoFar = state.Data.ToString();
                        bestTotalScore = newScore;
                    }
                    prevScore = newScore;
                }
                //}
                else
                {
                    double chance = Math.Exp((prevScore - newScore) / Temp);
                    double random = Program.random.NextDouble();
                    if (random > chance)
                    {
                        state.RevertNeighbour(neighbour);
                    }
                    else
                    {
                        prevScore = newScore;
                    }
                }

                if(Temp < 0.05)
                {
                    Temp = Program.START_TEMP;
                }

                if (i % 1000 == 0)
                {
                    Temp *= Program.TEMP_MULTIPLIER_PER_THOUSAND;
                }

            }
            return Program.bestStateSoFar;
        }
    }
}
