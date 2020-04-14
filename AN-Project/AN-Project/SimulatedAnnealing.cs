namespace AN_Project
{
    class SimulatedAnnealing<T, D> where T : State<D>
    {
        /// <summary>
        /// Uses Tabu Search to find a solution
        /// </summary>
        /// <returns>The state corresponding to the final solution</returns>
        public string Search(T initialState)
        {
            T state = initialState;

            double bestTotalScore = initialState.Score;
            Program.bestStateSoFar = initialState.Data.ToString();

            int i = 0;

            return Program.bestStateSoFar; // TODO: test only

            while (true)
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
                    Program.bestStateSoFar = state.Data.ToString();
                    bestTotalScore = newScore;
                }
                //}
                else
                {
                    state.RevertNeighbour(neighbour);
                }

                if (i == 100)
                {
                    return Program.bestStateSoFar;
                }
            }
            return Program.bestStateSoFar;
        }
    }
}
