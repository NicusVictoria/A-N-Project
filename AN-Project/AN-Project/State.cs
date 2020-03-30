using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    class State
    {

    }

    /// <summary>
    /// An interface for a basic tree-node. 
    /// </summary>

    public interface ITreeNode<T> where T : ITreeNode<T>
    {
        /// <summary>
        /// All nodes that are connected to this node
        /// </summary>
        List<T> Children { get; }
        T Parent { get; }
    }

    public interface ITree<N> where N : ITreeNode<N>
    {
        public N Root { get; }

        public int Depth { get; }
    }

    public class Tree : ITree<Node>
    {
        private readonly ScoreKeeper scoreKeeper = new ScoreKeeper();
        public Node Root { get; private set; }
        public int Depth { get; private set; }

        public double Score { get { return scoreKeeper.CurrentScore; } }

        public List<Node> LowestNodes { get; private set; }

        public void SwapWithParent(Node node)
        {
            Node parent = node.Parent;
            parent.Children.AddRange(node.Children);
            foreach (Node n in node.Children)
            {
                n.Parent = parent;
                n.depth;
            }

            
            node.Children = new List<Node>{ parent } ;
            node.Parent = node.Parent.Parent;
            parent.Parent.Children.Remove(parent);
            parent.Parent.Children.Add(node);
            parent.Parent = node;

            parent.RecursivelyAdjustDepth();
        }




        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }

    public class Node  : ITreeNode<Node>
    {
        public void RecursivelyAdjustDepth()
        {
            depth++;
            foreach(Node n in Children)
            {
                n.RecursivelyAdjustDepth();
            }
        }
        public List<Node> Children { get; set; }
        public Node Parent { get; set; }

        public int depth;
    }

}
