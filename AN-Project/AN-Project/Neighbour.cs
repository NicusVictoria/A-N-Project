using System.Collections.Generic;
using System;

namespace AN_Project
{
    abstract class Neighbour<D> : ElementaryNeighbourOperation<D>
    {
        protected Stack<ElementaryNeighbourOperation<D>> operations = new Stack<ElementaryNeighbourOperation<D>>();

        public abstract void Apply();

        public override void Revert()
        {
            while (operations.Count > 0)
            {
                ElementaryNeighbourOperation<D> operation = operations.Pop();
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

    class MultipleNeighbourNeighbour<D> : Neighbour<D>
    {
        private readonly List<Neighbour<D>> neighbours;

        public MultipleNeighbourNeighbour(List<Neighbour<D>> neighbours)
        {
            this.neighbours = neighbours;
        }

        public override void Apply()
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                neighbours[i].Apply();
                operations.Push(neighbours[i]);
            }
        }

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

    class MoveUpNeighbour : Neighbour<RecursiveTree<Node>>
    {
        public readonly RecursiveTree<Node> moveNode;
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
            throw new NotImplementedException();
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

    class SplitNeighbour : Neighbour<RecursiveTree<Node>>
    {
        private readonly RecursiveTree<Node> splitNode;
        private readonly Dictionary<Node, int> componentIndices; // TODO: maybe more efficient to use node.number instaed of node

        public SplitNeighbour(RecursiveTree<Node> splitNode)
        {
            this.splitNode = splitNode;
            componentIndices = new Dictionary<Node, int>();
        }

        public override void Apply()
        {
            List<List<Node>> connectedComponents = FindConnectedComponents();
            if (connectedComponents.Count == splitNode.Children.Count) return;
            bool[] foundComponent = new bool[connectedComponents.Count];
            List<Tuple<RecursiveTree<Node>, RecursiveTree<Node>>> newParents = new List<Tuple<RecursiveTree<Node>, RecursiveTree<Node>>>();

            List<RecursiveTree<Node>> newChildren = DFS.All(splitNode, (n) => 
            {
                if (n == splitNode) return false;
                int index = componentIndices[n.Value];
                if (foundComponent[index])
                {
                    RecursiveTree<Node> parent = n.Parent;
                    while(componentIndices[parent.Value] != index)
                    {
                        parent = parent.Parent;
                    }
                    newParents.Add(new Tuple<RecursiveTree<Node>, RecursiveTree<Node>>(n, parent));
                    return false;
                }
                foundComponent[index] = true;
                return true;
            });

            foreach (Tuple<RecursiveTree<Node>, RecursiveTree<Node>> nodePair in newParents)
            {
                operations.Push(new RemoveChild(nodePair.Item1.Parent, nodePair.Item1));
                operations.Push(new ChangeParent(nodePair.Item1, nodePair.Item2));
                operations.Push(new AddChild(nodePair.Item2, nodePair.Item1));
            }

            foreach (RecursiveTree<Node> newChild in newChildren)
            {
                if (newChild.Parent == splitNode) continue;
                operations.Push(new RemoveChild(newChild.Parent, newChild));
                operations.Push(new ChangeParent(newChild, splitNode));
                operations.Push(new AddChild(splitNode, newChild));
            }
        }

        public override double Delta()
        {
            throw new NotImplementedException();
        }

        private List<List<Node>> FindConnectedComponents()
        {
            List<List<Node>> connectedComponents = new List<List<Node>>();
            List<Node> subNodes = splitNode.AllNodesInSubtree;
            subNodes.Remove(splitNode.Value);
            HashSet<Node> beenList = new HashSet<Node>(splitNode.AncestorNodes)
            {
                splitNode.Value
            };
            for (int i = 0; i < subNodes.Count; i++)
            {
                if (beenList.Contains(subNodes[i])) continue;
                List<Node> connectedNodes = DFS.All(subNodes[i], (n) => 
                { 
                    componentIndices[n] = connectedComponents.Count;
                    return true;
                }, beenList);
                connectedComponents.Add(connectedNodes);
            }
            return connectedComponents;
        }
    }
}