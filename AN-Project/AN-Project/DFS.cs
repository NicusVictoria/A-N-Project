using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
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
            throw new Exception("there were no nodes that matched the criteria");
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
            stack.Push(startNode);
            while (stack.Count != 0)
            {
                N node = stack.Pop();
                if (beenList.Contains(node)) continue;
                beenList.Add(node);
                if (targetSelection(node)) retList.Add(node);
                foreach (N n in node.ConnectedNodes)
                {
                    //if (!beenList.Contains(n))
                    //{
                    //    beenList.Add(n);
                    //}
                    stack.Push(n);
                }
            }
            return retList;
        }

        /*
        public static List<Node> ArticulationPoints(int numberOfNodes, Node node, HashSet<Node> beenList = null)
        {
            List<Node> retList = new List<Node>();
            int time = 0;
            int[] discoveryTime = new int[numberOfNodes];
            int[] low = new int[numberOfNodes];

            return RecArticulationPoints(discoveryTime, low, time, node, null, beenList);
        }

        private static List<Node> RecArticulationPoints(int[] discoveryTime, int[] low, int time, Node node, Node parent, HashSet<Node> beenList = null)
        {
            beenList.Add(node);
            discoveryTime[node.Number] = time + 1;
            low[node.Number] = time + 1;

            int childNumber = 0;
            foreach (Node neighbour in node.ConnectedNodes)
            {
                if (neighbour == parent) continue;

                if (!beenList.Contains(neighbour))
                {
                    childNumber++;
                    time++;

                    List<Node> resultList = RecArticulationPoints(discoveryTime, low, time, neighbour, node, beenList);

                    low[neighbour.Number] = Math.Min(low[neighbour.Number], low[node.Number]);
                    if (parent == null && childNumber > 1)
                    {
                        resultList.Add(neighbour);
                    }
                    else if (parent != null && low[node.Number] >= discoveryTime[neighbour.Number])
                    {
                        resultList.Add(neighbour);
                    }
                }
                else
                {
                    low[neighbour.Number] = Math.Min(low[neighbour.Number], discoveryTime[node.Number]);
                }
            }
        }
        */
    }
}
