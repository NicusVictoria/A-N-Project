using System;
using System.Collections.Generic;

namespace AN_Project
{
    interface INeighbourSpace<T>
    {
        Neighbour<T> GetRandomNeighbour(T data);

        List<Neighbour<T>> GetAllNeighbours(T data);
    }

    class MoveUpNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
    {
        public List<Neighbour<RecursiveTree<Node>>> GetAllNeighbours(RecursiveTree<Node> data)
        {
            throw new NotImplementedException();
        }

        public Neighbour<RecursiveTree<Node>> GetRandomNeighbour(RecursiveTree<Node> data)
        {
            RecursiveTree<Node> root = Program.allRecTreeNodes[0].Root;
            RecursiveTree<Node> randomTree = null;
            while (randomTree == root || randomTree == null)
            {
                randomTree = Program.allRecTreeNodes[Program.random.Next(Program.allRecTreeNodes.Count)];
            }
            List<RecursiveTree<Node>> parents = new List<RecursiveTree<Node>>();
            RecursiveTree<Node> parent = randomTree.Parent.Parent; // Dit werkt dus niet echt als dit een child van de root is dus tja, vandaar de comment hierboven bij de whileloop
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.Parent;
            }
            parents.Add(null); // TODO: Fix, this is supposed to be the root
            RecursiveTree<Node> randomParent = parents[Program.random.Next(parents.Count)];
            return new MoveUpNeighbour(randomTree, randomParent);
        }
    }
}
