using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AN_Project
{
    static class IndependentSet
    {
        private static List<Node> CalculateMaximum(List<Node> graph)
        {
            if (graph.Count == 0) return new List<Node>();
            if (graph.Count == 1) return graph; 
            HashSet<Node> graphAsHash = new HashSet<Node>(graph);
            Node mostConnectedNode = null;
            int mostConnections = -1;
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
            
            if(valueIfUsed.Count + 1 >= valueIfNotUsed.Count)
            {
                valueIfUsed.Add(mostConnectedNode);
                return valueIfUsed;
            }
            return valueIfNotUsed;
        }

        public static RecursiveTree<Node> TreeFromIndependentSets(List<Node> graph) {
            
            List<Node> independentSet = CalculateMaximum(graph);
            Node[] oldNode = new Node[graph.Count];
            List<Node>[] neighboursPreviousLevel = new List<Node>[graph.Count];
            for (int i = 0; i < neighboursPreviousLevel.Length; i++)
            {
                neighboursPreviousLevel[i] = new List<Node>();
                oldNode[graph[i].Number - 1] = graph[i];
            }
            RecursiveTree<Node>[] treesFromNodes = new RecursiveTree<Node>[graph.Count];
            List<Node> newNodes = graph;

            do
            {
                List<Node> oldNewNodes = new List<Node>(newNodes);

                // The independent set contains nodes from the last layer (1 layer down in the tree)
                // Nodes in this specific layer; each layer has its own nodes. These nodes are not in the independent set
                newNodes = new List<Node>(newNodes.Count - independentSet.Count);

                // Dictionary from nodenumber to node in the current layer
                Node[] fromNumber = new Node[graph.Count];

                foreach (Node n in oldNewNodes)
                {
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
                        foreach (Node m in prevNeighbours)
                        {
                            treesFromNodes[n.Number - 1].AddChild(treesFromNodes[m.Number - 1]);
                            treesFromNodes[m.Number - 1].Parent = treesFromNodes[n.Number - 1];
                        }
                    }
                }

                // Connect the nodes in this layer via nodes in the independent set
                foreach (Node n in newNodes)
                {
                    foreach (Node m in oldNode[n.Number - 1].ConnectedNodes)
                    {
                        if (independentSet.Contains(m))
                        {
                            // Save for each node to which nodes in the independent set it is connected; used in the next iteration
                            neighboursPreviousLevel[n.Number - 1].Add(m);

                            foreach (Node o in m.ConnectedNodes)
                            {
                                // A node that is not in the independent set (n), with a neighbour that is in the independent set (m)
                                // gets connected to all neighbours (o) of (m)
                                if (o != n) n.ConnectedNodes.Add(fromNumber[o.Number - 1]);
                            }
                        }
                        if (newNodes.Contains(fromNumber[m.Number - 1]))
                        {
                            n.ConnectedNodes.Add(fromNumber[m.Number - 1]);
                        }
                    }
                }
                // Calculate new independent set for next iteration
                independentSet = CalculateMaximum(newNodes);
            } while (newNodes.Count > 1);
            return treesFromNodes[newNodes[0].Number - 1];
        }
    }
}
