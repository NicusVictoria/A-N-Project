using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AN_Project
{
    class Graph
    {

    }

    public interface IGraph<N>
    {
        /// <summary>
        /// Checks whether two nodes are connected. Depending on the implementation this could be edges or arcs.
        /// </summary>
        /// <param name="a">One of the nodes of the connection (origin if arc)</param>
        /// <param name="b">The other node of the connection (target if arc)</param>
        /// <returns></returns>
        bool Connected(N a, N b);

        /// <summary>
        /// Returns all nodes in the graph.
        /// </summary>
        List<N> Nodes { get; }

        /// <summary>
        /// Whether or not the graph is an undirected graph. 
        /// </summary>
        bool Undirected { get; }
    }

    /// <summary>
    /// Undirected graph without node contents, implemented using adjacency matrix. O(n^2) space where n is the amount of nodes.
    /// </summary>
    public class UndirectedGraph : IGraph<int>
    {
        protected int count;
        protected List<List<bool>> adjacency;

        /// <summary>
        /// Adds a new node. O(n) where n is the amount of nodes already in the graph.
        /// </summary>
        public void AddNode()
        {
            count++;
            foreach (List<bool> l in adjacency)
            {
                l.Add(false);
            }
            adjacency.Add(new List<bool>());
        }

        /// <summary>
        /// Adds an edge (both directions) to the graph. O(1).
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        public void AddEdge(int index1, int index2)
        {
            if (index1 < index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }
            adjacency[index1][index2] = true;
        }

        /// <summary>
        /// Checks if there is an edge between the specified nodes. Order of nodes does not matter. O(1).
        /// </summary>
        /// <param name="index1">the index of the first node</param>
        /// <param name="index2">the index of the second node</param>
        /// <returns></returns>
        public bool Connected(int index1, int index2)
        {
            if (index1 > count || index2 > count) throw new Exception("this index is higher than the amount of nodes in the graph");
            if (index1 < index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }
            return adjacency[index1][index2];
        }

        /// <summary>
        /// Returns all nodes that are currently in the graph. O(n) where n is the amounnt of nodes currently in the graph.
        /// </summary>
        public List<int> Nodes {
            get {
                return Enumerable.Range(0, count).ToList();
            }
        }

        /// <summary>
        /// Checks if the graph is undirected. always returns true as it is not possible to construct a directed graph using this class. O(1)
        /// </summary>
        public bool Undirected => true;
    }
}
