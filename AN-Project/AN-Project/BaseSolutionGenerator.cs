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
        public static State EmptyState()
        {
            int numberOfNodes = Program.inputGraph.Nodes.Count;

            State firstState = new State()
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
            int numberOfNodes = Program.allNodes.Count;

            List<Node> nodes = new List<Node>();

            Node root = new Node(0);

            RecursiveTree<Node> recTree = new RecursiveTree<Node>(root);

            nodes.Add(root);

            RecursiveTree<Node> parent = recTree;
            for (int i = 1; i < numberOfNodes; i++)
            {
                Node newNode = new Node(i);
                RecursiveTree<Node> child = new RecursiveTree<Node>(newNode);
                child.Parent = parent;
                parent.AddChild(child);
                nodes.Add(newNode);

                parent = child;
            }

            recTree.RecursivelyUpdateDepth();

            return recTree;
        }
    }
}
