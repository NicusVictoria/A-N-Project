﻿using System;
using System.Collections.Generic;
using System.IO;

namespace AN_Project
{
    /// <summary>
    /// Class that handles reading the input
    /// </summary>
    class IO
    {
        /// <summary>
        /// Read an instance from a file
        /// </summary>
        /// <param name="path">The file to read the input from</param>
        /// <returns>An array with all nodes that have been made</returns>
        public static Node[] ReadInputAsNodes(string path, int centerResemblanceCap, int articulationPointMinimum)
        {
            int numberOfNodes = -1;
            int numberOfEdges = -1;
            int counter = -1; // UNUSED here, needed for the HandleLine method
            Node[] allNodes = new Node[0];

            // Using the streamreader to read the input file
            using (StreamReader streamReader = new StreamReader(path))
            {
                // Read a and handle line while possible
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    HandleLine(line, ref numberOfNodes, ref numberOfEdges, ref counter, ref allNodes);
                }
            }

            // Calculate some extra values to be used in the heuristics
            // PrecomputeHeuristicValues(allNodes, centerResemblanceCap, articulationPointMinimum); // These heuristic values only made our solutions worse, so there are not used and not computed any more
            return allNodes;
        }

        /// <summary>
        /// Read an instance from console input
        /// </summary>
        /// <returns>An array with all nodes that have been made</returns>
        public static Node[] ReadInputAsNodes(int centerResemblanceCap, int articulationPointMinimum)
        {
            int numberOfNodes = -1;
            int numberOfEdges = int.MaxValue;
            Node[] allNodes = new Node[0];

            // Read and handle a line while possible
            string line;
            int numberOfEdgesRead = 0;
            while (numberOfEdgesRead < numberOfEdges)
            {
                line = Console.ReadLine();
                HandleLine(line, ref numberOfNodes, ref numberOfEdges, ref numberOfEdgesRead, ref allNodes);
            }

            // Calculate some extra values to be used in the heuristics
            // PrecomputeHeuristicValues(allNodes, centerResemblanceCap, articulationPointMinimum); // These heuristic values only made our solutions worse, so there are not used and not computed any more
            return allNodes;
        }

        /// <summary>
        /// Handles a single input line
        /// </summary>
        /// <param name="line">The line to be handled</param>
        /// <param name="numberOfNodes">The number of nodes variable, can get a value because of this line</param>
        /// <param name="numberOfEdges">The number of edges variable, can get a value because of this line</param>
        /// <param name="numberOfEdgesRead">The number of edges read</param>
        /// <param name="allNodes">The result array with all nodes</param>
        private static void HandleLine(string line, ref int numberOfNodes, ref int numberOfEdges, ref int numberOfEdgesRead, ref Node[] allNodes)
        {
            // Split the line
            string[] splittedLine = line.Split();

            // If the line is a comment, skip this line
            if (splittedLine[0] == "c") return;

            // If the line contains the information about the nodes and edges, save these and instantiate the node array
            if (splittedLine[0] == "p")
            {
                numberOfNodes = int.Parse(splittedLine[2]);
                numberOfEdges = int.Parse(splittedLine[3]);

                allNodes = new Node[numberOfNodes];
                for (int i = 0; i < numberOfNodes; i++)
                {
                    // +1 because the nodes are 1-based in the problem instance
                    allNodes[i] = new Node(i + 1)
                    {
                        ConnectedNodes = new List<Node>()
                    };
                }
            }
            // Else the line contains information about an edge, so add this edge to the graph
            else
            {
                numberOfEdgesRead++;

                // -1 because the nodes are 1-based in the problem instance
                int origin = int.Parse(splittedLine[0]) - 1;
                int destination = int.Parse(splittedLine[1]) - 1;

                allNodes[origin].ConnectedNodes.Add(allNodes[destination]);
                allNodes[destination].ConnectedNodes.Add(allNodes[origin]);
            }
        }

        /// <summary>
        /// Computes all possible values used in more complex heuristics for nodes
        /// </summary>
        /// <param name="allNodes">All the nodes in the graoh</param>
        /// <param name="centerResemblanceCap">The maximum number of nodes for which the center resemblance is computed</param>
        private static void PrecomputeHeuristicValues(Node[] allNodes, int centerResemblanceCap, int articulationPointMinimum)
        {
            if (allNodes.Length <= centerResemblanceCap) CalculateCenterResemblances(allNodes);
            if (allNodes.Length >= articulationPointMinimum) CalculateArticulationPoints(allNodes);
        }

        /// <summary>
        /// Computes the center resemblance for the given nodes
        /// </summary>
        /// <param name="allNodes">The nodes for which to compute the center resemblance</param>
        private static void CalculateCenterResemblances(Node[] allNodes)
        {
            int[] centerResemblances = FloydWarshall.CenterResemblances(allNodes);
            for (int i = 0; i < allNodes.Length; i++)
            {
                allNodes[i].CenterResemblance = centerResemblances[i];
            }
        }

        /// <summary>
        /// Finds all the articulation points in the given nodes
        /// </summary>
        /// <param name="allNodes">The nodes to look for articulation points in</param>
        private static void CalculateArticulationPoints(Node[] allNodes)
        {
            List<Node> articulationPoints = DFS.ArticulationPoints(allNodes.Length, allNodes[0]);
            foreach (Node node in articulationPoints)
            {
                node.ArticulationPointValue = 1;
            }
        }
    }
}
