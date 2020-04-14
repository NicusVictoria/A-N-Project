using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AN_Project
{
    abstract class State<S>
    {
        public S Data { get; protected set; }

        public double Score { get; protected set; }

        protected abstract List<(INeighbourSpace<S> neighbourSpace, double chance)> NeighbourSpaces { get; }

        public State(S Data)
        {
            this.Data = Data;
        }

        protected INeighbourSpace<S> GetRandomNeighbourSpace()
        {
            if (NeighbourSpaces.Count == 0) throw new Exception("No neighbourspaces to choose from");

            double random = Program.random.NextDouble();
            double accum = 0;
            int index = 0;
            do
            {
                accum += NeighbourSpaces[index].chance;
                index++;
            } while (accum < random);

            return NeighbourSpaces[index - 1].neighbourSpace;
        }

        public Neighbour<S> GetRandomNeighbour() 
        {
            INeighbourSpace<S> neighbourSpace = GetRandomNeighbourSpace();
            return neighbourSpace.GetRandomNeighbour(Data);
        }

        public List<Neighbour<S>> GetAllNeighbours()
        {
            List<Neighbour<S>> allNeighbours = new List<Neighbour<S>>();
            foreach ((INeighbourSpace<S> neighbourSpace, double chance) in NeighbourSpaces)
            {
                allNeighbours.AddRange(neighbourSpace.GetAllNeighbours(Data));
            }
            return allNeighbours;
        }

        public Neighbour<S> ApplyRandomNeighbour()
        {
            Neighbour<S> neighbour = GetRandomNeighbour();
            ApplyNeighbour(neighbour);
            return neighbour;
        }

        public void ApplyNeighbour(Neighbour<S> neighbour)
        {
            neighbour.Apply();
            Score += neighbour.Delta();
        }

        public void RevertNeighbour(Neighbour<S> neighbour)
        {
            neighbour.Revert();
            Score -= neighbour.Delta();
        }

    }

    class RecursiveTreeState : State<RecursiveTree<Node>>
    {
        public RecursiveTreeState(RecursiveTree<Node> Data) : base(Data)
        {
            Score = Data.Root.Depth;
        }

        protected override List<(INeighbourSpace<RecursiveTree<Node>> neighbourSpace, double chance)> NeighbourSpaces
        {
            get
            {
                return new List<(INeighbourSpace<RecursiveTree<Node>> neighbourSpace, double chance)>()
                {
                    (new MoveUpNeighbourSpace(), 1.0f)
                };
            }
        }
    }
}
