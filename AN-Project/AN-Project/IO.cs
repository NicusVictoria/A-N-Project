using System;
using System.Collections.Generic;
using System.IO;

namespace AN_Project
{
    class IO
    {
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
    }
}
