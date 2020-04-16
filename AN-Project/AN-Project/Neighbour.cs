using System.Collections.Generic;
using System;

namespace AN_Project
{
    /// <summary>
    /// Abstract class for a neighbour
    /// </summary>
    /// <typeparam name="D">The type of data used. This type is the one where neighbours are created from</typeparam>
    abstract class Neighbour<D> : ElementaryNeighbourOperation<D>
    {
        /// <summary>
        /// Stack containing all operations that were used to create a neighbour. This stack is used when reverting a neighbour
        /// </summary>
        protected Stack<ElementaryNeighbourOperation<D>> operations = new Stack<ElementaryNeighbourOperation<D>>();

        /// <summary>
        /// Applies this neighbour
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// Reverts this neighbour
        /// </summary>
        public override void Revert()
        {
            // Pop all operations from the stack and revert them. This way the operations are reverted in the other way around than they were added
            while (operations.Count > 0)
            {
                ElementaryNeighbourOperation<D> operation = operations.Pop();
                operation.Revert();
            }
        }

        /// <summary>
        /// Computes the difference in score from this neighbour compared to the original data
        /// </summary>
        /// <returns>The difference in score from this neighbour compared to the original data</returns>
        public abstract double Delta();
    }

    // TODO: can this be removed?
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

    /// <summary>
    /// A neighbour consisting of multiple neighbours executed in order
    /// </summary>
    /// <typeparam name="D">The type of data used. This type is the one where neighbours are created from</typeparam>
    class MultipleNeighbourNeighbour<D> : Neighbour<D>
    {
        /// <summary>
        /// The list of neighbours this neighbour consists of
        /// </summary>
        private readonly List<Neighbour<D>> neighbours;

        /// <summary>
        /// Create a neighbour from multiple neighbours. Neighbours are applied in the order they are in the parameter
        /// </summary>
        /// <param name="neighbours">The neighbours this neighbour consists of</param>
        public MultipleNeighbourNeighbour(List<Neighbour<D>> neighbours)
        {
            this.neighbours = neighbours;
        }

