using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace AN_Project
{
    /// <summary>
    /// Class that computes a RecursiveTree by splitting
    /// </summary>
    class RecursiveSplit
    {
        // Global datastructures that are used to avoid having to create new ones and hope the GC collects the others, which led to a lot of memory issues
        private readonly Node[] allNodes;
        private readonly HashSet<Node> beenList;
        private readonly HashSet<Node> nodesAsHash;
        private readonly List<List<Node>> connectedComponents;
        private IEnumerable<Node> subArray;

        /// <summary>
        /// Constructor for a RecursiveSplit
        /// </summary>
        /// <param name="allNodes">Array of all nodes in the instance</param>
        public RecursiveSplit(Node[] allNodes)
        {
            this.allNodes = allNodes;
            beenList = new HashSet<Node>();
            nodesAsHash = new HashSet<Node>();
            connectedComponents = new List<List<Node>>();
        }

        /// <summary>
        /// Approximates an optimal treedepth decomposition using heuristics
        /// </summary>
        /// <param name="timer">A RUNNING! timer to keep track of how long this computation is allowed to take</param>
        /// <param name="maxTimeAllowed">The maximum time this computation is allowed to take</param>
        /// <param name="fast">Whether heuristics should be updated during the computation. Fast means they are not updated</param>
        /// <returns>The resulting heuristic tree</returns>
        public RecursiveTree<Node> GetHeuristicTree(Stopwatch timer, double maxTimeAllowed, bool fast = true)
        {
            Node[] nodes = (Node[])allNodes.Clone();
            return RecGetHeuristicTree(nodes, new HashSet<Node>(), 0, allNodes.Length, timer, maxTimeAllowed, fast);
        }

        /// <summary>
        /// Recursive method used for computing the heuristic tree, computes the tree for a subset of nodes
        /// </summary>
        /// <param name="nodes">All nodes in the entire tree</param>
        /// <param name="ancestors">The list of ancestors for the current subtree</param>
        /// <param name="left">The INCLUSIVE index in nodes where the current subtree starts</param>
        /// <param name="right">The EXCLUSIVE index in nodes where the current subtree ends</param>
        /// <param name="timer">A RUNNING! timer to keep track of how long this computation is allowed to take</param>
        /// <param name="maxTimeAllowed">The maximum time this computation is allowed to take</param>
        /// <param name="fast">Whether heuristics should be updated during the computation. Fast means they are not updated</param>
        /// <returns>The resulting heuristic tree for this subset of nodes</returns>
        private RecursiveTree<Node> RecGetHeuristicTree(Node[] nodes, HashSet<Node> ancestors, int left, int right, Stopwatch timer, double maxTimeAllowed, bool fast) // Left inclusive, right exclusive
        {
            // Compute the array for this subset of nodes
            subArray = nodes.Skip(left).Take(right - left);

            // If the time is up, return the nodes in a single line with for this subtree
            if (timer.Elapsed.TotalSeconds >= maxTimeAllowed)
            {
                return CreateLine(subArray.ToList());
            }

            // If the new tree has only one node, return this node
            if (left - right == 1) return new RecursiveTree<Node>(nodes[left]);

            // Select a node as the root of this subtree based on the heuristics of the nodes in this subgraph and compute the connected component when this node is removed
            // These components are the subtrees that are children of the selected node
            Node selectedNode = GetHeuristicNode(subArray, fast);
            RecursiveTree<Node> newTree = new RecursiveTree<Node>(selectedNode);
            ComputeConnectedComponents(nodes, ancestors, selectedNode, left, right);
            Tuple<int, int>[] borders = ComputeNewSubgraphBorders(nodes, left);

            // For each of the new subtrees, do a recursive call to this method to compute it
            for (int i = 0; i < borders.Length; i++)
            {
                RecursiveTree<Node> ChildTree = RecGetHeuristicTree(nodes, ancestors, borders[i].Item1, borders[i].Item2, timer, maxTimeAllowed, fast);
                ChildTree.Parent = newTree;
                newTree.AddChild(ChildTree);
            }

            // Remove the selected node from the ancestors so the possible other subtrees of the parent of this subtree do not accidentaly use it. 
            // We do this to avoid having to copy the entire ancestors list for each recursive call
            ancestors.Remove(selectedNode);
            return newTree;
        }

        /// <summary>
        /// Selects the node in a subarray with the highest heuristic
        /// </summary>
        /// <param name="subArray">The array to select the node from</param>
        /// <param name="fast">Whether heuristics should be updated during the computation. Fast means they are not updated</param>
        /// <returns>The node with the highest heuristic value in this subarray</returns>
        private Node GetHeuristicNode(IEnumerable<Node> subArray, bool fast)
        {
            if (fast) return subArray.Max();

            // Compute the heuristic of each node in the remaining graph and select the node with the highest heuristic in it
            Node selectedNode = null;
            double maxHeuristic = -1;
            nodesAsHash.Clear();
            nodesAsHash.UnionWith(subArray);
            foreach (Node n in subArray)
            {
                double nRemainingDegree = n.RemainingDegree(nodesAsHash) + n.CenterResemblance;
                if (nRemainingDegree > maxHeuristic)
                {
                    maxHeuristic = nRemainingDegree;
                    selectedNode = n;
                }
            }
            return selectedNode;
        }

        /// <summary>
        /// Compute the connected components in a set of nodes and fills these in the list connectedComponents
        /// </summary>
        /// <param name="nodes">All nodes in the entire tree</param>
        /// <param name="ancestors">The list of ancestors for the current subtree</param>
        /// <param name="selectedNode">The node that is the root of the current subtree</param>
        /// <param name="left">The INCLUSIVE index in nodes where the current subtree starts</param>
        /// <param name="right">The EXCLUSIVE index in nodes where the current subtree ends</param>
        private void ComputeConnectedComponents(Node[] nodes, HashSet<Node> ancestors, Node selectedNode, int left, int right)
        {
            ancestors.Add(selectedNode);
            beenList.Clear();
            beenList.UnionWith(ancestors);
            connectedComponents.Clear();

            // For all nodes in the current subtree, do a DFS to find the connected components
            for (int i = left; i < right; i++)
            {
                if (beenList.Contains(nodes[i])) continue;
                List<Node> connectedNodes = DFS.All(nodes[i], node => { return true; }, beenList);
                connectedComponents.Add(connectedNodes);
            }
        }

        /// <summary>
        /// Computes the new left and right indices for the new subtrees that are children of the current subtree
        /// </summary>
        /// <param name="nodes">The array with all nodes</param>
        /// <param name="left">The INCLUSIVE index where the current subtree starts, the right index is not needed because it uses the number of nodes in the connected components to compute the borders</param>
        /// <returns>An array of tuples with left and right indices of the connected components</returns>
        private Tuple<int, int>[] ComputeNewSubgraphBorders(Node[] nodes, int left)
        {
            Tuple<int, int>[] borders = new Tuple<int, int>[connectedComponents.Count];
            int index = left;

            // Compute the border per connected component
            for (int i = 0; i < connectedComponents.Count; i++)
            {
                // Set the left border of this component, the right index is 0 for now. It will be updated when we reach the right border
                borders[i] = new Tuple<int, int>(index, 0);
                List<Node> component = connectedComponents[i];

                // Overwrite the values in nodes by the values in the connected components. All values in the part of nodes we are overwriting are stored in connectedcomponents.
                // We are just grouping the connected components. Instead of swapping each node, we are first grouping the components in lists, and then pasting these lists back in nodes
                for (int j = 0; j < component.Count; j++)
                {
                    nodes[index] = component[j];
                    index++;
                }

                // Clear the component again and set the right index of this border
                component.Clear();
                component.TrimExcess();
                borders[i] = new Tuple<int, int>(borders[i].Item1, index);
            }
            return borders;
        }

        /// <summary>
        /// Calculates the treedepth unsing an exact method
        /// </summary>
        /// <returns>The resulting tree</returns>
        public RecursiveTree<Node> GetBestTree()
        {
            // Uses a dictionary for already checked subsets for memoization
            Dictionary<string, RecursiveTree<Node>> checkedSubsets = new Dictionary<string, RecursiveTree<Node>>();
            int bestFoundSolution = allNodes.Length + 1;
            return RecGetBestTree(bestFoundSolution, allNodes.ToList(), new HashSet<Node>(), checkedSubsets);
        }

        /// <summary>
        /// Recursive method for calculating the best heuristic tree
        /// </summary>
        /// <param name="bestFoundSolution">The optimal solution thus far from this level of the tree, used for early stopping</param>
        /// <param name="nodes">A list of all nodes in this subtree</param>
        /// <param name="ancestors">The list of all ancestors of the current subtree</param>
        /// <param name="checkedSubsets">A dictionary with checked subsets, used for memoization</param>
        /// <returns>The optimal exact tree for the list of input nodes</returns>
        private RecursiveTree<Node> RecGetBestTree(int bestFoundSolution, List<Node> nodes, HashSet<Node> ancestors, Dictionary<string, RecursiveTree<Node>> checkedSubsets)
        {
            // If the currently best found solution is smaller than the list of ancestors here, we cannot possibly improve it. Thus, we return an empty tree
            if (bestFoundSolution < ancestors.Count + 1)
            {
                return null;
            }

            // Check if we have already computed a subtree for this set of nodes, if so, return that tree
            string asBits = NodeSubsetRepresentation(nodes);
            if (checkedSubsets.ContainsKey(asBits) && checkedSubsets[asBits] != null)
            {
                RecursiveTree<Node> computedTree = new RecursiveTree<Node>(checkedSubsets[asBits]);
                return computedTree;
            }

            // Sort the nodes on their remaining degree value (descending) and recursively compute the best trees
            HashSet<Node> nodesAsHash = new HashSet<Node>(nodes);
            nodes = nodes.OrderByDescending(n => n.RemainingDegree(nodesAsHash)).ToList();
            RecursiveTree<Node> bestTree = null;
            foreach (Node selectedNode in nodes)
            {
                RecursiveTree<Node> newTree = new RecursiveTree<Node>(selectedNode);
                HashSet<Node> beenList = new HashSet<Node>(ancestors) { selectedNode };
                bool broken = false;

                foreach (Node n in nodes)
                {
                    // Find the connected component this node is in
                    if (beenList.Contains(n)) continue;
                    List<Node> connectedNodes = DFS.All(n, (nn) => { return true; }, beenList);
                    HashSet<Node> newHash = new HashSet<Node>(ancestors) { selectedNode };

                    // Compute the best possible subtree for this connected component
                    RecursiveTree<Node> ChildTree = RecGetBestTree(bestFoundSolution, connectedNodes, newHash, checkedSubsets);
                    if (ChildTree == null)
                    {
                        // If the resulting tree is null, it is not a viable solution
                        broken = true;
                        break;
                    }
                    ChildTree.Parent = newTree;
                    newTree.AddChild(ChildTree);
                }

                // If we found a viable solution, update the best found solution thus far
                if (!broken)
                {
                    int newDepth = newTree.Depth;
                    if (newDepth + ancestors.Count < bestFoundSolution)
                    {
                        bestFoundSolution = newDepth + ancestors.Count;
                        bestTree = newTree;
                    }
                }
            }

            // Save the tree in the memoization dictionary and return it
            checkedSubsets[asBits] = bestTree;
            return bestTree;
        }

        /// <summary>
        /// Calculates an enconded string representation of a set of nodes
        /// </summary>
        /// <param name="nodes">The nodes to be represented as encoded string</param>
        /// <returns>An encoded string containing the "value" for this set of nodes</returns>
        private string NodeSubsetRepresentation(List<Node> nodes)
        {
            byte[] bytes = new byte[(int)Math.Ceiling(allNodes.Length * 0.125f)];
            for (int i = 0; i < nodes.Count; i++)
            {
                int bitIndex = nodes[i].Number - 1;
                int byteIndex = bitIndex / 8;
                byte bitInByteIndex = (byte)(bitIndex % 8);
                bytes[byteIndex] |= (byte)(1 << bitInByteIndex);
            }

            string encodedString = Convert.ToBase64String(bytes);
            return encodedString;
        }

        /// <summary>
        /// Creates a subtree in the form of a single line from a list of nodes
        /// </summary>
        /// <param name="nodes">The nodes to be in the subtree</param>
        /// <returns>The root of a subtree with the nodes in a single line</returns>
        private RecursiveTree<Node> CreateLine(List<Node> nodes)
        {
            RecursiveTree<Node> root = new RecursiveTree<Node>(nodes[0]);
            RecursiveTree<Node> parent = root;
            for (int i = 1; i < nodes.Count; i++)
            {
                RecursiveTree<Node> node = new RecursiveTree<Node>(nodes[i]) { Parent = parent };
                parent.AddChild(node);
                parent = node;
            }
            return root;
        }
    }
}
