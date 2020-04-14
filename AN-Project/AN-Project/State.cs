using System;
using System.Collections.Generic;

namespace AN_Project
{
    abstract class State<S>
    {
        public S Data { get; protected set; }

        public double Score { get; protected set; }

        protected abstract List<Tuple<INeighbourSpace<S>, double>> NeighbourSpaces { get; }

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
                accum += NeighbourSpaces[index].Item2;
                index++;
            } while (accum < random);

            return NeighbourSpaces[index - 1].Item1;
        }

        public Neighbour<S> GetRandomNeighbour() 
        {
            INeighbourSpace<S> neighbourSpace = GetRandomNeighbourSpace();
            return neighbourSpace.GetRandomNeighbour(Data);
        }

        public List<Neighbour<S>> GetAllNeighbours()
        {
            List<Neighbour<S>> allNeighbours = new List<Neighbour<S>>();
            foreach (Tuple<INeighbourSpace<S>, double> tuple in NeighbourSpaces)
            {
                allNeighbours.AddRange(tuple.Item1.GetAllNeighbours(Data));
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

        protected override List<Tuple<INeighbourSpace<RecursiveTree<Node>>, double>> NeighbourSpaces
        {
            get
            {
                return new List<Tuple<INeighbourSpace<RecursiveTree<Node>>, double>>()
                {
                    new Tuple<INeighbourSpace<RecursiveTree<Node>>, double>(new MoveUpNeighbourSpace(), 1.0f)
                };
            }
        }
    }
}
