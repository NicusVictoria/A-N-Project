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
        public static State Empty()
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
            parent.ChildrenList = new List<TreeNode>();

            for (int i = 1; i < numberOfNodes; i++)
            {
                TreeNode newNode = new TreeNode()
                {
                    Index = i,
                    depth = i + 1,
                    Parent = parent,
                    ChildrenList = new List<TreeNode>()
                };
                parent.ChildrenList.Add(newNode);
                nodes.Add(newNode);

                parent = newNode;
            }

            firstState.Tree.Depth = numberOfNodes;
            firstState.Tree.Nodes = nodes;

            firstState.Tree.UpdateLowestNodes();

            firstState.Tree.ScoreKeeper.CalculateTreeScore(firstState);

            return firstState;
        }
    }
}
