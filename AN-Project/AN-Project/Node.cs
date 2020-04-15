using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AN_Project
{
    public interface INode<N> where N : INode<N>
    {
        /// <summary>
        /// All nodes that are connected to this node
        /// </summary>
        List<N> ConnectedNodes { get; }
    }

    /// <summary>
    /// An interface for a basic tree-node. 
    /// </summary>
    public interface ITreeNode<T> where T : ITreeNode<T>
    {
        /// <summary>
        /// All nodes that are children of this node in the tree
        /// </summary>
        ReadOnlyCollection<T> Children { get; }

        /// <summary>
        /// The parent of this node in the tree
        /// </summary>
        T Parent { get; }
    }
    
    /// <summary>
    /// An implementation of the INode interface
    /// </summary>
    public class Node : INode<Node>, IComparable<Node>
    {
        /// <summary>
        /// The number of this node
        /// </summary>
        public int Number { get; private set; }

        public List<Node> ConnectedNodes { get; set; }

        /// <summary>
        /// The number of nodes this node is connected to
        /// </summary>
        public int Degree { get { return ConnectedNodes.Count; } }

        /// <summary>
        /// A measure for how good it is to choose this node for certain operations
        /// </summary>
        public double Heuristic { get { return Degree; } } //TODO improve this

        /// <summary>
        /// Constructor for a node
        /// </summary>
        /// <param name="number">The number of this node</param>
        public Node(int number)
        {
            Number = number;
        }

        /// <summary>
        /// Calculates the degree o this node while only considering nodes that are in the subgraph. Ignores connections to all other nodes
        /// </summary>
        /// <param name="subGraph">The subgraph to calculate the remaining degree in</param>
        /// <returns></returns>
        public int RemainingDegree(HashSet<Node> subGraph) => ConnectedNodes.Count(n => subGraph.Contains(n));

        int IComparable<Node>.CompareTo(Node other)
        {
            return Heuristic.CompareTo(other.Heuristic);
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}
