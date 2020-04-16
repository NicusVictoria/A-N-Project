using System;
using System.Collections.Generic;

namespace AN_Project
{
    /// <summary>
    /// Interface for a neighbourspace
    /// </summary>
    /// <typeparam name="T">The type of data neighbours should work with</typeparam>
    interface INeighbourSpace<T>
    {
        /// <summary>
        /// Get all neighbours from the given data
        /// </summary>
        /// <param name="data">The data to create all neighbours from</param>
        /// <returns>A list of all neighbours for this data</returns>
        List<Neighbour<T>> GetAllNeighbours(T data);

        /// <summary>
        /// Get a random neighbour from the given data
        /// </summary>
        /// <param name="data">The data to create a neighbour from</param>
        /// <returns>A random neighbour</returns>
        Neighbour<T> GetRandomNeighbour(T data);
    }

    /// <summary>
    /// Neighbourspace that generates a neighbour where a node has been moved up into the tree
    /// </summary>
    class MoveUpNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
    {
        /// <summary>
        /// The random used
        /// </summary>
        private Random Random { get; }

        private double[] CumulativeHeuristic { get; }

        /// <summary>
        /// Constructor for the MoveUpNeighbourSpace
        /// </summary>
        /// <param name="random">The random to be used</param>
        public MoveUpNeighbourSpace(Random random, double[] cumulativeHeuristic)
        {
            Random = random;
            CumulativeHeuristic = cumulativeHeuristic;
        }

        public List<Neighbour<RecursiveTree<Node>>> GetAllNeighbours(RecursiveTree<Node> data)
        {
            throw new NotImplementedException(); // TODO: implement
        }

        public Neighbour<RecursiveTree<Node>> GetRandomNeighbour(RecursiveTree<Node> data)
        {
            /* // TODO: might not be used
            // Grab a random tree that is not the root
            RecursiveTree<Node> root = Program.allRecTreeNodes[0].Root;
            RecursiveTree<Node> randomTree = null;
            while (randomTree == root || randomTree == null)
            {
                randomTree = Program.allRecTreeNodes[Random.Next(Program.allRecTreeNodes.Count)];
            }

            // Grab a random ancestor of this tree
            List<RecursiveTree<Node>> ancestors = new List<RecursiveTree<Node>>();
            RecursiveTree<Node> parent = randomTree.Parent.Parent;
            while (parent != null)
            {
                ancestors.Add(parent);
                parent = parent.Parent;
            }
            ancestors.Add(null);
            RecursiveTree<Node> randomParent = ancestors[Random.Next(ancestors.Count)];

            // Return a new neighbour with the randomly found tree and one of its ancestors
            return new MoveUpNeighbour(randomTree, randomParent);
            */

            // Grab a random tree that is not the root
            RecursiveTree<Node> root = Program.allRecTreeNodes[0].Root;
            RecursiveTree<Node> randomTree = null;
            while (randomTree == root || randomTree == null)
            {
                randomTree = GetGuidedMoveUpNode();
            }

            // Grab a random ancestor of this tree
            List<RecursiveTree<Node>> ancestors = new List<RecursiveTree<Node>>();
            RecursiveTree<Node> parent = randomTree.Parent.Parent;
            while (parent != null)
            {
                ancestors.Add(parent);
                parent = parent.Parent;
            }
            ancestors.Add(null);
            RecursiveTree<Node> randomParent = ancestors[Random.Next(ancestors.Count)];

            // Return a new neighbour with the randomly found tree and one of its ancestors
            return new MoveUpNeighbour(randomTree, randomParent);
        }

        private RecursiveTree<Node> GetGuidedMoveUpNode()
        {
            double totalHeuristic = CumulativeHeuristic[CumulativeHeuristic.Length - 1];
            double random = Random.NextDouble() * totalHeuristic;
            int index = Array.BinarySearch(CumulativeHeuristic, random);
            if (index < 0) index = -1 - index;
            return Program.allRecTreeNodes[index];
        }
    }

    /// <summary>
    /// Neighbourspace that generates a neighbour by splitting its subtrees into multiple smaller subtrees
    /// </summary>
    class SplitNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
    {
        /// <summary>
        /// The random used
        /// </summary>
        private Random Random { get; }

        private double[] CumulativeHeuristic { get; }

        /// <summary>
        /// Constructor for a SplitNeighbourSpace
        /// </summary>
        /// <param name="random">The random to be used</param>
        public SplitNeighbourSpace(Random random, double[] cumulativeHeuristic)
        {
            Random = random;
            CumulativeHeuristic = cumulativeHeuristic;
        }

        public List<Neighbour<RecursiveTree<Node>>> GetAllNeighbours(RecursiveTree<Node> data)
        {
            throw new NotImplementedException(); // TODO: implement
        }

