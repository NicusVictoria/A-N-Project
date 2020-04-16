using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace AN_Project
{
    class Program
    { 
        /// <summary>
        /// The maximum time initial solutions can use
        /// </summary>
        public const double MAX_TIME_INITIAL_SOLUTIONS_SECONDS = 540; // 540 sec = 9 minutes

        /// <summary>
        /// The maximum total the program is allowed to use for a single instance
        /// </summary>
        public const double MAX_TIME_TOTAL_SECONDS = 1680; // 1680 sec = 28 minutes

        /// <summary>
        /// If the number of nodes is bigger than this value, no faster heuristic is used for the inital solutions
        /// </summary>
        private const int FASTER_HEURISTIC_CAP = 2500000;

        /// <summary>
        /// If the number of nodes is bigger than this value, no fast heuristic is used for the inital solutions
        /// </summary>
        private const int FAST_HEURISTIC_CAP = 500000;

        /// <summary>
        /// If the number of nodes is bigger than this value, no independent set is used for the inital solutions
        /// </summary>
        private const int INDEPENDENT_SET_CAP = 100000;

        /// <summary>
        /// If the number of nodes is bigger than this value, local search is not used
        /// </summary>
        private const int SIMULATED_ANNEALING_CAP = 200000;

        /// <summary>
        /// If the number of nodes is bigger than this value, center resemblance is not used in the heuristic
        /// </summary>
        private const int CENTER_RESEMBLANCE_CAP = 5000;

        /// <summary>
        /// If the number of nodes is at least this value, articulation points are used in the heuristic
        /// </summary>
        private const int ARTICULATION_POINT_MINIMUM = 2000;

        /// <summary>
        /// Start temperature for Simulated Annealing
        /// </summary>
        private const double START_TEMP = .3;

        /// <summary>
        /// Reset threshold for the temperature in Simulated Annealing
        /// </summary>
        private const double RESET_TEMPERATURE_THRESHOLD = 0.01;

        /// <summary>
        /// Temperature multiplier for Simulated Annealing
        /// </summary>
        private const double TEMP_MULTIPLIER = 0.995;

        /// <summary>
        /// After how many iterations the temperature should be multiplied with its multiplier
        /// </summary>
        private const int MULTIPLY_TEMP_PER_ITERATIONS = 1000;

        /// <summary>
        /// Array of all nodes in this instance
        /// </summary>
        public static Node[] allNodes;

        /// <summary>
        /// List of all recursive trees that are used in this instance
        /// </summary>
        public static List<RecursiveTree<Node>> allRecTreeNodes;

        /// <summary>
        /// Single random used throughout the program
        /// </summary>
        private readonly static Random random = new Random();

        /// <summary>
        /// Stopwatch used for a single run of a test
        /// </summary>
        private readonly static Stopwatch stopwatchSingleRun = new Stopwatch();

        /// <summary>
        /// Stopwatch used for the total of multiple runs
        /// </summary>
        private readonly static Stopwatch allRunsTotalStopwatch = new Stopwatch();

        /// <summary>
        /// Timer used to keep track of the time base solution calculations are taking
        /// </summary>
        private readonly static Stopwatch initialSolutionsTimer = new Stopwatch();

        /// <summary>
        /// The best state found thus far by either the base solutions or simulated annealing
        /// </summary>
        private static string bestStateSoFar;


        // TODO: debug stuff only
        private static int totalTreedepthFaster = 0;
        private static int totalTreedepthFast = 0;
        private static int totalTreedepthIndependentSet = 0;
        private static int totalTreedepthBest = 0;


        /// <summary>
        /// Main method for the program
        /// </summary>
        static void Main()
        {
            ParameterizedThreadStart parameterizedThreadStart;

            // Multiple run modes, select one and comment the rest

            parameterizedThreadStart = new ParameterizedThreadStart((q) => RunSimAnnealingConsole());
            //parameterizedThreadStart = new ParameterizedThreadStart((q) => RunExactConsole());

            //string fileName = "heur_199";
            //parameterizedThreadStart = new ParameterizedThreadStart((q) => RunSimAnnealing(fileName));
            //parameterizedThreadStart = new ParameterizedThreadStart((q) => RunExact(fileName));

            //parameterizedThreadStart = new ParameterizedThreadStart((q) => RunAllSimAnnealing());
            //parameterizedThreadStart = new ParameterizedThreadStart((q) => RunAllExact());

            // Create a new thread and execute the program, using 1GB of stack size
            Thread thread = new Thread(parameterizedThreadStart, 1073741824);
            thread.Start();
        }

        /// <summary>
        /// Recreates the connections between different RecursiveTrees using a string representation of the tree.
        /// </summary>
        /// <param name="treeRepresentation">The string representation of a RecursiveTree</param>
        private static void RecreateTree(string treeRepresentation)
        {
            string[] split = treeRepresentation.Split('\n');
            int numberNodes = split.Length - 2;

            // Delete all the children; they are re-added in the next loop
            for (int i = 0; i < numberNodes; i++)
            {
                allRecTreeNodes[i].RemoveAllChildren();
            }
            
            // Add the children and their parents correctly
            for (int i = 0; i < numberNodes; i++)
            {
                int parentIndex = int.Parse(split[i + 1]) - 1;
                if (parentIndex == -1)
                {
                    // If this RecursiveTree is the root, its parent is null
                    allRecTreeNodes[i].Parent = null;
                    continue;
                }
                allRecTreeNodes[i].Parent = allRecTreeNodes[parentIndex];
                allRecTreeNodes[parentIndex].AddChild(allRecTreeNodes[i]);
            }
        }

        /// <summary>
        /// Runs either the exact version of RecursiveSplit. Does not use any form of local search
        /// </summary>
        /// <param name="fileName">The name of the file to be used as input (without extension)</param>
        private static void RunExact(string fileName)
        {
            Console.WriteLine($"Starting file {fileName}...");

            Node[] inputAsNodes = IO.ReadInputAsNodes($"..\\..\\..\\..\\..\\Testcases\\{fileName}.gr", CENTER_RESEMBLANCE_CAP, ARTICULATION_POINT_MINIMUM);
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);

            RecursiveTree<Node> recTree;
            recTree = recSplit.GetBestTree();
            string output = recTree.ToString();

            using (StreamWriter sw = new StreamWriter($"..\\..\\..\\..\\..\\Results\\{fileName}.tree", false))
            {
                sw.Write(output);
            }

            Console.WriteLine($"Tree found with depth {recTree.Depth}.");
            Console.WriteLine();
        }

        /// <summary>
        /// Create base solutions and use simulated annealing to improve on these solutions
        /// </summary>
        /// <param name="fileName">The name of the file to be run</param>
        private static void RunSimAnnealing(string fileName)
        {
            SimulatedAnnealing(fileName, false);
        }

        /// <summary>
        /// Create base solutions and use simulated annealing to improve on these solutions using data from the console
        /// </summary>
        private static void RunSimAnnealingConsole()
        {
            SimulatedAnnealing();
        }

        /// <summary>
        /// Execute the program using simulated annealing
        /// </summary>
        /// <param name="fileName">The name of the file to be run, or empty if the console is used</param>
        /// <param name="console">Whether input form the console is used. If so, the program runs in quiet mode</param>
        private static void SimulatedAnnealing(string fileName = "", bool console = true)
        {
            if (!console)
            {
                Console.WriteLine($"Starting file {fileName}...");
            }

            // The stopwatch keeps track of the total running time of the program
            stopwatchSingleRun.Start();

            // Read the input, depending on whether the console is used a file is being read or console input
            Node[] inputAsNodes;
            if (!console)
            {
                inputAsNodes = IO.ReadInputAsNodes($"..\\..\\..\\..\\..\\Testcases\\{fileName}.gr", CENTER_RESEMBLANCE_CAP, ARTICULATION_POINT_MINIMUM);
            }
            else
            {
                inputAsNodes = IO.ReadInputAsNodes(CENTER_RESEMBLANCE_CAP, ARTICULATION_POINT_MINIMUM);
            }
            allNodes = inputAsNodes;

            // Create instances for the simunlated annealing and recursivesplit classes
            SimulatedAnnealing<State<RecursiveTree<Node>>, RecursiveTree<Node>> simulatedAnnealing =
                new SimulatedAnnealing<State<RecursiveTree<Node>>, RecursiveTree<Node>>(random, START_TEMP, RESET_TEMPERATURE_THRESHOLD, TEMP_MULTIPLIER, MULTIPLY_TEMP_PER_ITERATIONS, stopwatchSingleRun, MAX_TIME_TOTAL_SECONDS);
            RecursiveSplit recursiveSplit = new RecursiveSplit(inputAsNodes);

            // Find an initial solution where all nodes are in a single line
            RecursiveTreeState heurInitState = new RecursiveTreeState(BaseSolutionGenerator.LineRecursiveTree(), random);
            bestStateSoFar = heurInitState.Data.ToString();
            if (!console)
            {
                Console.WriteLine($"Line found with depth {heurInitState.Data.Root.Depth}.");
            }

            RecursiveTree<Node> bestTree = null;
            // Find an initial solution using the RecursiveSplit using a fast but bad heuristic
            if (allNodes.Length <= FASTER_HEURISTIC_CAP)
            {
                double maxTime = MAX_TIME_INITIAL_SOLUTIONS_SECONDS;
                if (allNodes.Length > INDEPENDENT_SET_CAP)
                {
                    if (allNodes.Length > FAST_HEURISTIC_CAP) maxTime *= 3;
                    else maxTime *= 1.5;
                }
                else if (allNodes.Length > FAST_HEURISTIC_CAP) maxTime *= 1.5;

                initialSolutionsTimer.Start();
                bestTree = recursiveSplit.GetFastHeuristicTree(initialSolutionsTimer, maxTime, true);
                if (!console)
                {
                    Console.WriteLine($"Fastest heuristic tree found with depth {bestTree.Depth}.");
                    totalTreedepthFaster += bestTree.Depth;
                }
            }

            // Find an initial solution using the RecursiveSplit using a decent but pretty slow heuristic
            if (allNodes.Length <= FAST_HEURISTIC_CAP)
            {
                double maxTime = MAX_TIME_INITIAL_SOLUTIONS_SECONDS;
                if (allNodes.Length > INDEPENDENT_SET_CAP)
                {
                    if (allNodes.Length > FASTER_HEURISTIC_CAP) maxTime *= 3;
                    else maxTime *= 1.5;
                }
                else if (allNodes.Length > FASTER_HEURISTIC_CAP) maxTime *= 1.5;

                RecursiveTree<Node> treeFromFastHeuristic = recursiveSplit.GetFastHeuristicTree(initialSolutionsTimer, maxTime, false);
                if (bestTree == null || treeFromFastHeuristic.Depth < bestTree.Depth)
                {
                    bestTree = treeFromFastHeuristic;
                }
                if (!console)
                {
                    Console.WriteLine($"Fast heuristic tree found with depth {treeFromFastHeuristic.Depth}.");
                    totalTreedepthFast += treeFromFastHeuristic.Depth;
                }
            }

            // If the problem instance is small enough, we would like to try to find an initial solution using independent sets
            if (allNodes.Length <= INDEPENDENT_SET_CAP)
            {
                double maxTime = MAX_TIME_INITIAL_SOLUTIONS_SECONDS;
                if (allNodes.Length > FAST_HEURISTIC_CAP)
                {
                    if (allNodes.Length > FASTER_HEURISTIC_CAP) maxTime *= 3;
                    else maxTime *= 1.5;
                }
                else if (allNodes.Length > FASTER_HEURISTIC_CAP) maxTime *= 1.5;

                RecursiveTree<Node> treeFromIndependentSet = IndependentSet.TreeFromIndependentSets(allNodes, initialSolutionsTimer, maxTime);
                if (bestTree == null || treeFromIndependentSet.Depth < bestTree.Depth)
                {
                    bestTree = treeFromIndependentSet;
                }
                if (!console)
                {
                    Console.WriteLine($"Independent set tree found with depth {treeFromIndependentSet.Depth}.");
                    totalTreedepthIndependentSet += treeFromIndependentSet.Depth;
                }

                if (bestTree != null)
                {
                    bestStateSoFar = bestTree.ToString();
                    RecreateTree(bestStateSoFar);
                }
            }

            // Recreate the tree and save the best found initial solution as the initial solution for simulated annealing
            initialSolutionsTimer.Reset();
            if (bestTree != null)
            {
                bestStateSoFar = bestTree.ToString();
                heurInitState = new RecursiveTreeState(allRecTreeNodes[0].Root, random);
                totalTreedepthBest += bestTree.Depth;
            }

            if (allNodes.Length <= SIMULATED_ANNEALING_CAP) 
            { 
                simulatedAnnealing.Search(heurInitState, ref bestStateSoFar);
            }

            // If the input from the console is not used, write the output to a file, otherwise write it back to the console
            if (!console)
            {
                using (StreamWriter sw = new StreamWriter($"..\\..\\..\\..\\..\\Results\\{fileName}.tree", false))
                {
                    sw.Write(bestStateSoFar);
                }
            }
            else
            {
                Console.Write(bestStateSoFar);
            }

            // Print the result and total time thus far
            if (!console)
            {
                // Stop the total of this problem instace
                stopwatchSingleRun.Stop();
                Console.WriteLine($"Tree found with depth {bestStateSoFar.Split('\n')[0]} in {stopwatchSingleRun.Elapsed} seconds. (Total time: {allRunsTotalStopwatch.Elapsed})");
                Console.WriteLine();
            }

            // Reset the stopwatch for the next time the program is run
            stopwatchSingleRun.Reset();
        }

        /// <summary>
        /// Create an exact solution using input data from the Console
        /// </summary>
        private static void RunExactConsole()
        {
            Node[] inputAsNodes = IO.ReadInputAsNodes(CENTER_RESEMBLANCE_CAP, ARTICULATION_POINT_MINIMUM);
            RecursiveSplit recSplit = new RecursiveSplit(inputAsNodes);
            RecursiveTree<Node> recTree = recSplit.GetBestTree();
            string output = recTree.ToString();
            Console.Write(output);
        }

        /// <summary>
        /// Runs all heuristic tests using base solutions and simulated annealing
        /// </summary>
        private static void RunAllSimAnnealing()
        {
            // This stopwatch keeps track of the total running time of all tests
            allRunsTotalStopwatch.Reset();
            allRunsTotalStopwatch.Start();

            for (int i = 1; i < 200; i += 2)
            {
                string file = "heur_";
                if (i < 10) file += $"00{i}";
                else if (i < 100) file += $"0{i}";
                else file += i;
                RunSimAnnealing(file);
            }

            Console.WriteLine($"Faster\t{totalTreedepthFaster}");
            Console.WriteLine($"Fast\t{totalTreedepthFast}");
            Console.WriteLine($"InSet\t{totalTreedepthIndependentSet}");
            Console.WriteLine($"Best\t{totalTreedepthBest}");
        }

        /// <summary>
        /// Runs all exact tests
        /// </summary>
        private static void RunAllExact()
        {
            for (int i = 1; i < 200; i += 2)
            {
                string file = "exact_";
                if (i < 10) file += $"00{i}";
                else if (i < 100) file += $"0{i}";
                else file += i;
                RunExact(file);
            }
        }
    }
}
