using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AN_Project
{
    /// <summary>
    /// Class containing the current state of the program
    /// </summary>
    public class State
    {
        public readonly List<Node> allNodes;

        public State()
        {

        }

        public State(List<Node> allNodes)
        {
            this.allNodes = allNodes;
        }

        public Tree Tree { get; set; }

        public RecursiveTree<Node> RecTree { get; set; }    

        public State Clone()
        {
            return this.Copy();
        }
    }
}
