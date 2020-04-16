using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AN_Project
{
    public static class FloydWarshall
    {
        public static int[,] AllPairsShortestPath<N>(N[] nodes) where N : INode<N>
        {
            int length = nodes.Length;
            Dictionary<N, int> indexOfNode = new Dictionary<N, int>();
            for (int i = 0; i < length; i++)
            {
                indexOfNode[nodes[i]] = i;
            }
            int[,] distArray = new int[length, length];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    distArray[i, j] = length + 1;
                }
                foreach (N connectedNode in nodes[i].ConnectedNodes)
                {
                    distArray[i, indexOfNode[connectedNode]] = 1;
                }
                distArray[i, i] = 0;
            }

            

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        if (distArray[i, j] > distArray[i, k] + distArray[k, j]) distArray[i, j] = distArray[i, k] + distArray[k, j];
                    }
                }
            }
            return distArray;
        }

        public static N CenterOfGraph<N>(N[] nodes) where N : INode<N>
        {
            int length = nodes.Length;
            int[] maxDists = MaxDist(nodes);
            int min = int.MaxValue;
            N minNode = nodes[0];
            for (int i = 0; i < length; i++)
            {
                if(maxDists[i] < min)
                {
                    min = maxDists[i];
                    minNode = nodes[i];
                }
            }
            return minNode;
        }

        public static int[] MaxDist<N>(N[] nodes) where N : INode<N>
        {
            int length = nodes.Length;
            int[,] allPairsShortestPath = AllPairsShortestPath(nodes);
            int[] maxDists = new int[length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (maxDists[i] < allPairsShortestPath[i, j])
                    {
                        maxDists[i] = allPairsShortestPath[i, j];
                    }
                }
            }
            return maxDists;
        }

        public static int[] CenterResemblances<N>(N[] nodes) where N : INode<N>
        {
            int[] maxDists = MaxDist(nodes);
            int maxMaxDist = maxDists.Max();
            return maxDists.Select(n => maxMaxDist - n).ToArray();
        }
    }
}
