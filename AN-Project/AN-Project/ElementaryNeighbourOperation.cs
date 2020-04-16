using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AN_Project
{
    /// <summary>
    /// Base class for elementary operations that are executed when computing neighbours
    /// </summary>
    /// <typeparam name="D"></typeparam>
    abstract class ElementaryNeighbourOperation<D>
    {
        // TODO: Do we want this function or not?
        //public abstract double Delta();

        /// <summary>
        /// The revert of an operation
        /// </summary>
        public abstract void Revert();
    }

    /// <summary>
    /// Changes the parent of a node
    /// </summary>
    class ChangeParentOperation : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        /// <summary>
        /// The node whose parent gets changed
        /// </summary>
        private readonly RecursiveTree<Node> node;

        /// <summary>
        /// The old parent of the node, used for reverting this change
        /// </summary>
        private readonly RecursiveTree<Node> oldParent;

        /// <summary>
        /// Changes the parent of a node
        /// </summary>
        /// <param name="node">The node that needs a new parent</param>
        /// <param name="newParent">The new parent of the node</param>
        public ChangeParentOperation(RecursiveTree<Node> node, RecursiveTree<Node> newParent)
        {
            this.node = node;
            oldParent = node.Parent;
            node.Parent = newParent;
        }

        /// <summary>
        /// Revert the parent change: change the parent back to the old parent
        /// </summary>
        public override void Revert()
        {
            new ChangeParentOperation(node, oldParent);
        }
    }

    /// <summary>
    /// Operation that removes a certain child from a node
    /// </summary>
    class RemoveChildOperation : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        /// <summary>
        /// The node from which the child is removed
        /// </summary>
        private readonly RecursiveTree<Node> node;

        /// <summary>
        /// The child that is removed
        /// </summary>
        private readonly RecursiveTree<Node> child;

        /// <summary>
        /// Operation that removes a certain child from a node
        /// </summary>
        /// <param name="node">The node from which the child is removed</param>
        /// <param name="child">The child that is removed</param>
        public RemoveChildOperation(RecursiveTree<Node> node, RecursiveTree<Node> child)
        {
            this.node = node;
            this.child = child;
            node.RemoveChild(child);
        }

        /// <summary>
        /// Revert the remove: Add the child back to the node
        /// </summary>
        public override void Revert()
        {
            new AddChildOperation(node, child);
        }
    }

    /// <summary>
    /// Operation that removes a list of children from a node
    /// </summary>
    class RemoveChildrenOperation : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        /// <summary>
        /// The node from which the children are removed
        /// </summary>
        private readonly RecursiveTree<Node> node;

        /// <summary>
        /// The children that are removed from the node
        /// </summary>
        private readonly IEnumerable<RecursiveTree<Node>> children;

        /// <summary>
        /// Operation that removes a list of children from a node
        /// </summary>
        /// <param name="node">The node from which the children are removed</param>
        /// <param name="children">The children that are removed from the node</param>
        public RemoveChildrenOperation(RecursiveTree<Node> node, IEnumerable<RecursiveTree<Node>> children)
        {
            this.node = node;
            this.children = node.Children;
            node.RemoveChildren(children);
        }

        /// <summary>
        /// Reverts removing the children: Add the children back to this node
        /// </summary>
        public override void Revert()
        {
            new AddChildrenOperation(node, children);
        }
    }

    /// <summary>
    /// Operation that removes all children from a node
    /// </summary>
    class RemoveAllChildrenOperation : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        /// <summary>
        /// The node from which the children are removed
        /// </summary>
        private readonly RecursiveTree<Node> node;

        /// <summary>
        /// The children that are removed from the node
        /// </summary>
        private readonly IEnumerable<RecursiveTree<Node>> children;

        /// <summary>
        /// Operation that removes all children from a node
        /// </summary>
        /// <param name="node">The node from which the children are removed</param>
        public RemoveAllChildrenOperation(RecursiveTree<Node> node)
        {
            this.node = node;
            children = node.Children;
            node.RemoveAllChildren();
        }

        /// <summary>
        /// Reverts removing all children: Add the removed children back to the node
        /// </summary>
        public override void Revert()
        {
            new AddChildrenOperation(node, children);
        }
    }

    /// <summary>
    /// Operation that adds a child to a node
    /// </summary>
    class AddChildOperation : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        /// <summary>
        /// The node to which the child is added
        /// </summary>
        private readonly RecursiveTree<Node> node;

        /// <summary>
        /// The added child
        /// </summary>
        private readonly RecursiveTree<Node> child;

        /// <summary>
        /// Operation that adds a child to a node
        /// </summary>
        /// <param name="node">The node to which the child is added</param>
        /// <param name="child">The child to be added</param>
        public AddChildOperation(RecursiveTree<Node> node, RecursiveTree<Node> child)
        {
            this.node = node;
            this.child = child;
            node.AddChild(child);
        }

        /// <summary>
        /// Revert the addition of a child, removes the child again
        /// </summary>
        public override void Revert()
        {
            new RemoveChildOperation(node, child);
        }
    }

    /// <summary>
    /// Operation that adds multiple children to a node
    /// </summary>
    class AddChildrenOperation : ElementaryNeighbourOperation<RecursiveTree<Node>>
    {
        /// <summary>
        /// The node to add the children to
        /// </summary>
        private readonly RecursiveTree<Node> node;

        /// <summary>
        /// The collection of children to be added
        /// </summary>
        private readonly IEnumerable<RecursiveTree<Node>> children;

        /// <summary>
        /// Operation that adds multiple children to a node
        /// </summary>
        /// <param name="node">The node to add the children to</param>
        /// <param name="children">The collection of children to be added</param>
        public AddChildrenOperation(RecursiveTree<Node> node, IEnumerable<RecursiveTree<Node>> children)
        {
            this.node = node;
            this.children = children;
            node.AddChildren(children);
        }

        /// <summary>
        /// Reverts the addition of multiple children: Removes them again
        /// </summary>
        public override void Revert()
        {
            new RemoveChildrenOperation(node, children);
        }
    }
}
