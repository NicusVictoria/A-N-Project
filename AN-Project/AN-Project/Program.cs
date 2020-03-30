using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AN_Project
{
    class Program
    {
        public static UndirectedGraph inputGraph;

        static void Main(string[] args)
        {
            Console.WriteLine("A&N project");

            string filepath = "..\\..\\..\\..\\..\\Testcases\\heur_001.gr";
            inputGraph = IO.ReadInput(filepath);

            State initialState = BaseSolutionGenerator.Empty();

            string output = IO.WriteOutput(initialState);
        }
    }
}
