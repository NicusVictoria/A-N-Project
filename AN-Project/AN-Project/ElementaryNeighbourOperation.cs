using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AN_Project
{
    abstract class ElementaryNeighbourOperation<D>
    {
        //public abstract double Delta();

        public abstract void Revert();
    }

    class ChangeParent : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly RecursiveTree<Node> oldParent;

        public ChangeParent(RecursiveTree<Node> node, RecursiveTree<Node> newParent)
        {
            this.node = node;
            oldParent = node.Parent;
            node.Parent = newParent;
        }

        public override void Revert()
        {
            new ChangeParent(node, oldParent);
        }
    }

    class RemoveChild : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly RecursiveTree<Node> child;

        public RemoveChild(RecursiveTree<Node> node, RecursiveTree<Node> child)
        {
            this.node = node;
            this.child = child;
            node.RemoveChild(child);
        }

        public override void Revert()
        {
            new AddChild(node, child);
        }
    }

    class RemoveAllChildren : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly ReadOnlyCollection<RecursiveTree<Node>> children;

        public RemoveAllChildren(RecursiveTree<Node> node)
        {
            this.node = node;
            children = node.Children;
            node.RemoveAllChildren();
        }

        public override void Revert()
        {
            new AddChildren(node, children);
        }
    }

    class RemoveChildren : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly IEnumerable<RecursiveTree<Node>> children;

        public RemoveChildren(RecursiveTree<Node> node, IEnumerable<RecursiveTree<Node>> children)
        {
            this.node = node;
            this.children = node.Children;
            foreach (RecursiveTree<Node> child in children)
            {
                node.RemoveChild(child);
            }
        }

        public override void Revert()
        {
            new AddChildren(node, children);
        }
    }

    class AddChild : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly RecursiveTree<Node> child;

        public AddChild(RecursiveTree<Node> node, RecursiveTree<Node> child)
        {
            this.node = node;
            this.child = child;
            node.AddChild(child);
        }

        public override void Revert()
        {
            new RemoveChild(node, child);
        }
    }

    class AddChildren : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly IEnumerable<RecursiveTree<Node>> children;

        public AddChildren(RecursiveTree<Node> node, IEnumerable<RecursiveTree<Node>> children)
        {
            this.node = node;
            this.children = children;
            node.AddChildren(children);
        }

        public override void Revert()
        {
            new RemoveChildren(node, children);
        }
    }
}
