using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AN_Project
{
    abstract class Neighbour<D>
    {
        protected Stack<ElementaryOperation<D>> operations = new Stack<ElementaryOperation<D>>();

        public abstract void Apply();

        public void Revert()
        {
            while (operations.Count > 0)
            {
                ElementaryOperation<D> operation = operations.Pop();
                operation.Revert();
            }
        }

        public abstract double Delta();
    }

    /*
    class NeighbourFromList<D> : Neighbour<D>
    {
        //Stack<ElementaryOperation<D>> newOperations;

        public NeighbourFromList(Stack<ElementaryOperation<D>> newOperations)
        {
            this.operations = newOperations;
        }

        public override D Apply()
        {
            
        }
    }
    */

    class MoveUpNeighbour : Neighbour<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> moveNode;
        private readonly RecursiveTree<Node> newParent;
        private readonly RecursiveTree<Node> oldParent;
        private readonly RecursiveTree<Node> oldRoot;
        private readonly int startDepth;

        public MoveUpNeighbour(RecursiveTree<Node> moveNode, RecursiveTree<Node> newParent)
        {
            this.moveNode = moveNode;
            this.newParent = newParent;
            oldParent = moveNode.Parent;
            oldRoot = moveNode.Root;
            startDepth = oldRoot.Depth;
        }

        public override double Delta()
        {
            int endDepth = moveNode.Root.Depth;
            return endDepth - startDepth;
        }

        public override void Apply()
        {
            foreach (RecursiveTree<Node> child in moveNode.Children)
            {
                operations.Push(new ChangeParent(child, oldParent));
            }
            operations.Push(new RemoveChild(oldParent, moveNode));
            operations.Push(new AddChildren(oldParent, moveNode.Children));
            if (newParent != null)
            {
                foreach (RecursiveTree<Node> child in newParent.Children)
                {
                    operations.Push(new ChangeParent(child, moveNode));
                }
            }
            else
            {
                operations.Push(new ChangeParent(oldRoot, moveNode));
            }
            operations.Push(new RemoveAllChildren(moveNode));
            operations.Push(new ChangeParent(moveNode, newParent));
            if (newParent != null)
            {
                operations.Push(new AddChildren(moveNode, newParent.Children));
                operations.Push(new RemoveAllChildren(newParent));
                operations.Push(new AddChild(newParent, moveNode));
            }
            else
            {
                operations.Push(new AddChild(moveNode, oldRoot));
            }
        }
    }

    abstract class ElementaryOperation<D>
    {
        //public abstract double Delta();
        
        public abstract ElementaryOperation<D> Revert();
    }

    class ChangeParent : ElementaryOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly RecursiveTree<Node> oldParent;

        public ChangeParent(RecursiveTree<Node> node, RecursiveTree<Node> newParent)
        {
            this.node = node;
            oldParent = node.Parent;
            node.Parent = newParent;
        }

        public override ElementaryOperation<RecursiveTree<Node>> Revert()
        {
            return new ChangeParent(node, oldParent);
        }
    }

    class RemoveChild : ElementaryOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly RecursiveTree<Node> child;

        public RemoveChild(RecursiveTree<Node> node, RecursiveTree<Node> child)
        {
            this.node = node;
            this.child = child;
            node.RemoveChild(child);
        }

        public override ElementaryOperation<RecursiveTree<Node>> Revert()
        {
            return new AddChild(node, child);
        }
    }

    class RemoveAllChildren : ElementaryOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly ReadOnlyCollection<RecursiveTree<Node>> children;

        public RemoveAllChildren(RecursiveTree<Node> node)
        {
            this.node = node;
            children = node.Children;
            node.EmptyChildrenList();
        }

        public override ElementaryOperation<RecursiveTree<Node>> Revert()
        {
            return new AddChildren(node, children);
        }
    }

    class RemoveChildren : ElementaryOperation<RecursiveTree<Node>>
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

        public override ElementaryOperation<RecursiveTree<Node>> Revert()
        {
            return new AddChildren(node, children);
        }
    }

    class AddChild : ElementaryOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly RecursiveTree<Node> child;

        public AddChild(RecursiveTree<Node> node, RecursiveTree<Node> child)
        {
            this.node = node;
            this.child = child;
            node.AddChild(child);
        }

        public override ElementaryOperation<RecursiveTree<Node>> Revert()
        {
            return new RemoveChild(node, child);
        }
    }

    class AddChildren : ElementaryOperation<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> node;
        private readonly IEnumerable<RecursiveTree<Node>> children;

        public AddChildren(RecursiveTree<Node> node, IEnumerable<RecursiveTree<Node>> children)
        {
            this.node = node;
            this.children = children;
            node.AddChildren(children);
        }

        public override ElementaryOperation<RecursiveTree<Node>> Revert()
        {
            return new RemoveChildren(node, children);
        }
    }
}