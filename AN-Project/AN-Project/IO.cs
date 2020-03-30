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
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(finalState.Tree.Depth);
            stringBuilder.Append("\n");

            for (int i = 0; i < finalState.Tree.Nodes.Count; i++)
            {
                Node currentNode = finalState.Tree.Nodes[i];

                if (currentNode == finalState.Tree.Root)
                {
                    stringBuilder.Append("0");
                }
                else
                {
                    stringBuilder.Append(currentNode.Parent.Index + 1);
                }

                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }
    }
}
