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
        RecursiveTree<Node> moveNode;
        RecursiveTree<Node> newParent;
        RecursiveTree<Node> oldParent;
        RecursiveTree<Node> oldRoot;
        int startDepth;

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
        RecursiveTree<Node> node;
        RecursiveTree<Node> oldParent;

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
        RecursiveTree<Node> node;
        RecursiveTree<Node> child;

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
        RecursiveTree<Node> node;
        ReadOnlyCollection<RecursiveTree<Node>> children;

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
        RecursiveTree<Node> node;
        IEnumerable<RecursiveTree<Node>> children;

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
        RecursiveTree<Node> node;
        RecursiveTree<Node> child;

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
        RecursiveTree<Node> node;
        IEnumerable<RecursiveTree<Node>> children;

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











    /// <summary>
    /// Abstract class that represents neighbours of a solution
    /// </summary>
    abstract class OldNeighbour
    {
        /// <summary>
        /// The result of a mutation / The new neighbour
        /// </summary>
        /// <returns>The new neighbour</returns>
        public abstract OldState Result();

        /// <summary>
        /// Reverts a mutation
        /// </summary>
        /// <returns>The neighbour before it was mutated</returns>
        public abstract OldState Revert();

        /// <summary>
        /// Calculates the difference in score of the mutation over this neighbour
        /// </summary>
        /// <returns>The difference in score between the new and old neighbours</returns>
        public abstract double Delta();
    }

    class OldSwapNeighbour : OldNeighbour
    {
        private readonly OldState state;
        private readonly TreeNode swapNode;

        public OldSwapNeighbour(OldState state, TreeNode swapNode)
        {
            this.state = state;
            this.swapNode = swapNode;
        }

        public override double Delta()
        {
            throw new NotImplementedException();
        }

        public override OldState Result()
        {
            state.Tree.SwapWithParent(state, swapNode);
            return state;
        }

        public override OldState Revert()
        {
            throw new NotImplementedException();
        }
    }

    class OldMoveUpNeighbour : OldNeighbour
    {
        private readonly OldState state;
        private readonly Node moveUpNode;

        public OldMoveUpNeighbour(OldState state, Node moveUpNode)
        {
            this.state = state;
            this.moveUpNode = moveUpNode;
        }

        public override double Delta()
        {
            throw new NotImplementedException();
        }

        public override OldState Result()
        {
            RecursiveTree<Node> moveUpNodeTree = FindNode(state.RecTree, moveUpNode);

            if (moveUpNodeTree == null)
            {

            }

            //state.RecTree.MoveNodeUp(state, moveUpNodeTree);
            return state;
        }

        public override OldState Revert()
        {
            throw new NotImplementedException();
        }

        public RecursiveTree<Node> FindNode(RecursiveTree<Node> tree, Node node)
        {
            if (tree.Value.Number == node.Number) return tree;

            foreach (RecursiveTree<Node> child in tree.Children)
            {
                RecursiveTree<Node> result = FindNode(child, node);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
