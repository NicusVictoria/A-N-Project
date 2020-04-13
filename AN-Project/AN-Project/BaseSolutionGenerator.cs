using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Class that creates a base solution
    /// </summary>
    class BaseSolutionGenerator
    {
        /// <summary>
        /// Creates a base solution
        /// </summary>
        /// <returns>A base solution for the search algorithms to use</returns>
        public static OldState EmptyState()
        {
            int numberOfNodes = Program.inputGraph.Nodes.Count;

            OldState firstState = new OldState()
            {
                Tree = new Tree()
            };

            List<TreeNode> nodes = new List<TreeNode>();

            TreeNode root = new TreeNode()
            {
                Index = 0,
                depth = 1
            };

            firstState.Tree.Root = root;
            nodes.Add(root);

            TreeNode parent = root;
            parent.EmptyChildrenList();

            for (int i = 1; i < numberOfNodes; i++)
            {
                TreeNode newNode = new TreeNode()
                {
                    Index = i,
                    depth = i + 1,
                    Parent = parent,
                };
                parent.AddChild(newNode);
                nodes.Add(newNode);

                parent = newNode;
            }

            firstState.Tree.Depth = numberOfNodes;
            firstState.Tree.Nodes = nodes;

            firstState.Tree.UpdateLowestNodes();

            firstState.Tree.ScoreKeeper.CalculateTreeScore(firstState);

            return firstState;
        }

        public static RecursiveTree<Node> EmptyRecursiveTree()
        {
            Program.allRecTreeNodes = new List<RecursiveTree<Node>>();
            int numberOfNodes = Program.allNodes.Count;
            RecursiveTree<Node> recTree = new RecursiveTree<Node>(Program.allNodes[0]);
            Program.allRecTreeNodes.Add(recTree);
            RecursiveTree<Node> child = recTree;
            for (int i = 1; i < numberOfNodes; i++)
            {
                Node newNode = Program.allNodes[i];
                RecursiveTree<Node> parent = new RecursiveTree<Node>(newNode);
                child.Parent = parent;
                parent.AddChild(child);
                child = parent;
                Program.allRecTreeNodes.Add(parent);
            }
            return recTree;
        }
    }
}
