using System.Collections.Generic;
using System.Linq;

namespace AN_Project
{
    /// <summary>
    /// Implementation of the Flowd-Warshall algorithm
    /// </summary>
    public static class FloydWarshall
    {
        /// <summary>
        /// Computes all pairs shortest path for the given nodes. The indices in the result are the indices of the nodes in the nodes parameter
        /// </summary>
        /// <typeparam name="N">The type of nodes</typeparam>
        /// <param name="nodes">The nodes to compute the all pairs shortest path for</param>
        /// <returns></returns>
        public static int[,] AllPairsShortestPath<N>(N[] nodes) where N : INode<N>
        {
            int length = nodes.Length;
            Dictionary<N, int> indexOfNode = new Dictionary<N, int>();
            int[,] distanceArray = new int[length, length];

            // Save the index of each node in a dictionary
            for (int i = 0; i < length; i++)
            {
                indexOfNode[nodes[i]] = i;
            }
            
            // Distance from a node to itself is 0, edges are given distance 1 and the rest gets distance length + 1, which is larger than the largest possible distance
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    distanceArray[i, j] = length + 1;
                }
                foreach (N connectedNode in nodes[i].ConnectedNodes)
                {
                    distanceArray[i, indexOfNode[connectedNode]] = 1;
                }
                distanceArray[i, i] = 0;
            }

            // Actually compute all pairs shortest path
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    for (int k = 0; k < length; k++)
                    {
                        int newDistance = distanceArray[i, k] + distanceArray[k, j];
                        if (distanceArray[i, j] > newDistance)
                        {
                            distanceArray[i, j] = newDistance;
                        }
                    }
                }
            }

            return distanceArray;
        }

        /// <summary>
        /// Computes the center of a graph. For each node, calculate the maximum distance to all other nodes. The center of the graph is the node with the minimum of these maximum distances. The indices in the result are the indices of the nodes in the nodes parameter
        /// </summary>
        /// <typeparam name="N">The type of nodes used</typeparam>
        /// <param name="nodes">The nodes to compute the center from</param>
        /// <returns>The node that is the center of the graph</returns>
        public static N CenterOfGraph<N>(N[] nodes) where N : INode<N>
        {
            int length = nodes.Length;

            // For each node, find its maximum distance to any other node
            int[] maxDistances = MaximumDistance(nodes);

            // Find the node with the smallest biggest distance
            int min = int.MaxValue;
            N minNode = nodes[0];
            for (int i = 0; i < length; i++)
            {
                if(maxDistances[i] < min)
                {
                    min = maxDistances[i];
                    minNode = nodes[i];
                }
            }

            return minNode;
        }

        /// <summary>
        /// For each node, compute the biggest distance to all other nodes. The indices in the result are the indices of the nodes in the nodes parameter
        /// </summary>
        /// <typeparam name="N">The type of nodes used</typeparam>
        /// <param name="nodes">The nodes to compute the maximum distance for</param>
        /// <returns>An array with for each node the maximum distance to any other node</returns>
        public static int[] MaximumDistance<N>(N[] nodes) where N : INode<N>
        {
            int length = nodes.Length;

            // Compute all pairs shortest path
            int[,] allPairsShortestPath = AllPairsShortestPath(nodes);

            // For each node, save its biggest distance to any other node
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

        /// <summary>
        /// Computes for each node how much it resembles a center of the graph. A higher value means it resembles a center more. The indices in the result are the indices of the nodes in the nodes parameter
        /// </summary>
        /// <typeparam name="N">The type of nodes used</typeparam>
        /// <param name="nodes">The nodes to compute the center recemblance for</param>
        /// <returns>For each node a value for how much it resembles a center of the graph</returns>
        public static int[] CenterResemblances<N>(N[] nodes) where N : INode<N>
        {
            // Compute the maximum distance for each node
            int[] maximumDistances = MaximumDistance(nodes);

            // Select the node with the biggest maximum distance. This node resembles the center least of all nodes
            int maximumMaximumDistance = maximumDistances.Max();

            // Return for each node the overall maximum distance minus its own maximum distance
            return maximumDistances.Select(node => maximumMaximumDistance - node).ToArray();
        }
    }
}
