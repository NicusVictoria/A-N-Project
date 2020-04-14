using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace AN_Project
{
    class Program
    {
        public const int MAXTIMESECONDS = 480;

        public static List<Node> allNodes;
        public static List<RecursiveTree<Node>> allRecTreeNodes;
        public static Random random = new Random();

        private readonly static Stopwatch stopwatch = new Stopwatch();
        private readonly static Stopwatch cumulStopwatch = new Stopwatch();
        private readonly static Stopwatch timer = new Stopwatch();

        public static string bestStateSoFar;

        static void Main()
        {
            //Console.CancelKeyPress += ConsoleOnCancelKeyPressed;

            /*
            ParameterizedThreadStart pm = new ParameterizedThreadStart((q) => RunSimAnnealingConsole());
            Thread t = new Thread(pm, 1073741824);
            t.Start();
            //*/

            /*
            RunHeuristicConsole();
            //*/

            /*
            RunExactConsole();
            //*/

            
            string fileName = "heur_001";
            ParameterizedThreadStart pm = new ParameterizedThreadStart((q) => RunSimAnnealing(fileName));
            Thread t = new Thread(pm, 1073741824);
            t.Start();
            //*/

            /*
            RunSimAnnealing(fileName);
            Run(fileName, true);
            //*/

            /*
            ParameterizedThreadStart pm2 = new ParameterizedThreadStart((q) => RunAllSimAnnealing());
            Thread t2 = new Thread(pm2, 1073741824);
            t2.Start();
            //*/
            
            /*
            RunAllHeuristic();
            //*/

            /*
            RunAllExact();
            //*/
        }

        private static void ConsoleOnCancelKeyPressed(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine(bestStateSoFar);
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

            string output = recTree.ToString();

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
            RecursiveSplit recursiveSplit = new RecursiveSplit(inputAsNodes);

            RecursiveTreeState heurInitStateLine = new RecursiveTreeState(BaseSolutionGenerator.EmptyRecursiveTree());
            Console.WriteLine($"Line found with depth {heurInitStateLine.Data.Root.Depth}.");
            bestStateSoFar = heurInitStateLine.Data.ToString();

            timer.Start();
            RecursiveTree<Node> bestTree = recursiveSplit.GetFastHeuristicTree(timer, true);
            Console.WriteLine($"Fastest heuristic tree found with depth {bestTree.Depth}.");

            timer.Restart();
            RecursiveTree<Node> treeFromFastHeuristic = recursiveSplit.GetFastHeuristicTree(timer, false);
            Console.WriteLine($"Fast heuristic tree found with depth {treeFromFastHeuristic.Depth}.");
            if (treeFromFastHeuristic.Depth < bestTree.Depth)
            {
                bestTree = treeFromFastHeuristic;
            }

            timer.Restart();
            RecursiveTree<Node> treeFromIndependentSet = IndependentSet.TreeFromIndependentSets(allNodes, timer);
            Console.WriteLine($"Independent set tree found with depth {treeFromIndependentSet.Depth}.");
            if (treeFromIndependentSet.Depth < bestTree.Depth)
            {
                bestTree = treeFromIndependentSet;
            }

            timer.Reset();
            allRecTreeNodes = bestTree.Root.AllRecTreeNodes;
            RecursiveTreeState heurInitState = new RecursiveTreeState(bestTree);

            bestStateSoFar = sa.Search(heurInitState);
            //string finalState = heurInitState.Data.ToString();

            using (StreamWriter sw = new StreamWriter($"..\\..\\..\\..\\..\\Results\\{fileName}.tree", false))
            {
                sw.Write(bestStateSoFar);
            }

            stopwatch.Stop();

            Console.WriteLine($"Tree found with depth {bestStateSoFar.Split('\n')[0]} in {stopwatch.Elapsed} seconds. (Total time: {cumulStopwatch.Elapsed})");
            Console.WriteLine();

            stopwatch.Reset();
        }
        private static void RunSimAnnealingConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes();
            allNodes = inputAsNodes.ToList();
            SimulatedAnnealing<State<RecursiveTree<Node>>, RecursiveTree<Node>> sa = new SimulatedAnnealing<State<RecursiveTree<Node>>, RecursiveTree<Node>>();
            RecursiveSplit recursiveSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTreeState heurInitStateLine = new RecursiveTreeState(BaseSolutionGenerator.EmptyRecursiveTree());
            bestStateSoFar = heurInitStateLine.Data.ToString();
            timer.Start();
            RecursiveTree<Node> bestTree = recursiveSplit.GetFastHeuristicTree(timer, true);
            timer.Restart();
            RecursiveTree<Node> treeFromFastHeuristic = recursiveSplit.GetFastHeuristicTree(timer, false);
            if (treeFromFastHeuristic.Depth < bestTree.Depth)
            {
                bestTree = treeFromFastHeuristic;
            }
            timer.Restart();
            RecursiveTree<Node> treeFromIndependentSet = IndependentSet.TreeFromIndependentSets(allNodes, timer);
            if (treeFromIndependentSet.Depth < bestTree.Depth)
            {
                bestTree = treeFromIndependentSet;
            }
            timer.Reset();
            allRecTreeNodes = bestTree.Root.AllRecTreeNodes;
            RecursiveTreeState heurInitState = new RecursiveTreeState(bestTree);
            bestStateSoFar = sa.Search(heurInitState);
            Console.Write(bestStateSoFar);
            stopwatch.Reset();
        }

        private static void RunHeuristicConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes();
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.GetHeuristicTree();
            string output = recTree.ToString();
            Console.Write(output);
        }
        
        private static void RunExactConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes();
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.GetBestTree();
            string output = recTree.ToString();
            Console.Write(output);
        }

        private static void RunAllSimAnnealing()
        {
            cumulStopwatch.Reset();
            cumulStopwatch.Start();
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
