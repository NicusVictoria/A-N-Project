using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AN_Project
{
    static class IndependentSet
    {
        static List<Node> CalculateMaximum(List<Node> graph)
        {
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
        static RecursiveTree<Node> TreeFromIndependentSets(List<Node> graph) {
            
            List<Node> independentSet = CalculateMaximum(graph);
            List<Node>[] neighBoursPreviousLevel = new List<Node>[graph.Count];
            for (int i = 0; i < neighBoursPreviousLevel.Length; i++)
            {
                neighBoursPreviousLevel[i] = new List<Node>();
            }
            RecursiveTree<Node>[] treesFromNodes = new RecursiveTree<Node>[graph.Count];
            List<Node> newNodes;

            while (true)
            {

                newNodes = new List<Node>(graph.Count - independentSet.Count);

                
                Node[] fromNumber = new Node[graph.Count];
                Node[] oldNode = new Node[graph.Count];


                foreach (Node n in graph)
                {
                    oldNode[n.Number] = n;
                    if (!independentSet.Contains(n))
                    {
                        Node newNode = new Node(n.Number);
                        newNodes.Add(newNode);
                        fromNumber[n.Number] = newNode;
                    }
                    else
                    {
                        treesFromNodes[n.Number] = new RecursiveTree<Node>(n);
                        List<Node> prevNeighbours = neighBoursPreviousLevel[n.Number];
                        foreach(Node m in prevNeighbours)
                        {
                            treesFromNodes[n.Number].AddChild(treesFromNodes[m.Number]);
                            treesFromNodes[m.Number].Parent = treesFromNodes[n.Number];
                        }
                    }
                }


                foreach (Node n in newNodes)
                {
                    foreach (Node m in n.ConnectedNodes)
                    {
                        if (independentSet.Contains(m))
                        {
                            foreach (Node o in m.ConnectedNodes)
                            {
                                neighBoursPreviousLevel[n.Number].Add(m);
                                if (o != n) n.ConnectedNodes.Add(fromNumber[o.Number]);
                            }
                        }
                    }
                }


                independentSet = CalculateMaximum(newNodes);
            }


        }
    }

}
