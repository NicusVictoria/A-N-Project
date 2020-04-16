using System;
using System.Diagnostics;

namespace AN_Project
{
    /// <summary>
    /// Runs simulated annealing on a state
    /// </summary>
    /// <typeparam name="S">The type of State being used</typeparam>
    /// <typeparam name="D">The type of data being used in the state</typeparam>
    class SimulatedAnnealing<S, D> where S : State<D>
    {
        /// <summary>
        /// An arbitrary random used
        /// </summary>
        private Random Random { get; }

        /// <summary>
        /// The start temperature
        /// </summary>
        private double StartTemperature { get; }

        /// <summary>
        /// When the temperature comes below this number, reset it to its start value
        /// </summary>
        private double ResetTemperatureThreshold { get; }

        /// <summary>
        /// When the temperature reaches a certain number of iterations, multiply it by this value
        /// </summary>
        private double TemperatureMultiplier { get; }

        /// <summary>
        /// After this many iterations the temperature gets multiplied by the multiplier
        /// </summary>
        private int MultiplyTemperaturePerIterations { get; }

        /// <summary>
        /// A timer to be used to keep track of the total running time
        /// </summary>
        private Stopwatch Timer { get; }

        /// <summary>
        /// The maximum time the search is allowed to take
        /// </summary>
        private double MaxTimeAllowed { get; }

        /// <summary>
        /// Constructor for Simulated Annealing
        /// </summary>
        /// <param name="random">The random to be used</param>
        /// <param name="startTemperature">The start temperature to be used by the program</param>
        /// <param name="resetTemperatureThreshold">Reset the temperature when it is smaller than this value</param>
        /// <param name="temperatureMultiplier">Multiply the temperature by this value every X iterations</param>
        /// <param name="multiplyTemperaturePerIterations">Per how many iterations the temperature should be multiplied</param>
        /// <param name="timer">A RUNNING! timer to keep track of how long the simulated annealing is allowed to take</param>
        /// <param name="maxTimeAllowed">The maximum amount of time the search is allowed to take</param>
        public SimulatedAnnealing(Random random, double startTemperature, double resetTemperatureThreshold, double temperatureMultiplier, int multiplyTemperaturePerIterations, Stopwatch timer, double maxTimeAllowed)
        {
            Random = random;
            StartTemperature = startTemperature;
            ResetTemperatureThreshold = resetTemperatureThreshold;
            TemperatureMultiplier = temperatureMultiplier;
            MultiplyTemperaturePerIterations = multiplyTemperaturePerIterations;
            Timer = timer;
            MaxTimeAllowed = maxTimeAllowed;
        }

        /// <summary>
        /// Starts searching for an optimal solution
        /// </summary>
        /// <param name="initialState">The start state from where to search</param>
        /// <param name="bestStateSoFar">Reference where the result (and updates) are supposed to be stored</param>
        public void Search(S initialState, ref string bestStateSoFar)
        {
            S state = initialState;
            double bestTotalScore = initialState.Score;
            double previousScore = bestTotalScore;
            double temperature = StartTemperature;
            int iterations = 0;
            
            while (MaxTimeAllowed > Timer.Elapsed.TotalSeconds)
            {
                iterations++;

                // Get a random neighbour and apply it
                Neighbour<D> neighbour = state.GetRandomNeighbour();
                state.ApplyNeighbour(neighbour);
                double newScore = state.Score;

                // Update the scores and best solution if necessary
                if (newScore <= previousScore)
                {
                    if(newScore < bestTotalScore)
                    {
                        bestStateSoFar = state.Data.ToString();
                        bestTotalScore = newScore;
                    }
                    previousScore = newScore;
                }
                else
                {
                    // There is a chance the neighbour is reverted
                    double chance = Math.Exp((previousScore - newScore) / temperature);
                    double random = Random.NextDouble();
                    if (random > chance)
                    {
                        state.RevertNeighbour(neighbour);
                    }
                    else
                    {
                        previousScore = newScore;
                    }
                }

                // Reset the temperature if the threshold has been reached
                if(temperature < ResetTemperatureThreshold)
                {
                    temperature = StartTemperature;
                }

                // Multiply the temperature if needed
                if (iterations % MultiplyTemperaturePerIterations == 0)
                {
                    temperature *= TemperatureMultiplier;
                }
            }
        }
    }
}
