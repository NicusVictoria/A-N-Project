using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Interface for a tree
    /// </summary>
    /// <typeparam name="N">The type of nodes in the tree</typeparam>
    public interface ITree<N> where N : ITreeNode<N>
    {
        /// <summary>
        /// The root of the tree
        /// </summary>
        N Root { get; }

        /// <summary>
        /// The depth of the tree; the maximum depth a node in the tree has
        /// </summary>
        int Depth { get; }
    }

    /// <summary>
    /// Implementation of a tree for TreeDepth algorithm; implements the ITree interface
    /// </summary>
    public class Tree : ITree<TreeNode>
    {
        /// <summary>
        /// List of nodes in the tree
        /// </summary>
        public List<TreeNode> Nodes { get; set; }

        /// <summary>
        /// Scorekeeper
        /// </summary>
        public ScoreKeeper ScoreKeeper { get; private set; }

        /// <summary>
        /// Root of the tree
        /// </summary>
        public TreeNode Root { get; set; }

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
        public List<TreeNode> LowestNodes { get; private set; }

        public Tree()
        {
            ScoreKeeper = new ScoreKeeper();
        }

        /// <summary>
        /// Updates the list of lowest nodes
        /// </summary>
        public void UpdateLowestNodes()
        {
            LowestNodes = new List<TreeNode>();

            foreach (TreeNode n in Nodes)
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
        public void SwapWithParent(State state, TreeNode node)
        {
            if (Root == node) throw new Exception("Cannot swap the root of the tree!");

            // Save the old parent for later use
            TreeNode parent = node.Parent;

            parent.RemoveChild(node);

            // Update the depth for the parent and all children of the parent
            Depth = Math.Max(Depth, parent.RecursivelyAdjustDepthAndScore(state, ScoreKeeper, 1));

            // The swapped node is going up one level in the tree because of the swap, so decrease the depth by 1.
            node.depth -= 1;

            // Add all children of "node" to the parent
            parent.AddChildren(node.Children);

            foreach (TreeNode n in node.Children)
            {
                n.Parent = parent;
            }


            // The only child of the new upper node is its old parent
            node.EmptyChildrenList();
            node.AddChild(parent);
            node.Parent = parent.Parent;

            // The swapped node is no child anymore of its old parent
            if (parent != Root)
            {
                parent.Parent.RemoveChild(parent);
                parent.Parent.AddChild(node);
            }
            parent.Parent = node;

            // If the old parent was the root, update the parent to the new node
            if (parent == Root)
            {
                Root = node;
            }

            // Update the list of the lowest nodes
            UpdateLowestNodes();
        }

        public void MoveNodeUp(State state, TreeNode node)
        {
            if (node == Root) throw new Exception("Cannot move the root of the tree up!");

            TreeNode parent = node.Parent;

            int newDepth = Program.random.Next(0, node.depth);
            int moveUpAmount = node.depth - newDepth;

            if (newDepth == 0)
            {
                foreach (TreeNode child in node.Children)
                {
                    child.Parent = parent;
                }

                parent.RemoveChild(node);
                parent.AddChildren(node.Children);

                Root.Parent = node;
                node.EmptyChildrenList();
                node.AddChild(Root);
                node.Parent = null;
                Root = node;
            }
            else
            {
                TreeNode newParent = node.Parent;
                TreeNode newChild = node;

                for (int i = 0; i < moveUpAmount; i++)
                {
                    newParent = newParent.Parent;
                    newChild = newChild.Parent;
                }

                foreach (TreeNode child in node.Children)
                {
                    child.Parent = parent;
                }
                parent.RemoveChild(node);

                newChild.RecursivelyAdjustDepthAndScore(state, ScoreKeeper, -1);

                parent.AddChildren(node.Children);

                newChild.Parent = node;
                node.EmptyChildrenList();
                node.AddChild(newChild);

                newParent.RemoveChild(newChild);
                newParent.AddChild(node);

                node.Parent = newParent;
            }

            node.depth = newDepth;
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
                TreeNode currentNode = Nodes[i];

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
}
