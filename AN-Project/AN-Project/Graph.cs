using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Interface for a graph
    /// </summary>
    /// <typeparam name="N">Type of the nodes in the graph</typeparam>
    public interface IGraph<N>
    {
        /// <summary>
        /// Checks whether two nodes are connected. Depending on the implementation this could be edges or arcs.
        /// </summary>
        /// <param name="a">One of the nodes of the connection (origin if arc)</param>
        /// <param name="b">The other node of the connection (target if arc)</param>
        /// <returns>True if the nodes are connected, false otherwise</returns>
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
        /// <summary>
        /// The number of nodes in this graph
        /// </summary>
        protected int count;

        /// <summary>
        /// The adjacency matrix containing all edges in the graph
        /// </summary>
        protected List<List<bool>> adjacencyMatrix;

        /// <summary>
        /// Constructor when the number of nodes is not known
        /// </summary>
        public UndirectedGraph()
        {

        }

        /// <summary>
        /// Constructor for undirected graph when the number of nodes is already known
        /// </summary>
        /// <param name="count">The number of nodes in the graph</param>
        public UndirectedGraph(int count)
        {
            this.count = count;

            adjacencyMatrix = new List<List<bool>>(new List<bool>[count]);

            for (int i = 0; i < count; i++)
            {
                adjacencyMatrix[i] = new List<bool>(new bool[count]);
            }
        }

        /// <summary>
        /// Adds a new node. O(n) where n is the amount of nodes already in the graph.
        /// </summary>
        public void AddNode()
        {
            count++;

            foreach (List<bool> l in adjacencyMatrix)
            {
                l.Add(false);
            }

            adjacencyMatrix.Add(new List<bool>(count));
        }

        /// <summary>
        /// Adds an edge (both directions) to the graph. O(1).
        /// </summary>
        /// <param name="index1">The index of the start node</param>
        /// <param name="index2">The index of the destination node</param>
        public void AddEdge(int index1, int index2)
        {
            if (index1 < index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }

            adjacencyMatrix[index1][index2] = true;
        }

        /// <summary>
        /// Checks if there is an edge between the specified nodes. Order of nodes does not matter. O(1).
        /// </summary>
        /// <param name="index1">The index of the first node</param>
        /// <param name="index2">The index of the second node</param>
        /// <returns>True if the nodes are connected, false otherwise</returns>
        public bool Connected(int index1, int index2)
        {
            if (index1 > count) throw new IndexOutOfRangeException($"The index ({index1}) of node 1 is higher than the amount of nodes in the graph ({count})!");
            if (index2 > count) throw new IndexOutOfRangeException($"The index ({index2}) of node 2 is higher than the amount of nodes in the graph ({count})!");
            
            if (index1 < index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }

            return adjacencyMatrix[index1][index2];
        }

        /// <summary>
        /// Returns all nodes that are currently in the graph. O(n) where n is the amounnt of nodes currently in the graph.
        /// </summary>
        public List<int> Nodes
        {
            get
            {
                return Enumerable.Range(0, count).ToList();
            }
        }

        /// <summary>
        /// Checks if the graph is undirected. always returns true as it is not possible to construct a directed graph using this class. O(1)
        /// </summary>
        public bool Undirected => true;
    }
}
