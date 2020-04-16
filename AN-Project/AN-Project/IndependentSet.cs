using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AN_Project
{
    /// <summary>
    /// Class that can calculate independent sets
    /// </summary>
    static class IndependentSet
    {
        /// <summary>
        /// Compute a RecursiveTree built by using maximal independent sets
        /// </summary>
        /// <param name="inputGraph">The input graph</param>
        /// <param name="timer">A RUNNING! timer to keep track of the total time this computation takes</param>
        /// <param name="maxTimeAllowed">The total time the computation is allowed to take. If this time is exceeded, all remaining nodes are placed in a single line on top of the tree with independent sets computed thus far</param>
        /// <returns>A RecursiveTree built by using maximal independent sets</returns>
        public static RecursiveTree<Node> TreeFromIndependentSets(Node[] inputGraph, Stopwatch timer, int maxTimeAllowed)
        {
            List<Node> graph = new List<Node>(inputGraph);
            List<Node> independentSet = CalculateMaximal(graph);
            Node[] oldNodes = new Node[graph.Count];
            List<Node>[] neighboursPreviousLevel = new List<Node>[graph.Count];
            for (int i = 0; i < neighboursPreviousLevel.Length; i++)
            {
                neighboursPreviousLevel[i] = new List<Node>();
                oldNodes[graph[i].Number - 1] = graph[i];
            }
            RecursiveTree<Node>[] treesFromNodes = new RecursiveTree<Node>[graph.Count];
            List<Node> newNodes = graph;
            List<Node> oldNewNodes;

            do
            {
                oldNewNodes = new List<Node>(newNodes);

                // The independent set contains nodes from the last layer (1 layer down in the tree)
                // Nodes in this specific layer; each layer has its own nodes. These nodes are not in the independent set
                newNodes = new List<Node>(newNodes.Count - independentSet.Count);

                // Dictionary from nodenumber to node in the current layer
                Node[] fromNumber = new Node[graph.Count];

                // Compute the subtrees for the nodes in the independent set and compute the new layer of nodes
                ComputeSubtreesAndNextLayer(oldNewNodes, oldNodes, independentSet, newNodes, fromNumber, treesFromNodes, neighboursPreviousLevel);

                // Compute the new connections for the next layer
                int totalEdges = 0;
                ComputeNewConnections(newNodes, oldNodes, fromNumber, independentSet, neighboursPreviousLevel, ref totalEdges);

                // If the remaining nodes form a clique or we are taking too long, put the remaining nodes in a line and return
                if (newNodes.Count * (newNodes.Count - 1) / 2 == totalEdges || timer.Elapsed.TotalSeconds >= maxTimeAllowed)
                {
                    RecursiveTree<Node> cliqueTreeLeaf = CreateLine(newNodes);
                    foreach (RecursiveTree<Node> tree in treesFromNodes)
                    {
                        if (tree != null && tree.Parent == null)
                        {
                            cliqueTreeLeaf.AddChild(tree);
                            tree.Parent = cliqueTreeLeaf;
                        }
                    }
                    return cliqueTreeLeaf.Root;
                }

                // Calculate new independent set for next iteration
                independentSet = CalculateMaximal(newNodes);
            } while (newNodes.Count > 0);

            return treesFromNodes[oldNewNodes[0].Number - 1];
        }

        /// <summary>
        /// Computes the new nodes for the next layer and creates subtrees for the nodes that are currently in the independent set
        /// </summary>
        /// <param name="oldNewNodes">The nodes in this layer</param>
        /// <param name="oldNodes">The original nodes</param>
        /// <param name="independentSet">The nodes in the independent set of this layer</param>
        /// <param name="newNodes">The nodes that are not in the independent set of this layer, these are the nodes that are the new nodes in the next layer</param>
        /// <param name="fromNumber">Dictionary from a node number to the node in the previous layer</param>
        /// <param name="treesFromNodes">Dictionary from a node number to its corresponding RecursiveTree</param>
        /// <param name="neighboursPreviousLevel">The neigbours of a node on the previous level</param>
        private static void ComputeSubtreesAndNextLayer(List<Node> oldNewNodes, Node[] oldNodes, List<Node> independentSet, List<Node> newNodes, Node[] fromNumber, RecursiveTree<Node>[] treesFromNodes, List<Node>[] neighboursPreviousLevel)
        {
            foreach (Node n in oldNewNodes)
            {
                oldNodes[n.Number - 1] = n;
                if (!independentSet.Contains(n))
                {
                    // If the not was not in the independent set in the last layer, it is present in this layer
                    Node newNode = new Node(n.Number, n.CenterResemblance)
                    {
                        ConnectedNodes = new List<Node>()
                    };
                    newNodes.Add(newNode);
                    fromNumber[n.Number - 1] = newNode;
                }
                else
                {
                    // If it was in the independent set, it is now a node in the this layer of the tree
                    treesFromNodes[n.Number - 1] = new RecursiveTree<Node>(n);

                    // Add the nodes in the previous layer that this node is connected to as children to this node, and this node as their parent
                    List<Node> prevNeighbours = neighboursPreviousLevel[n.Number - 1];

                    foreach (Node m in prevNeighbours)
                    {
                        if (treesFromNodes[m.Number - 1].Parent == null)
                        {
                            treesFromNodes[m.Number - 1].Parent = treesFromNodes[n.Number - 1];
                            treesFromNodes[n.Number - 1].AddChild(treesFromNodes[m.Number - 1]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Computes the connections in the next layer of the graph. If edges AB and BC existed, and B is in de independent set, edge AC is added. Old connections are still present
        /// </summary>
        /// <param name="newNodes">The list of nodes in the next layer</param>
        /// <param name="oldNodes">The original nodes</param>
        /// <param name="fromNumber">Dictionary from the number of a node to the node in the previous layer</param>
        /// <param name="independentSet">The independent set of this layer</param>
        /// <param name="neighboursPreviousLevel">The array of lists of neighbours on the previous layer. These are changed in this method</param>
        /// <param name="totalEdges">Refernce parameter for the total number of edges in the next layer. Used when determining whether the next layer is a clique</param>
        private static void ComputeNewConnections(List<Node> newNodes, Node[] oldNodes, Node[] fromNumber, List<Node> independentSet, List<Node>[] neighboursPreviousLevel, ref int totalEdges)
        {
            // Connect the nodes in this layer via nodes in the independent set
            foreach (Node n in newNodes)
            {
                foreach (Node m in oldNodes[n.Number - 1].ConnectedNodes)
                {
                    if (newNodes.Contains(fromNumber[m.Number - 1]) && !n.ConnectedNodes.Contains(fromNumber[m.Number - 1]))
                    {
                        totalEdges++;
                        n.ConnectedNodes.Add(fromNumber[m.Number - 1]);
                    }
                    if (independentSet.Contains(m))
                    {
                        // Save for each node to which nodes in the independent set it is connected; used in the next iteration
                        neighboursPreviousLevel[n.Number - 1].Add(m);

                        foreach (Node o in m.ConnectedNodes)
                        {
                            // A node that is not in the independent set (n), with a neighbour that is in the independent set (m)
                            // gets connected to all neighbours (o) of (m)
                            if (o.Number != n.Number && !n.ConnectedNodes.Contains(fromNumber[o.Number - 1]))
                            {
                                totalEdges++;
                                n.ConnectedNodes.Add(fromNumber[o.Number - 1]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a subtree in the form of a single line from a list of nodes
        /// </summary>
        /// <param name="nodes">The nodes to be in the subtree</param>
        /// <returns>The leaf of the subtree with the nodes in a single line</returns>
        private static RecursiveTree<Node> CreateLine(List<Node> nodes)
        {
            RecursiveTree<Node> leaf = new RecursiveTree<Node>(nodes[0]);
            RecursiveTree<Node> child = leaf;
            for (int i = 1; i < nodes.Count; i++)
            {
                RecursiveTree<Node> node = new RecursiveTree<Node>(nodes[i]);
                node.AddChild(child);
                child.Parent = node;
                child = node;
            }
            return leaf;
        }

        /// <summary>
        /// Calcuate a maximal independent set
        /// </summary>
        /// <param name="graph">The graph for which to calculate the independent set</param>
        /// <returns>A list of nodes in the independent set</returns>
        private static List<Node> CalculateMaximal(List<Node> graph)
        {
            // Order the nodes so the best node to take is first in the list
            graph = graph.OrderBy(n => n.Degree + n.CenterResemblance).ToList();
            List<Node> retList = new List<Node>();
            HashSet<Node> coveredNodes = new HashSet<Node>();
            for (int i = 0; i < graph.Count; i++)
            {
                // Grab a node, and if it is not covered, add it to the result and cover all its neighbours
                Node n = graph[i];
                if (!coveredNodes.Contains(n))
                {
                    retList.Add(n);
                    foreach (Node neighbour in n.ConnectedNodes)
                    {
                        coveredNodes.Add(neighbour);
                    }
                }
            }
            return retList;
        }

        /// <summary>
        /// Calculates a maximum independent set on a graph
        /// </summary>
        /// <param name="graph">The graph for which to calculate the independent set</param>
        /// <returns>A list of nodes in the independent set</returns>
        private static List<Node> CalculateMaximum(List<Node> graph)
        {
            if (graph.Count == 0) return new List<Node>();
            if (graph.Count == 1) return graph;

            Node mostConnectedNode = ComputeMostConnectedNode(graph);

            // Compute the value if the mostConnectedNode is used
            List<Node> usedList = new List<Node>(graph);
            foreach (Node n in mostConnectedNode.ConnectedNodes)
            {
                usedList.Remove(n);
            }
            usedList.Remove(mostConnectedNode);
            List<Node> valueIfUsed = CalculateMaximum(usedList);

            // Compute the value is the mostConnectedNode is not used
            List<Node> notUsedList = new List<Node>(graph);
            notUsedList.Remove(mostConnectedNode);
            List<Node> valueIfNotUsed = CalculateMaximum(notUsedList);

            // Return the best possible option of the 2
            if (valueIfUsed.Count > valueIfNotUsed.Count)
            {
                valueIfUsed.Add(mostConnectedNode);
                return valueIfUsed;
            }
            return valueIfNotUsed;
        }

        /// <summary>
        /// Compute the node with the highest (remaining) degree in a (sub)graph
        /// </summary>
        /// <param name="graph">A list of all nodes in the graph</param>
        /// <returns>The node with the highest remaining degree in this (sub)graph</returns>
        private static Node ComputeMostConnectedNode(List<Node> graph)
        {
            HashSet<Node> graphAsHash = new HashSet<Node>(graph);
            Node mostConnectedNode = null;
            int mostConnections = 0;
            foreach (Node n in graph)
            {
                int degree = n.RemainingDegree(graphAsHash);
                if (degree > mostConnections)
                {
                    mostConnections = degree;
                    mostConnectedNode = n;
                }
            }
            return mostConnectedNode;
        }
    }
}
