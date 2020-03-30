using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    abstract class Neighbour
    {
        public abstract State Result();
        public abstract State Revert();
        public abstract double Delta();
    }
}
