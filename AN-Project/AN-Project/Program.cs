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
            
            string fileName = "heur_049";
            Run(fileName);
            
            /*
            for (int i = 1; i < 200; i += 2)
            {
                fileName = "heur_";
                if (i < 100) fileName += "0";
                if (i < 10) fileName += "0";
                fileName += i;
                Run(fileName);
            }
            //*/



            //string filepath = $"..\\..\\..\\..\\..\\Testcases\\{fileName}.gr";
            //inputGraph = IO.ReadInput(filepath);

            //State initialState = BaseSolutionGenerator.Empty();
            //string initialOutput = IO.WriteOutput(initialState);

            //TabuSearcher tabuSearcher = new TabuSearcher();
            //State resultState = tabuSearcher.Search();

            //string output = IO.WriteOutput(resultState);

            //Node[] inputAsNodes = IO.ReadInputAsNodes(filepath);
            //RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            //RecursiveTree<Node> recTree = recSplit.getHeuristicTree();
            //string output = RecursiveTreePrinter.PrintTree(recTree);

            //using (StreamWriter sw = new StreamWriter($"..\\..\\..\\..\\..\\Results\\{fileName}.tree", false))
            //{
            //    sw.Write(output);
            //}

            //Console.WriteLine("heuristic tree found with depth  " + recTree.Depth);
            Console.Read();
        }

        private static void Run(string fileName)
        {
            Console.WriteLine($"Starting file {fileName}...");

            Node[] inputAsNodes = IO.ReadInputAsNodes($"..\\..\\..\\..\\..\\Testcases\\{fileName}.gr");
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.getHeuristicTree();
            string output = RecursiveTreePrinter.PrintTree(recTree);

            using (StreamWriter sw = new StreamWriter($"..\\..\\..\\..\\..\\Results\\{fileName}.tree", false))
            {
                sw.Write(output);
            }

            Console.WriteLine($"Tree found with depth {recTree.Depth}.");
            Console.WriteLine();
        }
    }
}
