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
            (new SwapNeighbourGenerator(), 1.0f)
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
        public abstract Neighbour GenerateRandom(State state);
        
        /// <summary>
        /// Generates all new neighbours
        /// </summary>
        /// <param name="state">The current state</param>
        /// <returns>All generated neighbours</returns>
        public abstract List<Neighbour> GenerateAll(State state);
    }

    class SwapNeighbourGenerator : NeighbourGenerator
    {
        public override List<Neighbour> GenerateAll(State state)
        {
            List<Neighbour> generatedNeighbours = new List<Neighbour>(state.Tree.Nodes.Count - 1);

            foreach (TreeNode n in state.Tree.Nodes)
            {
                if (n == state.Tree.Root) continue;

                generatedNeighbours.Add(new SwapNeighbour(state, n));
            }

            return generatedNeighbours;
        }

        public override Neighbour GenerateRandom(State state)
        {
            throw new NotImplementedException();
        }
    }
}
