using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Runtime.Loader;
using System.Timers;
using System.Diagnostics;

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

        public static List<Node> allNodes;
        public static List<RecursiveTree<Node>> allRecTreeNodes;
        public static Random random = new Random();

        private static Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args)
        {
            /*
            RunHeuristicConsole();
            //*/

            /*
            RunExactConsole();
            //*/
            string fileName = "heur_001";

            /*
            ParameterizedThreadStart pm = new ParameterizedThreadStart((q) => RunSimAnnealing(fileName));
            Thread t = new Thread(pm, 1073741824);
            t.Start();
            //*/

            //RunSimAnnealing(fileName);
            //Run(fileName, true);
            //*/

            
            ParameterizedThreadStart pm = new ParameterizedThreadStart((q) => RunAllSimAnnealing());
            Thread t = new Thread(pm, 1073741824);
            t.Start();
            //*/
            /*
            RunAllHeuristic();
            //*/

            /*
            RunAllExact();
            //*/
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
            
            string output = IO.PrintTree(recTree);

            using (StreamWriter sw = new StreamWriter($"..\\..\\..\\..\\..\\Results\\{fileName}.tree", false))
            {
                sw.Write(output);
            }

            Console.WriteLine($"Tree found with depth {recTree.Depth}.");
            Console.WriteLine();
        }

        private static void RunSimAnnealing(string fileName)
        {
            Console.WriteLine($"Starting file {fileName}...");

            stopwatch.Start();

            Node[] inputAsNodes = IO.ReadInputAsNodes($"..\\..\\..\\..\\..\\Testcases\\{fileName}.gr");
            allNodes = inputAsNodes.ToList();

            SimulatedAnnealing<State<RecursiveTree<Node>>,RecursiveTree<Node>> sa = new SimulatedAnnealing<State<RecursiveTree<Node>>, RecursiveTree<Node>>();
            
            //RecursiveTreeState initialState = new RecursiveTreeState(BaseSolutionGenerator.EmptyRecursiveTree());
            
            RecursiveSplit recursiveSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> tree = recursiveSplit.GetFastHeuristicTree();
            allRecTreeNodes = tree.Root.AllRecTreeNodes;
            RecursiveTreeState heurInitState = new RecursiveTreeState(tree);

            string finalState = sa.Search(heurInitState);
            //string finalState = heurInitState.Data.ToString();

            using (StreamWriter sw = new StreamWriter($"..\\..\\..\\..\\..\\Results\\{fileName}.tree", false))
            {
                sw.Write(finalState);
            }

            stopwatch.Stop();

            Console.WriteLine($"Tree found with depth {finalState.Split('\n')[0]} in {stopwatch.Elapsed.ToString()} seconds.");
            Console.WriteLine();

            stopwatch.Reset();
        }

        private static void RunSimAnnealingConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes();
            allNodes = inputAsNodes.ToList();
            SimulatedAnnealing<State<RecursiveTree<Node>>, RecursiveTree<Node>> sa = new SimulatedAnnealing<State<RecursiveTree<Node>>, RecursiveTree<Node>>();
            RecursiveSplit recursiveSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> tree = recursiveSplit.GetFastHeuristicTree();
            allRecTreeNodes = tree.Root.AllRecTreeNodes;
            RecursiveTreeState heurInitState = new RecursiveTreeState(tree);
            string finalState = sa.Search(heurInitState);
            Console.Write(finalState);
        }

        private static void RunHeuristicConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes();
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.GetHeuristicTree();
            string output = IO.PrintTree(recTree);
            Console.Write(output);
        }
        
        private static void RunExactConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes();
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.GetBestTree();
            string output = IO.PrintTree(recTree);
            Console.Write(output);
        }

        private static void RunAllSimAnnealing()
        {
            for (int i = 1; i < 200; i += 2)
            {
                string file = "heur_";
                if (i < 10) file += $"00{i}";
                else if (i < 100) file += $"0{i}";
                else file += i;
                RunSimAnnealing(file);
            }
        }

        private static void RunAllHeuristic()
        {
            for (int i = 1; i < 200; i += 2)
            {
                string file = "heur_";
                if (i < 10) file += $"00{i}";
                else if (i < 100) file += $"0{i}";
                else file += i;
                Run(file, true);
            }
        }

        private static void RunAllExact()
        {
            for (int i = 1; i < 200; i += 2)
            {
                string file = "exact_";
                if (i < 10) file += $"00{i}";
                else if (i < 100) file += $"0{i}";
                else file += i;
                Run(file, false);
            }
        }
    }
}
