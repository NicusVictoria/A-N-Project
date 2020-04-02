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
            //Console.WriteLine("A&N project");

            /*
            RunHeuristicConsole();
            //*/

            /*
            RunExactConsole();
            //*/

            
            string fileName = "exact_001";
            Run(fileName, false);
            //*/


            /*
            for (int i = 1; i < 200; i += 2)
            {
                string file = "exact_";
                if (i < 10) file += $"00{i}";
                else if (i < 100) file += $"0{i}";
                else file += i;
                Run(file, true);
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
            //Console.Read();
        }

        private static void Run(string fileName, bool heuristic = true)
        {
            Console.WriteLine($"Starting file {fileName}...");

            Node[] inputAsNodes = IO.ReadInputAsNodes($"..\\..\\..\\..\\..\\Testcases\\{fileName}.gr");
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);

            RecursiveTree<Node> recTree;
            if (heuristic)
            {
                recTree = recSplit.GetHeuristicTree();
            }
            else
            {
                recTree = recSplit.GetBestTree();
            }
            
            string output = RecursiveTreePrinter.PrintTree(recTree);

            using (StreamWriter sw = new StreamWriter($"..\\..\\..\\..\\..\\Results\\{fileName}.tree", false))
            {
                sw.Write(output);
            }

            Console.WriteLine($"Tree found with depth {recTree.Depth}.");
            Console.WriteLine();
        }

        private static void RunHeuristicConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes();
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.GetHeuristicTree();
            string output = RecursiveTreePrinter.PrintTree(recTree);
            Console.Write(output);
        }
        
        private static void RunExactConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes();
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.GetBestTree();
            string output = RecursiveTreePrinter.PrintTree(recTree);
            Console.Write(output);
        }
    }
}
