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
            // TODO: Implement
            int numberOfNodes = Program.inputGraph.Nodes.Count;

            State firstState = new State();

            firstState.Tree = new Tree();

            List<Node> nodes = new List<Node>();

            Node root = new Node();

            root.Index = 0;
            root.depth = 1;
            firstState.Tree.Root = root;
            nodes.Add(root);

            Node parent = root;
            for (int i = 1; i < numberOfNodes; i++)
            {
                Node newNode = new Node();
                newNode.Index = i;
                newNode.depth = i + 1;
                newNode.Parent = parent;
                parent.Children = new List<Node>() { newNode };
                nodes.Add(newNode);

                parent = newNode;
            }

            firstState.Tree.Depth = numberOfNodes;
            firstState.Tree.Nodes = nodes;

            firstState.Tree.UpdateLowestNodes();

            return firstState;
        }
    }
}
