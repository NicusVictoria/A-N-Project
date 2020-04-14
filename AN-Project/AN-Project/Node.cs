using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections;

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


    public class Node : INode<Node>, IComparable<Node>
    {
        public Node(int number)
        {
            this.Number = number;
        }

        public int Number { get; private set; }

        public List<Node> ConnectedNodes { get; set; }

        public int Degree { get { return ConnectedNodes.Count; } }

        public int RemainingDegree(HashSet<Node> subGraph) => ConnectedNodes.Count(n => subGraph.Contains(n));

        public double Heuristic { get { return Degree; } } //TODO improve this

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