        /// <summary>
        /// Applies the neighbour. The single neighbours are applied one by one
        /// </summary>
        public override void Apply()
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                neighbours[i].Apply();
                operations.Push(neighbours[i]);
            }
        }

        /// <summary>
        /// Computes the difference in score for the multiple neighbours. The individual delta's are summed for this result
        /// </summary>
        /// <returns>The total difference in score for all the individual neighbours</returns>
        public override double Delta()
        {
            double total = 0;
            for (int i = 0; i < neighbours.Count; i++)
            {
                total += neighbours[i].Delta();
            }
            return total;
        }
    }

    /// <summary>
    /// Neighbour that moves a node up into the tree
    /// </summary>
    class MoveUpNeighbour : Neighbour<RecursiveTree<Node>>
    {
        /// <summary>
        /// The new parent of the node that is moved
        /// </summary>
        private readonly RecursiveTree<Node> newParent;

        /// <summary>
        /// The old parent of the node that is moved
        /// </summary>
        private readonly RecursiveTree<Node> oldParent;

        /// <summary>
        /// The old root of the tree
        /// </summary>
        private readonly RecursiveTree<Node> oldRoot;

        /// <summary>
        /// The depth of the tree before the move happens
        /// </summary>
        private readonly int startDepth;

        /// <summary>
        /// The node that is moved up
        /// </summary>
        public RecursiveTree<Node> MoveNode { get; }

        /// <summary>
        /// Neighbour that moves a node up into the tree
        /// </summary>
        /// <param name="moveNode">The node that is moved up</param>
        /// <param name="newParent">The new parent of the node</param>
        public MoveUpNeighbour(RecursiveTree<Node> moveNode, RecursiveTree<Node> newParent)
        {
            MoveNode = moveNode;
            this.newParent = newParent;
            oldParent = moveNode.Parent;
            oldRoot = moveNode.Root;
            startDepth = oldRoot.Depth;
        }

        public override double Delta()
        {
            throw new NotImplementedException(); // TODO: implement
        }

        public override void Apply()
        {
            // The children of the node that moves up are moved to the old parent of the node
            foreach (RecursiveTree<Node> child in MoveNode.Children)
            {
                operations.Push(new ChangeParentOperation(child, oldParent));
            }
            operations.Push(new RemoveChildOperation(oldParent, MoveNode));
            operations.Push(new AddChildrenOperation(oldParent, MoveNode.Children));

            // If the node that moves is not the new root
            if (newParent != null)
            {
                // The children of the new parent are moved to be children of the node that moves
                foreach (RecursiveTree<Node> child in newParent.Children)
                {
                    operations.Push(new ChangeParentOperation(child, MoveNode));
                }
            }
            // The old root is now a child of this node
            else
            {
                operations.Push(new ChangeParentOperation(oldRoot, MoveNode));
            }

            // Remove all children of the node that moves and change its parent to the new parent
            operations.Push(new RemoveAllChildrenOperation(MoveNode));
            operations.Push(new ChangeParentOperation(MoveNode, newParent));

            // If the node that moves is not the new root
            if (newParent != null)
            {
                // Add the children of the new parent to the move node and set the move node as child of the new parent
                operations.Push(new AddChildrenOperation(MoveNode, newParent.Children));
                operations.Push(new RemoveAllChildrenOperation(newParent));
                operations.Push(new AddChildOperation(newParent, MoveNode));
            }
            // It is the new root, so the old root is its new child
            else
            {
                operations.Push(new AddChildOperation(MoveNode, oldRoot));
            }
        }
    }

    /// <summary>
    /// Splits a subtree into multiple subtrees
    /// </summary>
    class SplitNeighbour : Neighbour<RecursiveTree<Node>>
    {
        /// <summary>
        /// The node that is the root of the subtree that is going to be split
        /// </summary>
        private readonly RecursiveTree<Node> splitNode;

        /// <summary>
        /// Dictionary from node to its index
        /// </summary>
        private readonly Dictionary<Node, int> componentIndices;

        /// <summary>
        /// Splits a subtree into multiple subtrees
        /// </summary>
        /// <param name="splitNode">The root of the subtree that is going to be split</param>
        public SplitNeighbour(RecursiveTree<Node> splitNode)
        {
            this.splitNode = splitNode;
            componentIndices = new Dictionary<Node, int>();
        }

        public override void Apply()
        {
            // Find all the connected components in this subtree
            List<List<Node>> connectedComponents = FindConnectedComponents();

            // If the number of connected components is equal to the number of children, we cannot split the tree even further, so return
            if (connectedComponents.Count == splitNode.Children.Count)
            {
                return;
            }
            
            // Consider a subtree where the middle part is a connected component. We want that part as a new child, and the rest as a new child, but connected to each other
            // Thus, if we remove the middle part of a connected component, we want to reconnect this component
            // In each component, find the new children of the splitnode, and possibly new parents for nodes that were cut off
            bool[] foundComponent = new bool[connectedComponents.Count];
            List<Tuple<RecursiveTree<Node>, RecursiveTree<Node>>> newParents = new List<Tuple<RecursiveTree<Node>, RecursiveTree<Node>>>();
            List<RecursiveTree<Node>> newChildren = DFS.All(splitNode, (n) => 
            {
                if (n == splitNode) return false;
                int index = componentIndices[n.Value];
                if (foundComponent[index])
                {
                    // If we have already found this component, find the first parent that is also part of this component. This node is now namely divided from the rest of this component
                    RecursiveTree<Node> parent = n.Parent;
                    while (componentIndices[parent.Value] != index)
                    {
                        parent = parent.Parent;
                    }
                    newParents.Add(new Tuple<RecursiveTree<Node>, RecursiveTree<Node>>(n, parent));
                    return false;
                }
                foundComponent[index] = true;
                return true;
            });

            // Reconnect the divided components
            foreach (Tuple<RecursiveTree<Node>, RecursiveTree<Node>> nodePair in newParents)
            {
                operations.Push(new RemoveChildOperation(nodePair.Item1.Parent, nodePair.Item1));
                operations.Push(new ChangeParentOperation(nodePair.Item1, nodePair.Item2));
                operations.Push(new AddChildOperation(nodePair.Item2, nodePair.Item1));
            }

            // Connect the new children to the split node
            foreach (RecursiveTree<Node> newChild in newChildren)
            {
                if (newChild.Parent == splitNode) continue;
                operations.Push(new RemoveChildOperation(newChild.Parent, newChild));
                operations.Push(new ChangeParentOperation(newChild, splitNode));
                operations.Push(new AddChildOperation(splitNode, newChild));
            }
        }

        public override double Delta()
        {
            throw new NotImplementedException(); // TODO: implement
        }

        /// <summary>
        /// Find the connected components in the subtree
        /// </summary>
        /// <returns></returns>
        private List<List<Node>> FindConnectedComponents()
        {
            List<List<Node>> connectedComponents = new List<List<Node>>();
            List<Node> subNodes = splitNode.AllNodesInSubtree;
            subNodes.Remove(splitNode.Value);
            HashSet<Node> beenList = new HashSet<Node>(splitNode.AncestorNodes) { splitNode.Value };
            
            // Use DFS to find all connected components
            for (int i = 0; i < subNodes.Count; i++)
            {
                if (beenList.Contains(subNodes[i])) continue;
                List<Node> connectedNodes = DFS.All(subNodes[i], (n) => 
                { 
                    // When a node is explored, add the index of the component it is in to the dictionary
                    componentIndices[n] = connectedComponents.Count;
                    return true;
                }, beenList);
                connectedComponents.Add(connectedNodes);
            }
            return connectedComponents;
        }
    }
}