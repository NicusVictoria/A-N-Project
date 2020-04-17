using System;
using System.Collections.Generic;

namespace AN_Project
{
    /// <summary>
    /// A state with all the information needed to run local search
    /// </summary>
    /// <typeparam name="S">The type of data structure used for the data</typeparam>
    abstract class State<S>
    {
        /// <summary>
        /// The data itself
        /// </summary>
        public S Data { get; protected set; }

        /// <summary>
        /// The score of this state
        /// </summary>
        public virtual double Score { get; protected set; }

        /// <summary>
        /// List of the possible neighbourspaces and their chance
        /// </summary>
        protected abstract List<Tuple<INeighbourSpace<S>, double>> NeighbourSpaces { get; }

        /// <summary>
        /// The random used
        /// </summary>
        protected Random Random { get; }

        /// <summary>
        /// Constructor for a state
        /// </summary>
        /// <param name="data">The initial data to use</param>
        /// <param name="random">An arbitrary instance of random</param>
        public State(S data, Random random)
        {
            Data = data;
            Random = random;
        }

        /// <summary>
        /// Takes a random neighbourspace according to their chance to be selected
        /// </summary>
        /// <returns>A random neighbourspace</returns>
        protected INeighbourSpace<S> GetRandomNeighbourSpace()
        {
            if (NeighbourSpaces.Count == 0) throw new Exception("No neighbourspaces to choose from");

            // Calculate a random value and keep iterating over the possible neighbourspaces until this value is reached
            double random = Random.NextDouble();
            double accum = 0;
            int index = 0;
            do
            {
                accum += NeighbourSpaces[index].Item2;
                index++;
            } while (accum < random);

            // Return the corresponding neighbourspace
            return NeighbourSpaces[index - 1].Item1;
        }

        /// <summary>
        /// Calculates a random neighbourspace, and from that neighbourspace a random neighbour
        /// </summary>
        /// <returns>A random neighbour</returns>
        public Neighbour<S> GetRandomNeighbour()
        {
            INeighbourSpace<S> neighbourSpace = GetRandomNeighbourSpace();
            return neighbourSpace.GetRandomNeighbour(Data);
        }

        /// <summary>
        /// Returns all neighbours from all neighbourspaces
        /// </summary>
        /// <returns>A list with all possible neighbours</returns>
        public List<Neighbour<S>> GetAllNeighbours()
        {
            List<Neighbour<S>> allNeighbours = new List<Neighbour<S>>();
            foreach (Tuple<INeighbourSpace<S>, double> tuple in NeighbourSpaces)
            {
                allNeighbours.AddRange(tuple.Item1.GetAllNeighbours(Data));
            }
            return allNeighbours;
        }

        /// <summary>
        /// Generates a random neighbour and applies it to the current state
        /// </summary>
        /// <returns>The neighbour that was applied</returns>
        public Neighbour<S> ApplyRandomNeighbour()
        {
            Neighbour<S> neighbour = GetRandomNeighbour();
            ApplyNeighbour(neighbour);
            return neighbour;
        }

        /// <summary>
        /// Applies a neighbour
        /// </summary>
        /// <param name="neighbour">The neigbour to apply</param>
        public virtual void ApplyNeighbour(Neighbour<S> neighbour)
        {
            neighbour.Apply();
            //Score += neighbour.Delta(); // TODO: use the delta again
        }

        /// <summary>
        /// Reverts a neighbour
        /// </summary>
        /// <param name="neighbour">The neighbour to revert</param>
        public virtual void RevertNeighbour(Neighbour<S> neighbour)
        {
            neighbour.Revert();
            //Score -= neighbour.Delta(); // TODO: use the delta again
        }
    }

    /// <summary>
    /// State that uses a recursive tree with nodes as data
    /// </summary>
    class RecursiveTreeState : State<RecursiveTree<Node>>
    {
        /// <summary>
        /// List of all RecursiveTrees in this instance
        /// </summary>
        private List<RecursiveTree<Node>> AllRecTreeNodes { get; }

        public override double Score { get => Data.Root.Depth; }

        private double[] CumulativeHeuristic { get; }

        /// <summary>
        /// Constructor of the RecursiveTreeState
        /// </summary>
        /// <param name="Data">The initial data to use</param>
        /// <param name="random">The random to be used</param>
        /// <param name="allRecTreeNodes">List of all RecursiveTrees in this instance</param>
        public RecursiveTreeState(RecursiveTree<Node> Data, Random random, List<RecursiveTree<Node>> allRecTreeNodes) : base(Data, random)
        {
            AllRecTreeNodes = allRecTreeNodes;
            CumulativeHeuristic = new double[AllRecTreeNodes.Count];
            CalculateCumulativeHeuristic();
        }

        private void CalculateCumulativeHeuristic()
        {
            double counter = 0;
            for (int i = 0; i < AllRecTreeNodes.Count; i++)
            {
                counter += AllRecTreeNodes[i].Value.Heuristic;
                CumulativeHeuristic[i] = counter;
            }
        }

        protected override List<Tuple<INeighbourSpace<RecursiveTree<Node>>, double>> NeighbourSpaces
        {
            get
            {
                return new List<Tuple<INeighbourSpace<RecursiveTree<Node>>, double>>()
                {
                    new Tuple<INeighbourSpace<RecursiveTree<Node>>, double>(new SplitNeighbourSpace(AllRecTreeNodes, Random, CumulativeHeuristic), 0.2f),
                    new Tuple<INeighbourSpace<RecursiveTree<Node>>, double>(new MoveUpNeighbourSpace(AllRecTreeNodes, Random, CumulativeHeuristic), 0.2f),
                    new Tuple<INeighbourSpace<RecursiveTree<Node>>, double>(new MoveAndSplitNeighbourSpace(AllRecTreeNodes, Random, CumulativeHeuristic), 0.1f),
                    new Tuple<INeighbourSpace<RecursiveTree<Node>>, double>(new RandomMoveAndSplitNeighbourSpace(AllRecTreeNodes, Random, 3, CumulativeHeuristic), 0.5f)
                };
            }
        }

        public override void ApplyNeighbour(Neighbour<RecursiveTree<Node>> neighbour)
        {
            base.ApplyNeighbour(neighbour);
            
            // Set the data to be the root again; needed because of some nodes that were moved
            Data = Data.Root;
        }

        public override void RevertNeighbour(Neighbour<RecursiveTree<Node>> neighbour)
        {
            base.RevertNeighbour(neighbour);

            // Set the data to be the root again; needed because of some nodes that were moved
            Data = Data.Root;
        }
    }
}
