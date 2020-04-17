using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Tree with a node and RecursiveTrees as children
    /// </summary>
    /// <typeparam name="V">The type of nodes used in the tree</typeparam>
    public class RecursiveTree<V> : ITree<RecursiveTree<V>>, ITreeNode<RecursiveTree<V>>, INode<RecursiveTree<V>>
    {
        /// <summary>
        /// Internal list with all the children of this tree
        /// </summary>
        private List<RecursiveTree<V>> ChildrenList { get; set; }

        /// <summary>
        /// The node in this tree
        /// </summary>
        public V Value { get; set; }

        /// <summary>
        /// The depth of this subtree, gets the maximum depth of its children and adds 1 for itself (recursive)
        /// </summary>
        public int Depth => (Children.Max(c => (int?)c.Depth) ?? 0) + 1;

        /// <summary>
        /// Calcuates the root of the entire tree recursively
        /// </summary>
        public RecursiveTree<V> Root 
        {
            get
            {
                if(Parent == null)
                {
                    return this;
                }
                return Parent.Root;
            } 
        }

        /// <summary>
        /// Calculates the number of nodes in this subtree recursively
        /// </summary>
        public int NumberOfNodesInSubtree => Children.Sum(c => c.NumberOfNodesInSubtree) + 1;

        /// <summary>
        /// Gets all RecursiveTrees in this subtree
        /// </summary>
        public List<RecursiveTree<V>> AllRecTreeNodesInSubtree
        {
            get
            {
                List<RecursiveTree<V>> retList = new List<RecursiveTree<V>>() { this };
                Stack<RecursiveTree<V>> stack = new Stack<RecursiveTree<V>>(ChildrenList);
                while (stack.Count != 0)
                {
                    RecursiveTree<V> current = stack.Pop();
                    foreach (RecursiveTree<V> child in current.Children)
                    {
                        stack.Push(child);
                    }
                    retList.Add(current);
                }
                return retList;
            }
        }

        /// <summary>
        /// Gets all Vs in this subtree
        /// </summary>
        public List<V> AllNodesInSubtree
        {
            get
            {
                List<V> retList = new List<V>() { Value };
                Stack<RecursiveTree<V>> stack = new Stack<RecursiveTree<V>>(ChildrenList);
                while(stack.Count != 0)
                {
                    RecursiveTree<V> current = stack.Pop();
                    foreach(RecursiveTree<V> child in current.Children)
                    {
                        stack.Push(child);
                    }
                    retList.Add(current.Value);
                }
                return retList;
            }
        }

        /// <summary>
        /// The children of this subtree
        /// </summary>
        public ReadOnlyCollection<RecursiveTree<V>> Children => new ReadOnlyCollection<RecursiveTree<V>>(ChildrenList);

        /// <summary>
        /// The parent of this node
        /// </summary>
        public RecursiveTree<V> Parent { get; set; }

        /// <summary>
        /// List of all ancestors (in RecursiveTree form) of this subtree, including itself
        /// </summary>
        public List<RecursiveTree<V>> Ancestors
        {
            get
            {
                List<RecursiveTree<V>> retList = new List<RecursiveTree<V>>() { this };
                RecursiveTree<V> parent = Parent;
                while (parent != null)
                {
                    retList.Add(parent);
                    parent = parent.Parent;
                }
                return retList;
            }
        }

        /// <summary>
        /// List of all ancestors (in V/node form) of this subtree, including itself
        /// </summary>
        public List<V> AncestorNodes
        {
            get
            {
                List<V> retList = new List<V>() { Value };
                RecursiveTree<V> parent = Parent;
                while (parent != null)
                {
                    retList.Add(parent.Value);
                    parent = parent.Parent;
                }
                return retList;
            }
        }

        public List<RecursiveTree<V>> ConnectedNodes => ChildrenList;

        /// <summary>
        /// Constructor for the RecursiveTree
        /// </summary>
        /// <param name="n">The node in this tree</param>
        public RecursiveTree(V n)
        {
            Value = n;
            ChildrenList = new List<RecursiveTree<V>>();
        }

        /// <summary>
        /// Constructor for the RecursiveTree
        /// </summary>
        /// <param name="original">The original RecursiveTree this tree is supposed to be made off</param>
        public RecursiveTree(RecursiveTree<V> original)
        {
            Value = original.Value;
            ChildrenList = original.ChildrenList;
        }

        /// <summary>
        /// Add a child to the children of this tree
        /// </summary>
        /// <param name="child">The child to be added</param>
        public void AddChild(RecursiveTree<V> child)
        {
            ChildrenList.Add(child);
        }

        /// <summary>
        /// Add multiple children to this tree
        /// </summary>
        /// <param name="children">The list of children to be added</param>
        public void AddChildren(IEnumerable<RecursiveTree<V>> children)
        {
            ChildrenList.AddRange(children);
        }

        /// <summary>
        /// Remove a child from this RecursiveTree
        /// </summary>
        /// <param name="child">The child to be removed</param>
        public void RemoveChild(RecursiveTree<V> child)
        {
            try { ChildrenList.Remove(child); }
            catch { throw new Exception("Child is no child of this node!"); }
        }

        /// <summary>
        /// Removes multiple children from this RecursiveTree
        /// </summary>
        /// <param name="children">The children to be removed</param>
        public void RemoveChildren(IEnumerable<RecursiveTree<V>> children)
        {
            foreach (RecursiveTree<V> child in children)
            {
                RemoveChild(child);
            }
        }

        /// <summary>
        /// Remove all children of this RecursiveTree
        /// </summary>
        public void RemoveAllChildren()
        {
            ChildrenList = new List<RecursiveTree<V>>();
        }
    
        public override string ToString()
        {
            // If V is of type Node, we can use Node's properties too
            if (typeof(V) == typeof(Node))
            {
                return Root.PrintTree(Root as RecursiveTree<Node>);
            }
            else
            {
                return base.ToString();
            }
        }

        /// <summary>
        /// Prints a RecursiveTree where V is a Node
        /// </summary>
        /// <param name="root">The root of the tree</param>
        /// <returns>The tree in string representation</returns>
        private string PrintTree(RecursiveTree<Node> root)
        {
            // Save the number of the parent of each node in an array
            int[] nodeArray = new int[root.NumberOfNodesInSubtree];
            nodeArray[root.Value.Number - 1] = 0;
            foreach (RecursiveTree<Node> subTree in root.Children)
            {
                PrintTree(subTree, nodeArray);
            }

            // Use a stringbuilder to print the tree
            StringBuilder stringBuilder = new StringBuilder();
            
            // Print the depth of the complete tree
            stringBuilder.Append(root.Depth);
            stringBuilder.Append("\n");

            // Print each node's parent
            for (int i = 0; i < nodeArray.Length/*Program.allRecTreeNodes.Count*/; i++)
            {
                stringBuilder.Append(/*Program.allRecTreeNodes[i].Parent.Value*/nodeArray[i]);
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Set the value of the parent of the subtree in the nodearray and recursively calls for all children
        /// </summary>
        /// <param name="tree">The subtree to print</param>
        /// <param name="nodeArray">The array with all parent values</param>
        private void PrintTree(RecursiveTree<Node> tree, int[] nodeArray)
        {
            nodeArray[tree.Value.Number - 1] = tree.Parent.Value.Number;
            foreach (RecursiveTree<Node> subTree in tree.Children)
            {
                PrintTree(subTree, nodeArray);
            }
        }
    }
}
