using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Abstract class that represents neighbours of a solution
    /// </summary>
    abstract class Neighbour
    {
        /// <summary>
        /// The result of a mutation / The new neighbour
        /// </summary>
        /// <returns>The new neighbour</returns>
        public abstract State Result();

        /// <summary>
        /// Reverts a mutation
        /// </summary>
        /// <returns>The neighbour before it was mutated</returns>
        public abstract State Revert();

        /// <summary>
        /// Calculates the difference in score of the mutation over this neighbour
        /// </summary>
        /// <returns>The difference in score between the new and old neighbours</returns>
        public abstract double Delta();
    }

    class SwapNeighbour : Neighbour
    {
        private readonly State state;
        private readonly TreeNode swapNode;

        public SwapNeighbour(State state, TreeNode swapNode)
        {
            this.state = state;
            this.swapNode = swapNode;
        }

        public override double Delta()
        {
            throw new NotImplementedException();
        }

        public override State Result()
        {
            state.Tree.SwapWithParent(state, swapNode);
            return state;
        }

        public override State Revert()
        {
            throw new NotImplementedException();
        }
    }

    class MoveUpNeighbour : Neighbour
    {
        private readonly State state;
        private readonly Node moveUpNode;

        public MoveUpNeighbour(State state, Node moveUpNode)
        {
            this.state = state;
            this.moveUpNode = moveUpNode;
        }

        public override double Delta()
        {
            throw new NotImplementedException();
        }

        public override State Result()
        {
            RecursiveTree<Node> moveUpNodeTree = FindNode(state.RecTree, moveUpNode);

            if (moveUpNodeTree == null)
            {

            }

            state.RecTree.MoveNodeUp(state, moveUpNodeTree);
            return state;
        }

        public override State Revert()
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
