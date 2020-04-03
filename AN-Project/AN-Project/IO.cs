using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AN_Project
{
    class IO
    {
        /// <summary>
        /// Reads the input from a given file
        /// </summary>
        /// <param name="path">The path with the input file</param>
        /// <returns>The undirected graph corresponding to the input</returns>
        public static UndirectedGraph ReadInput(string path)
        {
            int numberOfNodes;
            int numberOfEdges;

            UndirectedGraph graph = new UndirectedGraph();

            // Using the streamreader to read the input file
            using (StreamReader streamReader = new StreamReader(path))
            {
                // Read a line while possible
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    // Split the line
                    string[] splittedLine = line.Split();

                    // If the line is a comment, continue
                    if (splittedLine[0] == "c")
                    {
                        continue;
                    }

                    // If the line contains the information about the nodes and edges, save these and create an empty graph
                    if (splittedLine[0] == "p")
                    {
                        numberOfNodes = int.Parse(splittedLine[2]);
                        numberOfEdges = int.Parse(splittedLine[3]);

                        graph = new UndirectedGraph(numberOfNodes);
                    }
                    // Else the line contains information about an edge, so add this edge to the graph
                    else
                    {
                        int origin = int.Parse(splittedLine[0]) - 1;
                        int destination = int.Parse(splittedLine[1]) - 1;

                        graph.AddEdge(origin, destination);
                    }
                }
            }

            return graph;
        }

        public static Node[] ReadInputAsNodes(string path)
        {
            int numberOfNodes;
            int numberOfEdges;
            Node[] allNodes = new Node[0];

            // Using the streamreader to read the input file
            using (StreamReader streamReader = new StreamReader(path))
            {
                // Read a line while possible
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    // Split the line
                    string[] splittedLine = line.Split();

                    // If the line is a comment, continue
                    if (splittedLine[0] == "c")
                    {
                        continue;
                    }

                    // If the line contains the information about the nodes and edges, save these and create an empty graph
                    if (splittedLine[0] == "p")
                    {
                        numberOfNodes = int.Parse(splittedLine[2]);
                        numberOfEdges = int.Parse(splittedLine[3]);
                        allNodes = new Node[numberOfNodes];
                        for(int i = 0; i < numberOfNodes; i++)
                        {
                            allNodes[i] = new Node(i + 1)
                            {
                                ConnectedNodes = new List<Node>(),
                            };
                        }
                    }
                    // Else the line contains information about an edge, so add this edge to the graph
                    else
                    {
                        int origin = int.Parse(splittedLine[0]) - 1;
                        int destination = int.Parse(splittedLine[1]) - 1;

                        allNodes[origin].ConnectedNodes.Add(allNodes[destination]);
                        allNodes[destination].ConnectedNodes.Add(allNodes[origin]);
                    }
                }
            }

            return allNodes;
        }

        public static UndirectedGraph ReadInput()
        {
            int numberOfNodes;
            int numberOfEdges;

            UndirectedGraph graph = new UndirectedGraph();

            // Read a line while possible
            string line;
            int counter = 0;
            int maxCounter = 1;     // Starts at one because the number of edges is unknown; this is known before the first edge is read, so before counter is incremented
            while (counter < maxCounter)
            {
                line = Console.ReadLine();

                // Split the line
                string[] splittedLine = line.Split();

                // If the line is a comment, continue
                if (splittedLine[0] == "c")
                {
                    continue;
                }

                // If the line contains the information about the nodes and edges, save these and create an empty graph
                if (splittedLine[0] == "p")
                {
                    numberOfNodes = int.Parse(splittedLine[2]);
                    numberOfEdges = int.Parse(splittedLine[3]);

                    maxCounter = numberOfEdges;

                    graph = new UndirectedGraph(numberOfNodes);
                }
                // Else the line contains information about an edge, so add this edge to the graph
                else
                {
                    counter++;

                    int origin = int.Parse(splittedLine[0]);
                    int destination = int.Parse(splittedLine[1]);

                    graph.AddEdge(origin, destination);
                }
            }

            return graph;
        }

        public static Node[] ReadInputAsNodes()
        {
            int numberOfNodes;
            int numberOfEdges;
            Node[] allNodes = new Node[0];

            // Read a line while possible
            string line;
            int counter = 0;
            int maxCounter = 1;     // Starts at one because the number of edges is unknown; this is known before the first edge is read, so before counter is incremented
            while (counter < maxCounter)
            {
                line = Console.ReadLine();

                // Split the line
                string[] splittedLine = line.Split();

                // If the line is a comment, continue
                if (splittedLine[0] == "c")
                {
                    continue;
                }

                // If the line contains the information about the nodes and edges, save these and create an empty graph
                if (splittedLine[0] == "p")
                {
                    numberOfNodes = int.Parse(splittedLine[2]);
                    numberOfEdges = int.Parse(splittedLine[3]);

                    maxCounter = numberOfEdges;

                    allNodes = new Node[numberOfNodes];
                    for (int i = 0; i < numberOfNodes; i++)
                    {
                        allNodes[i] = new Node(i + 1)
                        {
                            ConnectedNodes = new List<Node>()
                        };
                    }
                }
                // Else the line contains information about an edge, so add this edge to the graph
                else
                {
                    counter++;

                    int origin = int.Parse(splittedLine[0]) - 1;
                    int destination = int.Parse(splittedLine[1]) - 1;

                    allNodes[origin].ConnectedNodes.Add(allNodes[destination]);
                    allNodes[destination].ConnectedNodes.Add(allNodes[origin]);
                }
            }

            return allNodes;
        }

        public static string WriteOutput(State finalState)
        {
            return finalState.Tree.ToString();
        }

        public static string PrintTree(RecursiveTree<Node> tree)
        {
            int[] nodeArray = new int[tree.NumberOfNodes];

            nodeArray[tree.Value.Number - 1] = 0;

            foreach (RecursiveTree<Node> subTree in tree.Children)
            {
                PrintTree(subTree, nodeArray);
            }


            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(tree.Depth);
            stringBuilder.Append("\n");

            for (int i = 0; i < nodeArray.Length; i++)
            {
                stringBuilder.Append(nodeArray[i]);
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        public static void PrintTree(RecursiveTree<Node> tree, int[] nodeArray)
        {
            nodeArray[tree.Value.Number - 1] = tree.Parent.Value.Number;

            foreach (RecursiveTree<Node> subTree in tree.Children)
            {
                PrintTree(subTree, nodeArray);
            }
        }
    }
}
