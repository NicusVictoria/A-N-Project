using System.Collections.Generic;

namespace AN_Project
{
    /// <summary>
    /// Class that creates a base solution
    /// </summary>
    class BaseSolutionGenerator
    {
        /// <summary>
        /// Create a RecursiveTree with all nodes in a single line (each tree has exactly one child, except for the single leaf)
        /// </summary>
        /// <returns>The root of a RecursiveTree in a line</returns>
        public static RecursiveTree<Node> LineRecursiveTree(ref List<RecursiveTree<Node>> allRecTreeNodes, Node[] allNodes)
        {
            allRecTreeNodes = new List<RecursiveTree<Node>>();
            int numberOfNodes = allNodes.Length;
            
            // Create the root of the line
            RecursiveTree<Node> recTree = new RecursiveTree<Node>(allNodes[0]);
            allRecTreeNodes.Add(recTree);

            // Loop through all other nodes and create trees for them with the correct parent and cildren references
            RecursiveTree<Node> child = recTree;
            for (int i = 1; i < numberOfNodes; i++)
            {
                Node newNode = allNodes[i];
                RecursiveTree<Node> parent = new RecursiveTree<Node>(newNode);
                child.Parent = parent;
                parent.AddChild(child);
                child = parent;
                allRecTreeNodes.Add(parent);
            }
            return recTree;
        }
    }
}
