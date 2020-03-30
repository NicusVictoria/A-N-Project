using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Class containing the current state of the program
    /// </summary>
    class State
    {

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
        private List<Node> nodes;
        
        /// <summary>
        /// Scorekeeper
        /// </summary>
        private readonly ScoreKeeper scoreKeeper = new ScoreKeeper();
        
        /// <summary>
        /// Root of the tree
        /// </summary>
        public Node Root { get; private set; }
        
        /// <summary>
        /// The depth of the tree
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// The score of this solution / tree
        /// </summary>
        public double Score { get { return scoreKeeper.CurrentScore; } }

        /// <summary>
        /// List of nodes that are on the lowest level; not equal to the leaves.
        /// </summary>
        public List<Node> LowestNodes { get; private set; }

        /// <summary>
        /// Updates the list of lowest nodes
        /// </summary>
        private void UpdateLowestNodes()
        {
            LowestNodes.Clear();
            
            foreach (Node n in nodes)
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
        public void SwapWithParent(Node node)
        {
            // Save the old parent for later use
            Node parent = node.Parent;

            // Update the depth for the parent and all children of the parent
            Depth = Math.Max(Depth, parent.RecursivelyAdjustDepth(1));

            // The node that is going to be swapped upwards has just been moved down by the RecursivelyAdjustDepth-call.
            // To compensate for this, and because it is going up one level in the tree because of the swap, decrease the depth by 2.
            node.depth -= 2;

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
            parent.Parent.Children.Remove(parent);
            parent.Parent.Children.Add(node);
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
            // TODO: implement
            throw new NotImplementedException();
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
        /// Recursively adjusts the depth of this node and all its children
        /// </summary>
        /// <param name="change">The amount the depth should be changed by</param>
        /// <returns>The new maximum depth encountered in this subtree</returns>
        public int RecursivelyAdjustDepth(int change)
        {
            depth += change;
            int maxDepth = depth;
            
            // Recursive call to all children, also updates the maxDepth along the way
            foreach(Node n in Children)
            {
                maxDepth = Math.Max(maxDepth, n.RecursivelyAdjustDepth(change));
            }

            return maxDepth;
        }
    }
}
