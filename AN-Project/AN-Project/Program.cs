using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace AN_Project
{
    class Program
    {
        public static UndirectedGraph inputGraph;
        public static int maxDegree;
        public static int[] degreeRatios;
        public static int maxDistanceCOG;
        public static int[] distanceRatiosToCOG;
        public static double averageDegreeRatio;
        public static double averageDistanceRatio;

        static void Main(string[] args)
        {
            Console.WriteLine("A&N project");

            string filepath = "..\\..\\..\\..\\..\\Testcases\\heur_049.gr";
            //inputGraph = IO.ReadInput(filepath);

            //State initialState = BaseSolutionGenerator.Empty();
            //string initialOutput = IO.WriteOutput(initialState);

            //TabuSearcher tabuSearcher = new TabuSearcher();
            //State resultState = tabuSearcher.Search();

            //string output = IO.WriteOutput(resultState);

            Node[] inputAsNodes = IO.ReadInputAsNodes(filepath);
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.getHeuristicTree();
            string output = RecursiveTreePrinter.PrintTree(recTree);

            using (StreamWriter sw = new StreamWriter("..\\..\\..\\..\\..\\Results\\heur_049.tree", false))
            {
                sw.Write(output);
            }

            Console.WriteLine("heuristic tree found with depth  " + recTree.Depth);
            Console.Read();
        }
    }
}
