using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Class containing the current state of the program
    /// </summary>
    public class State
    {
        public Tree Tree { get; set; }

        public State Clone()
        {
            return (State)MemberwiseClone();
        }
    }

    /// <summary>
    /// An interface for a basic tree-node. 
    /// </summary>
    public interface ITreeNode<T> where T : ITreeNode<T>
    {
        /// <summary>
        /// All nodes that are children of this node in the tree
        /// </summary>
        List<T> Children { get; }

        /// <summary>
        /// The parent of this node in the tree
        /// </summary>
        T Parent { get; }
    }

    /// <summary>
    /// Interface for a tree
    /// </summary>
    /// <typeparam name="N">The type of nodes in the tree</typeparam>
    public interface ITree<N> where N : ITreeNode<N>
    {
        /// <summary>
        /// The root of the tree
        /// </summary>
        public N Root { get; }

        /// <summary>
        /// The depth of the tree; the maximum depth a node in the tree has
        /// </summary>
        public int Depth { get; }
    }

    /// <summary>
    /// Implementation of a tree for TreeDepth algorithm; implements the ITree interface
    /// </summary>
    public class Tree : ITree<Node>
    {
        /// <summary>
        /// List of nodes in the tree
        /// </summary>
        public List<Node> Nodes { get; set; }
        
        /// <summary>
        /// Scorekeeper
        /// </summary>
        public ScoreKeeper ScoreKeeper { get; private set; }
        
        /// <summary>
        /// Root of the tree
        /// </summary>
        public Node Root { get; set; }
        
        /// <summary>
        /// The depth of the tree
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// The score of this solution / tree
        /// </summary>
        public double Score { get { return ScoreKeeper.CurrentScore; } }

        /// <summary>
        /// List of nodes that are on the lowest level; not equal to the leaves.
        /// </summary>
        public List<Node> LowestNodes { get; private set; }

        public Tree()
        {
            ScoreKeeper = new ScoreKeeper();
        }

        /// <summary>
        /// Updates the list of lowest nodes
        /// </summary>
        public void UpdateLowestNodes()
        {
            LowestNodes = new List<Node>();
            
            foreach (Node n in Nodes)
            {
                if (n.depth == Depth)
                {
                    LowestNodes.Add(n);
                }
            }
        }

        /// <summary>
        /// Swaps a node with its parents and moces all children of these nodes to the new lower node
        /// </summary>
        /// <param name="node">The node to be swapped with its parent</param>
        public void SwapWithParent(State state, Node node)
        {
            if (Root == node) throw new Exception("Cannot swap the root of the tree!");

            // Save the old parent for later use
            Node parent = node.Parent;

            parent.Children.Remove(node);

            // Update the depth for the parent and all children of the parent
            Depth = Math.Max(Depth, parent.RecursivelyAdjustDepthAndScore(state, ScoreKeeper, 1));

            // The node that is going to be swapped upwards has just been moved down by the RecursivelyAdjustDepth-call.
            // To compensate for this, and because it is going up one level in the tree because of the swap, decrease the depth by 2.
            node.depth -= 1;

            // Add all children of "node" to the parent
            parent.Children.AddRange(node.Children);
            
            foreach (Node n in node.Children)
            {
                n.Parent = parent;
            }

            // If the old parent was the root, update the parent to the new node
            if (parent == Root)
            {
                Root = node;
            }

            // The only child of the new upper node is its old parent
            node.Children = new List<Node>{ parent };
            node.Parent = node.Parent.Parent;

            // The swapped node is no child anymore of its old parent
            if (parent.Parent != null)
            {
                parent.Parent.Children.Remove(parent);
                parent.Parent.Children.Add(node);
            }
            parent.Parent = node;

            // Update the list of the lowest nodes
            UpdateLowestNodes();
        }

        /// <summary>
        /// Prints the tree in the output representation as requested by the challenge
        /// </summary>
        /// <returns>A string in the output representation as requested by the challenge</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(Depth);
            stringBuilder.Append("\n");

            for (int i = 0; i < Nodes.Count; i++)
            {
                Node currentNode = Nodes[i];

                if (currentNode == Root)
                {
                    stringBuilder.Append("0");
                }
                else
                {
                    stringBuilder.Append(currentNode.Parent.Index + 1);
                }

                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Calculates the hascode corresponding to this tree
        /// </summary>
        /// <returns>The hashcode of the tree</returns>
        public override int GetHashCode()
        {
            // TODO: (maybe) implement
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Implementation of a node for TreeDepth algorithm; implements the ITreeNode interface
    /// </summary>
    public class Node : ITreeNode<Node>
    {
        /// <summary>
        /// The depth of this node
        /// </summary>
        public int depth;

        /// <summary>
        /// The list of children of this node
        /// </summary>
        public List<Node> Children { get; set; }

        /// <summary>
        /// The parent of this node
        /// </summary>
        public Node Parent { get; set; }

        /// <summary>
        /// The index of this node in the list of nodes in the tree
        /// </summary>
        public int Index { get; set; }
        
        public double DegreeScore { get; set; }
        
        public double DistanceScore { get; set; }

        /// <summary>
        /// Recursively adjusts the depth of this node and all its children
        /// </summary>
        /// <param name="change">The amount the depth should be changed by</param>
        /// <returns>The new maximum depth encountered in this subtree</returns>
        public int RecursivelyAdjustDepthAndScore(State state, ScoreKeeper scoreKeeper, int change)
        {
            depth += change;
            int maxDepth = depth;

            //scoreKeeper.CalculateNodeScore(state, this);
            
            // Recursive call to all children, also updates the maxDepth along the way
            foreach(Node n in Children)
            {
                maxDepth = Math.Max(maxDepth, n.RecursivelyAdjustDepthAndScore(state, scoreKeeper, change));
            }

            return maxDepth;
        }
    }
}