        public Neighbour<RecursiveTree<Node>> GetRandomNeighbour(RecursiveTree<Node> data)
        {
            // Grab a random tree that is not a child and return it as the node to be split
            RecursiveTree<Node> randomTree = null;
            while (randomTree == null || randomTree.Children.Count == 0)
            {
                randomTree = GetGuidedSplitNode();//Program.allRecTreeNodes[Random.Next(Program.allRecTreeNodes.Count)];
            }
            return new SplitNeighbour(randomTree);
        }

        private RecursiveTree<Node> GetGuidedSplitNode()
        {
            double totalHeuristic = CumulativeHeuristic[CumulativeHeuristic.Length - 1];
            double random = Random.NextDouble() * totalHeuristic;
            int index = Array.BinarySearch(CumulativeHeuristic, random);
            if (index < 0) index = -1 - index;
            return Program.allRecTreeNodes[index];
        }
    }

    /// <summary>
    /// Neighbourspace that combines a move and a split
    /// </summary>
    class MoveAndSplitNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
    {
        /// <summary>
        /// The random used
        /// </summary>
        private Random Random { get; }

        /// <summary>
        /// A neighbourspace to generate MoveUpNeighbours from
        /// </summary>
        private MoveUpNeighbourSpace MoveUpNeighbourSpace { get; }

        /// <summary>
        /// Constructor for a MoveAndSplitNeighbourSpace
        /// </summary>
        /// <param name="random">The random to be used</param>
        public MoveAndSplitNeighbourSpace(Random random, double[] cumulativeHeuristic)
        {
            Random = random;
            MoveUpNeighbourSpace = new MoveUpNeighbourSpace(Random, cumulativeHeuristic);
        }

        public List<Neighbour<RecursiveTree<Node>>> GetAllNeighbours(RecursiveTree<Node> data)
        {
            throw new NotImplementedException(); // TODO: implement
        }

        public Neighbour<RecursiveTree<Node>> GetRandomNeighbour(RecursiveTree<Node> data)
        {
            // Get a random MoveNeighbour, create a new SplitNeighbour on the node that has just been moved up and return the combination of these two
            MoveUpNeighbour move = MoveUpNeighbourSpace.GetRandomNeighbour(data) as MoveUpNeighbour;
            SplitNeighbour split = new SplitNeighbour(move.MoveNode.Parent);
            List<Neighbour<RecursiveTree<Node>>> neighbours = new List<Neighbour<RecursiveTree<Node>>>() { move, split };
            return new MultipleNeighbourNeighbour<RecursiveTree<Node>>(neighbours);
        }
    }

    /// <summary>
    /// Neighbourspace for a neighbour that moves one node up, and splits on a few random nodes
    /// </summary>
    class RandomMoveAndSplitNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
    {
        /// <summary>
        /// The random used
        /// </summary>
        private Random Random { get; }

        /// <summary>
        /// The number of times to split
        /// </summary>
        private int Splits { get; }

        /// <summary>
        /// A neighbourspace to generate MoveUpNeighbours from
        /// </summary>
        private MoveUpNeighbourSpace MoveUpNeighbourSpace { get; }

        /// <summary>
        /// A neighbourspace to generate SplitNeighbours from
        /// </summary>
        private SplitNeighbourSpace SplitNeighbourSpace { get; }

        /// <summary>
        /// Constructor for the RandomMoveAndSplitNeighbourSpace
        /// </summary>
        /// <param name="random">The random to be used</param>
        /// <param name="splits">The number SplitNeighbours to be generated</param>
        public RandomMoveAndSplitNeighbourSpace(Random random, int splits, double[] cumulativeHeuristic)
        {
            Random = random;
            Splits = splits;
            MoveUpNeighbourSpace = new MoveUpNeighbourSpace(Random, cumulativeHeuristic);
            SplitNeighbourSpace = new SplitNeighbourSpace(Random, cumulativeHeuristic);
        }

        public List<Neighbour<RecursiveTree<Node>>> GetAllNeighbours(RecursiveTree<Node> data)
        {
            throw new NotImplementedException(); // TODO: implement
        }

        public Neighbour<RecursiveTree<Node>> GetRandomNeighbour(RecursiveTree<Node> data)
        {
            List<Neighbour<RecursiveTree<Node>>> neighbourlist = new List<Neighbour<RecursiveTree<Node>>>();

            // Create a random MoveUpNeighbour
            Neighbour<RecursiveTree<Node>> move = MoveUpNeighbourSpace.GetRandomNeighbour(data);
            neighbourlist.Add(move);

            // Generate Splits amount of random SplitNeighbours
            for (int i = 0; i < Splits; i++)
            {
                Neighbour<RecursiveTree<Node>> split = SplitNeighbourSpace.GetRandomNeighbour(data);
                neighbourlist.Add(split);
            }

            // Return the list with the single Move and multiple SplitNeighbours
            return new MultipleNeighbourNeighbour<RecursiveTree<Node>>(neighbourlist);
        }
    }
}