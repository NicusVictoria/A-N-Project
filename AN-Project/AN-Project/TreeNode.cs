using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AN_Project
{
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
    /// Implementation of a node for TreeDepth algorithm; implements the ITreeNode interface
    /// </summary>
    public class TreeNode : ITreeNode<TreeNode>
    {
        /// <summary>
        /// The depth of this node
        /// </summary>
        public int depth;

        /// <summary>
        /// The list of children of this node
        /// </summary>
        public List<TreeNode> ChildrenList { get; set; }

        /// <summary>
        /// The parent of this node
        /// </summary>
        public TreeNode Parent { get; set; }

        /// <summary>
        /// The index of this node in the list of nodes in the tree
        /// </summary>
        public int Index { get; set; }

        public double DegreeScore { get; set; }

        public double DistanceScore { get; set; }

        public ReadOnlyCollection<TreeNode> Children => new ReadOnlyCollection<TreeNode>(ChildrenList);

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
            foreach (TreeNode n in Children)
            {
                maxDepth = Math.Max(maxDepth, n.RecursivelyAdjustDepthAndScore(state, scoreKeeper, change));
            }

            return maxDepth;
        }
    }

}
