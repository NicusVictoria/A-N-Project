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

    class SplitNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
    {
        public List<Neighbour<RecursiveTree<Node>>> GetAllNeighbours(RecursiveTree<Node> data)
        {
            throw new NotImplementedException();
        }

        public Neighbour<RecursiveTree<Node>> GetRandomNeighbour(RecursiveTree<Node> data)
        {
            RecursiveTree<Node> root = Program.allRecTreeNodes[0].Root;
            RecursiveTree<Node> randomTree = null;
            while (randomTree == null || randomTree.Children.Count == 0)
            {
                randomTree = Program.allRecTreeNodes[Program.random.Next(Program.allRecTreeNodes.Count)];
            }
            return new SplitNeighbour(randomTree);
        }
    }

    class MoveAndSplitNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
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
            MoveUpNeighbour move = new MoveUpNeighbour(randomTree, randomParent);
            SplitNeighbour split = new SplitNeighbour(randomTree.Parent);
            List<Neighbour<RecursiveTree<Node>>> neighbours = new List<Neighbour<RecursiveTree<Node>>>()
            {
                move, split
            };
            return new MultipleNeighbourNeighbour<RecursiveTree<Node>>(neighbours);
        }
    }

    class RandomMoveAndSplitNeighbourSpace : INeighbourSpace<RecursiveTree<Node>>
    {
        private readonly int splits;

        public RandomMoveAndSplitNeighbourSpace(int splits)
        {
            this.splits = splits;
        }

        public List<Neighbour<RecursiveTree<Node>>> GetAllNeighbours(RecursiveTree<Node> data)
        {
            throw new NotImplementedException();
        }

        public Neighbour<RecursiveTree<Node>> GetRandomNeighbour(RecursiveTree<Node> data)
        {
            List<Neighbour<RecursiveTree<Node>>> neighbourlist = new List<Neighbour<AN_Project.RecursiveTree<Node>>>();
            MoveUpNeighbourSpace moveSpace = new MoveUpNeighbourSpace();
            MoveUpNeighbour move = (moveSpace.GetRandomNeighbour(data)) as MoveUpNeighbour;
            neighbourlist.Add(move);

            SplitNeighbourSpace splitSpace = new SplitNeighbourSpace();
            for (int i = 0; i < splits; i++)
            {
                Neighbour<RecursiveTree<Node>> split = splitSpace.GetRandomNeighbour(data);
                neighbourlist.Add(split);
            }
            return new MultipleNeighbourNeighbour<RecursiveTree<Node>>(neighbourlist);
        }
    }
}
