using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Class that generates a neighbourgenerator
    /// </summary>
    class NeighbourGeneratorGenerator
    {
        /// <summary>
        /// List of tuples (neighbour generator, chance to use this generator)
        /// </summary>
        public static List<(NeighbourGenerator, double)> usageChance = new List<(NeighbourGenerator, double)>
        {
            (new MoveUpNeighbourGenerator(), 1.0f)
            //(new SwapNeighbourGenerator(), 1.0f)
        };

        /// <summary>
        /// Generates a random neighbourgenerator
        /// </summary>
        /// <param name="r">The random used</param>
        /// <returns>A random neighbourgenerator</returns>
        public static NeighbourGenerator GenerateRandom(Random r)
        {
            double accum = 0;
            double random = r.NextDouble();
            int index = 0;

            // Update the accumulative value until it is equal or larger than the random value.
            // Then return the generator that corresponds to that value.
            while (accum < random)
            {
                double chance = usageChance[index].Item2;
                accum += chance;
                index++;
            }

            return usageChance[index - 1].Item1;
        }
    }


    interface INeighbourSpace<T>
    {
        public Neighbour<T> GetRandomNeighbour(T data);

        public List<Neighbour<T>> GetAllNeighbours(T data);
    }

    class MoveUpNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
    {
        public List<Neighbour<RecursiveTree<Node>>> GetAllNeighbours(RecursiveTree<Node> data)
        {
            throw new NotImplementedException();
        }

        public Neighbour<RecursiveTree<Node>> GetRandomNeighbour(RecursiveTree<Node> data)
        {
            RecursiveTree<Node> root = Program.allRecTreeNodes[0].Root;
            RecursiveTree<Node> randomTree = null;
            while (randomTree == root || randomTree == null)
            {
                randomTree = Program.allRecTreeNodes[Program.random.Next(Program.allRecTreeNodes.Count)];
            }
            List<RecursiveTree<Node>> parents = new List<RecursiveTree<Node>>();
            RecursiveTree<Node> parent = randomTree.Parent.Parent; // Dit werkt dus niet echt als dit een child van de root is dus tja, vandaar de comment hierboven bij de whileloop
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.Parent;
            }
            parents.Add(null); // TODO: Fix, this is supposed to be the root
            RecursiveTree<Node> randomParent = parents[Program.random.Next(parents.Count)];
            return new MoveUpNeighbour(randomTree, randomParent);
        }
    }

    /// <summary>
    /// Abstract class of an object that generates neighbours
    /// </summary>
    abstract class NeighbourGenerator
    {
        /// <summary>
        /// Generates a random new neighbour
        /// </summary>
        /// <param name="state">The current state</param>
        /// <returns>A generated neighbour</returns>
        public abstract OldNeighbour GenerateRandom(OldState state);
        
        /// <summary>
        /// Generates all new neighbours
        /// </summary>
        /// <param name="state">The current state</param>
        /// <returns>All generated neighbours</returns>
        public abstract List<OldNeighbour> GenerateAll(OldState state);
    }

    class SwapNeighbourGenerator : NeighbourGenerator
    {
        public override List<OldNeighbour> GenerateAll(OldState state)
        {
            List<OldNeighbour> generatedNeighbours = new List<OldNeighbour>(state.Tree.Nodes.Count - 1);

            foreach (TreeNode n in state.Tree.Nodes)
            {
                if (n == state.Tree.Root) continue;

                generatedNeighbours.Add(new OldSwapNeighbour(state, n));
            }

            return generatedNeighbours;
        }

        public override OldNeighbour GenerateRandom(OldState state)
        {
            throw new NotImplementedException();
        }
    }

    class MoveUpNeighbourGenerator : NeighbourGenerator
    {
        public override List<OldNeighbour> GenerateAll(OldState state)
        {
            List<OldNeighbour> generatedNeighbours = new List<OldNeighbour>(state.RecTree.NumberOfNodes - 1);

            foreach (Node n in state.allNodes)
            {
                if (n == state.RecTree.Value) continue;

                generatedNeighbours.Add(new OldMoveUpNeighbour(state, n));
            }

            return generatedNeighbours;
        }

        public override OldNeighbour GenerateRandom(OldState state)
        {
            throw new NotImplementedException();
        }
    }
}
