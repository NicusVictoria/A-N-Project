using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections;

namespace AN_Project
{
    class RecursiveTree<V> : ITree<RecursiveTree<V>>, ITreeNode<RecursiveTree<V>>
    {
        private int depth;
        private bool depthCalculated;

        private List<RecursiveTree<V>> ChildrenList { get; set; }

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

        public RecursiveTree<V> Root => this;

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
        }

        public RecursiveTree(RecursiveTree<V> original)
        {
            Value = original.Value;
            ChildrenList = original.ChildrenList;
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
        public void AddChildren(List<RecursiveTree<V>> children)
        {
            ChildrenList.AddRange(children);
            RecursivelyUpdateDepth();
        }
    }
}
