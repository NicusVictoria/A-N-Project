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
