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
        private List<Node> Nodes;
        
        private readonly ScoreKeeper scoreKeeper = new ScoreKeeper();
        public Node Root { get; private set; }
        public int Depth { get; private set; }

        public double Score { get { return scoreKeeper.CurrentScore; } }

        public List<Node> LowestNodes { get; private set; }

        private void UpdateLowestNodes()
        {
            LowestNodes.Clear();
            foreach(Node n in Nodes)
            {
                if(n.depth == Depth)
                {
                    LowestNodes.Add(n);
                }
            }
        }

        public void SwapWithParent(Node node)
        {


            Node parent = node.Parent;

            Depth = Math.Max(Depth, parent.RecursivelyAdjustDepth(1));
            node.depth -= 2;

            parent.Children.AddRange(node.Children);
            foreach (Node n in node.Children)
            {
                n.Parent = parent;
            }

            if (parent == Root) Root = node;

            node.Children = new List<Node>{ parent } ;
            node.Parent = node.Parent.Parent;
            parent.Parent.Children.Remove(parent);
            parent.Parent.Children.Add(node);
            parent.Parent = node;

            UpdateLowestNodes();

        }




        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }

    public class Node  : ITreeNode<Node>
    {
        public int RecursivelyAdjustDepth(int change)
        {
            depth+= change;
            int maxDepth = depth;
            foreach(Node n in Children)
            {
                maxDepth = Math.Max(maxDepth, n.RecursivelyAdjustDepth(change));
            }
            return maxDepth;

        }

        public List<Node> Children { get; set; }
        public Node Parent { get; set; }

        public int depth;
    }

}
