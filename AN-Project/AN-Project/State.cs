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
        public Tree Tree { get; set; }

        public State Clone()
        {
            return this.Copy();
        }
    }
}
