using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections;
using System.Diagnostics;

namespace AN_Project
{
    public class RecursiveTree<V> : ITree<RecursiveTree<V>>, ITreeNode<RecursiveTree<V>>
    {
        private int depth;
        private bool depthCalculated;

        public RecursiveTree()
        {
            ScoreKeeper = new ScoreKeeper();
            ChildrenList = new List<RecursiveTree<V>>();
        }

        private List<RecursiveTree<V>> ChildrenList { get; set; }

        public ScoreKeeper ScoreKeeper { get; set; }

        public V Value { get; set; }

        public int Depth
        {
            get
            {
                //if (!depthCalculated)
                //{
                depth = (Children.Max(c => (int?)c.Depth) ?? 0) + 1;
                //depthCalculated = true;
                //}
                return depth;
            }
        }

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

        public int NumberOfNodes => Children.Sum(c => c.NumberOfNodes) + 1;

        public List<RecursiveTree<V>> AllRecTreeNodes
        {
            get
            {
                List<RecursiveTree<V>> retList = new List<RecursiveTree<V>>() { this };
                foreach(RecursiveTree<V> child in Children)
                {
                    retList.AddRange(child.AllRecTreeNodes);
                }
                return retList;
            }
        }

        public ReadOnlyCollection<RecursiveTree<V>> Children
        {
            get
            {
                return new ReadOnlyCollection<RecursiveTree<V>>(ChildrenList);
            }
        }

        public RecursiveTree<V> Parent { get; set; }

        public RecursiveTree(V n)
        {
            Value = n;
            ChildrenList = new List<RecursiveTree<V>>();
            ScoreKeeper = new ScoreKeeper();
        }

        public RecursiveTree(RecursiveTree<V> original)
        {
            Value = original.Value;
            ChildrenList = original.ChildrenList;
            ScoreKeeper = new ScoreKeeper();
        }

        public void RecursivelyUpdateDepth()
        {
            //if (depthCalculated == false) return;
            depthCalculated = false;
            if (Parent != null)
            {
                Parent.RecursivelyUpdateDepth();
            }
        }

        /// <summary>
        /// Add a child to the children of this tree
        /// </summary>
        /// <param name="child">The child to be added</param>
        public void AddChild(RecursiveTree<V> child)
        {
            ChildrenList.Add(child);
            //RecursivelyUpdateDepth();
        }

        /// <summary>
        /// Add multiple children to this tree
        /// </summary>
        /// <param name="children">The list of children to be added</param>
        public void AddChildren(IEnumerable<RecursiveTree<V>> children)
        {
            //Debug.Assert(!CheckForParentLoop()); //TODO remove this, very expensive, just for debug purposes.
            ChildrenList.AddRange(children);
            //RecursivelyUpdateDepth();
        }

        public bool CheckForParentLoop()
        {
            RecursiveTree<V> parent = Parent;
            while(parent != null)
            {
                if (parent == this) return true;
                parent = parent.Parent;
            }
            return false;
        }

        public void RemoveChild(RecursiveTree<V> child)
        {
            try { ChildrenList.Remove(child); }
            catch { throw new Exception("Child is no child of this node!"); }
        }

        public void EmptyChildrenList()
        {
            ChildrenList = new List<RecursiveTree<V>>();
        }

        /*
        public void MoveNodeUp(OldState state, RecursiveTree<Node> node)
        {
            if (node == null) throw new Exception("Node was null!");
            if (node == state.RecTree.Root) throw new Exception("Cannot move the root of the tree up!");

            RecursiveTree<Node> parent = node.Parent;

            int newDepth = Program.random.Next(0, node.depth);
            int moveUpAmount = node.depth - newDepth;

            if (newDepth == 0)
            {
                foreach (RecursiveTree<Node> child in node.Children)
                {
                    child.Parent = parent;
                }

                parent.RemoveChild(node);
                parent.AddChildren(node.Children);

                state.RecTree.Root.Parent = node;
                node.EmptyChildrenList();
                node.AddChild(state.RecTree.Root);
                node.Parent = null;
                state.RecTree.Root = node;
            }
            else
            {
                RecursiveTree<Node> newParent = node.Parent;
                RecursiveTree<Node> newChild = node;

                for (int i = 0; i < moveUpAmount; i++)
                {
                    newParent = newParent.Parent;
                    newChild = newChild.Parent;
                }

                foreach (RecursiveTree<Node> child in node.Children)
                {
                    child.Parent = parent;
                }
                parent.RemoveChild(node);

                newChild.RecursivelyUpdateDepth();

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
        */
    
        public override string ToString()
        {
            if (typeof(V) == typeof(Node))
            {
                return Root.PrintTree(Root as RecursiveTree<Node>);
            }
            else
            {
                return base.ToString();
            }
        }

        private string PrintTree(RecursiveTree<Node> tree)
        {
            int[] nodeArray = new int[tree.NumberOfNodes];

            nodeArray[tree.Value.Number - 1] = 0;

            foreach (RecursiveTree<Node> subTree in tree.Children)
            {
                PrintTree(subTree, nodeArray);
            }


            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(tree.Depth);
            stringBuilder.Append("\n");

            for (int i = 0; i < nodeArray.Length; i++)
            {
                stringBuilder.Append(nodeArray[i]);
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

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
