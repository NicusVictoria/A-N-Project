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

    public class Node : INode<Node>, IComparable<Node>
    {
        public Node(int number)
        {
            this.Number = number;
        }

        public int Number { get; private set; }

        public List<Node> ConnectedNodes { get; set; }

        public int Degree { get { return ConnectedNodes.Count; } }

        public int RemainingDegree(HashSet<Node> subGraph) => ConnectedNodes.Select(n => !subGraph.Contains(n)).Count(); // TODO: should this not be select those that are in the subgraph?

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
