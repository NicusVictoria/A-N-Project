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
        private State state;
        private readonly State originalState;
        private Node swapNode;

        public SwapNeighbour(State state, Node swapNode)
        {
            this.state = state;
            this.swapNode = swapNode;

            originalState = state.Clone();
        }

        public override double Delta()
        {
            throw new NotImplementedException();
        }

        public override State Result()
        {
            state.Tree.SwapWithParent(swapNode);
            return state;
        }

        public override State Revert()
        {
            return originalState;
        }
    }
}
