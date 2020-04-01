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

        public static Node[] ReadInputAsNodes(String path)
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
                            allNodes[i] = new Node(i + 1);
                            allNodes[i].ConnectedNodes = new List<Node>();
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
            while ((line = Console.ReadLine()) != null)
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
                    int origin = int.Parse(splittedLine[0]);
                    int destination = int.Parse(splittedLine[1]);

                    graph.AddEdge(origin, destination);
                }
            }

            return graph;
        }

        public static string WriteOutput(State finalState)
        {
            return finalState.Tree.ToString();
        }
    }
}
