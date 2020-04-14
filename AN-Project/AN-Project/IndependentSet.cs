using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AN_Project
{
    static class IndependentSet
    {

        static List<Node> CalculateMaximal(List<Node> graph)
        {
            graph.Sort();
            List<Node> retList = new List<Node>();
            HashSet<Node> tabu = new HashSet<Node>();
            for(int i = 0; i < graph.Count; i++)
            {
                Node n = graph[i];
                if (!tabu.Contains(n))
                {
                    retList.Add(n);
                    foreach(Node neighbour in n.ConnectedNodes)
                    {
                        tabu.Add(neighbour);
                    }
                }
            }
            return retList;
        }

        static List<Node> CalculateMaximum(List<Node> graph)
        {
            if (graph.Count == 0) return new List<Node>();
            if (graph.Count == 1) return graph; 
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
            List<Node> usedList = new List<Node>(graph);
            foreach(Node n in mostConnectedNode.ConnectedNodes)
            {
                usedList.Remove(n);
            }
            usedList.Remove(mostConnectedNode);
            List<Node> valueIfUsed = CalculateMaximum(usedList);

            List<Node> notUsedList = new List<Node>(graph);
            notUsedList.Remove(mostConnectedNode);
            List<Node> valueIfNotUsed = CalculateMaximum(notUsedList);
            
            if(valueIfUsed.Count + 1>= valueIfNotUsed.Count)
            {
                valueIfUsed.Add(mostConnectedNode);
                return valueIfUsed;
            }
            return valueIfNotUsed;
        }

        public static RecursiveTree<Node> TreeFromIndependentSets(List<Node> inputGraph, Stopwatch timer)
        {
            List<Node> graph = new List<Node>(inputGraph);
            List<Node> independentSet = CalculateMaximal(graph);
            Node[] oldNode = new Node[graph.Count];
            List<Node>[] neighboursPreviousLevel = new List<Node>[graph.Count];
            for (int i = 0; i < neighboursPreviousLevel.Length; i++)
            {
                neighboursPreviousLevel[i] = new List<Node>();
                oldNode[graph[i].Number - 1] = graph[i];
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

                foreach (Node n in oldNewNodes)
                {
                    oldNode[n.Number - 1] = n;
                    if (!independentSet.Contains(n))
                    {
                        // If the not was not in the independent set in the last layer, it is present in this layer
                        Node newNode = new Node(n.Number);
                        newNode.ConnectedNodes = new List<Node>();
                        newNodes.Add(newNode);
                        fromNumber[n.Number - 1] = newNode;
                    }
                    else
                    {
                        // If it was in the independent set, it is now a node in the this layer of the tree
                        treesFromNodes[n.Number - 1] = new RecursiveTree<Node>(n);

                        // Add the nodes in the previous layer that this node is connected to as children to this node, and this node as their parent
                        List<Node> prevNeighbours = neighboursPreviousLevel[n.Number - 1];

                        if (prevNeighbours.Count == 0)
                        {

                        }

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

                int totalEdges = 0;

                // Connect the nodes in this layer via nodes in the independent set
                foreach (Node n in newNodes)
                {
                    //neighboursPreviousLevel[n.Number - 1] = new List<Node>();
                    bool isConnectedToAnIndependentSetNode = false;
                    foreach (Node m in oldNode[n.Number - 1].ConnectedNodes)
                    {
                        if (newNodes.Contains(fromNumber[m.Number - 1]) && !n.ConnectedNodes.Contains(fromNumber[m.Number - 1]))
                        {
                            totalEdges++;
                            n.ConnectedNodes.Add(fromNumber[m.Number - 1]);
                        }
                        if (independentSet.Contains(m))
                        {
                            isConnectedToAnIndependentSetNode = true;
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
                    if (!isConnectedToAnIndependentSetNode) throw new Exception("wa");
                }

                if (newNodes.Count * (newNodes.Count - 1) / 2 == totalEdges || timer.Elapsed.TotalSeconds >= Program.MaxTimeSeconds)
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
        /// Creates a subtree in the form of a single line from a list of nodes
        /// </summary>
        /// <param name="nodes">The nodes to be in the subtree</param>
        /// <returns>A subtree with the nodes in a single line</returns>
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
    }
}
