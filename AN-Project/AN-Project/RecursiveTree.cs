using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections;

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
                if (!depthCalculated)
                {
                    depth = (Children.Max(c => (int?)c.Depth) ?? 0) + 1;
                    depthCalculated = true;
                }
                return depth;
            }
        }

        public RecursiveTree<V> Root { get; set; }

        public int NumberOfNodes => Children.Sum(c => c.NumberOfNodes) + 1;

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
            RecursivelyUpdateDepth();
        }

        /// <summary>
        /// Add multiple children to this tree
        /// </summary>
        /// <param name="children">The list of children to be added</param>
        public void AddChildren(IEnumerable<RecursiveTree<V>> children)
        {
            ChildrenList.AddRange(children);
            RecursivelyUpdateDepth();
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

        public void MoveNodeUp(State state, RecursiveTree<Node> node)
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
    }
}
