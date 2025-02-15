﻿using System;
using System.Collections.Generic;

namespace AN_Project
{
    /// <summary>
    /// Implementation of Depth First Search
    /// </summary>
    public static class DFS
    {
        /// <summary>
        /// Look for nodes that fulfill the criteria, stopping when you have found one. O(n).
        /// </summary>
        /// <param name="startNode">the origin from which will be searched</param>
        /// <param name="targetSelection">the function that will determine whether a node gets returned; 
        /// will be returned when the function returns true, not returned when the function returns false</param>
        public static N First<N>(N startNode, Func<N, bool> targetSelection) where N : INode<N>
        {
            Stack<N> stack = new Stack<N>();
            HashSet<N> beenList = new HashSet<N>();
            stack.Push(startNode);
            while (stack.Count != 0)
            {
                N node = stack.Pop();
                beenList.Add(node);
                if (targetSelection(node)) return node;
                foreach (N n in node.ConnectedNodes)
                {
                    stack.Push(n);
                }
            }
            throw new Exception("There were no nodes that matched the criteria");
        }

        /// <summary>
        /// Look for nodes that fulfill the criteria, stopping when the entire graph has been explored. O(n) with n being the amount of nodes reachable from the startNode.
        /// </summary>
        /// <param name="startNode">the origin from which will be searched</param>
        /// <param name="targetSelection">the function that will determine whether a node gets returned; 
        /// will be returned when the function returns true, not returned when the function returns false</param>
        public static List<N> All<N>(N startNode, Func<N, bool> targetSelection, HashSet<N> beenList = null) where N : INode<N>
        {
            List<N> retList = new List<N>();
            Stack<N> stack = new Stack<N>();
            if (beenList == null) beenList = new HashSet<N>();
            beenList.Add(startNode);
            stack.Push(startNode);
            while (stack.Count != 0)
            {
                N node = stack.Pop();
                if (targetSelection(node)) retList.Add(node);
                foreach (N n in node.ConnectedNodes)
                {
                    if (!beenList.Contains(n))
                    {
                        stack.Push(n);
                        beenList.Add(n);
                    }
                }
            }
            return retList;
        }

        /// <summary>
        /// Find all articulation points in a graph
        /// </summary>
        /// <param name="numberOfNodes">The total number of nodes in the graph</param>
        /// <param name="node">An arbitrary node in the graph</param>
        /// <param name="beenList">A list with nodes that already have been visited</param>
        /// <returns>A list with articulation points</returns>
        public static List<Node> ArticulationPoints(int numberOfNodes, Node node, HashSet<Node> beenList = null)
        {
            // Algorithm is based on the code on the following website: https://www.geeksforgeeks.org/articulation-points-or-cut-vertices-in-a-graph/

            // Initialise values
            if (beenList == null) beenList = new HashSet<Node>(); 
            List<Node> articulationPoints = new List<Node>();
            int time = 0;
            int[] discoveryTime = new int[numberOfNodes];
            int[] low = new int[numberOfNodes];
            Node[] parents = new Node[numberOfNodes];

            // Find all articulation points recursively
            RecArticulationPoints(discoveryTime, low, time, node, parents, beenList, articulationPoints);

            return articulationPoints;
        }

        /// <summary>
        /// Recursive function for finding the Articulation points
        /// </summary>
        /// <param name="discoveryTime">Array containing the discovery times for each node</param>
        /// <param name="low">The array containing the discovery times of the lowest value in its subtree</param>
        /// <param name="time">The time</param>
        /// <param name="node">The node to start looking from</param>
        /// <param name="parents">An array with the nodes from which a node was discovered</param>
        /// <param name="beenList">A set of nodes that were already visited</param>
        /// <param name="articulationPoints">Teh list with all articulation points</param>
        private static void RecArticulationPoints(int[] discoveryTime, int[] low, int time, Node node, Node[] parents, HashSet<Node> beenList, List<Node> articulationPoints)
        {
            beenList.Add(node); 
            time++;
            discoveryTime[node.Number - 1] = time;
            low[node.Number - 1] = time;
            int childNumber = 0;

            foreach (Node neighbour in node.ConnectedNodes)
            {
                // If we have not already seen this neighbour, it is now a child of node and we will recurse for this neighbour
                if (!beenList.Contains(neighbour))
                {
                    childNumber++;
                    parents[neighbour.Number - 1] = node;
                    RecArticulationPoints(discoveryTime, low, time, neighbour, parents, beenList, articulationPoints);
                    low[node.Number - 1] = Math.Min(low[node.Number - 1], low[neighbour.Number - 1]);
                    
                    if (parents[node.Number - 1] == null && childNumber > 1)
                    {
                        articulationPoints.Add(node);
                    }
                    else if (parents[node.Number - 1] != null && low[neighbour.Number - 1] >= discoveryTime[node.Number - 1])
                    {
                        articulationPoints.Add(node);
                    }
                }
                else if (neighbour != parents[node.Number - 1])
                {
                    low[node.Number - 1] = Math.Min(low[node.Number - 1], discoveryTime[neighbour.Number - 1]);
                }
            }
        }
    }
}
