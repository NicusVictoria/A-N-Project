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
        private const double CENTER_RESEMBLANCE_MULTIPLIER = 0.5;
        private const double ARTICULATION_POINT_MULTIPLIER = 3;

        private int centerResemblance;
        private int articulationPointValue;

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
        /// Measure for how much this node resembles a center of the graph. Higher is better
        /// </summary>
        public int CenterResemblance { get { return centerResemblance; } set { centerResemblance = (int)(value * CENTER_RESEMBLANCE_MULTIPLIER); } }

        /// <summary>
        /// Whether this point is an articulation point, and its value. 0 is no articulation point, >0 is an articulation point.
        /// </summary>
        public int ArticulationPointValue { get { return articulationPointValue; } set {articulationPointValue = (int)(value * ARTICULATION_POINT_MULTIPLIER); } }

        /// <summary>
        /// A measure for how good it is to choose this node for certain operations
        /// </summary>
        public double Heuristic { get { return Degree + CenterResemblance + ArticulationPointValue; } } //TODO improve this

        /// <summary>
        /// Constructor for a node
        /// </summary>
        /// <param name="number">The number of this node</param>
        /// <param name="centerResemblance">How much this node resembles the center of a graph. Higher value means it resembles the center more</param>
        /// <param name="articulationPointValue">Whether this point is an articulation point. Use 0 for no articulation point, 1 for articulation point</param>
        public Node(int number, int centerResemblance = 0, int articulationPointValue = 0)
        {
            Number = number;
            CenterResemblance = centerResemblance;
            ArticulationPointValue = articulationPointValue;
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
