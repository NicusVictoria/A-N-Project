using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    class NeighbourGeneratorGenerator
    {
        static List<(NeighbourGenerator, double)> usageChance = new List<(NeighbourGenerator, double)>
        {

        };

        public static NeighbourGenerator GenerateRandom(Random r)
        {
            double accum = 0;
            double random = r.NextDouble();
            int index = 0;
            while (accum < random)
            {
                double chance = usageChance[index].Item2;
                accum += chance;
                index++;
            }
            return usageChance[index - 1].Item1;
        }
    }

    abstract class NeighbourGenerator
    {
        public abstract Neighbour Generate(State s);
    }
}
