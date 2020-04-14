using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections;
using System.Diagnostics;

namespace AN_Project
{
    class RecursiveSplit
    {
        private readonly Node[] allNodes;
        
        public RecursiveSplit(Node[] allNodes)
        {
            this.allNodes = allNodes;
        }

        public RecursiveTree<Node> GetFastHeuristicTree(Stopwatch timer, bool fast = true)
        {
            Node[] nodes = (Node[])allNodes.Clone();
            return RecGetFastHeuristicTree(nodes, new HashSet<Node>(), 0, allNodes.Length, timer, fast);
        }

        private RecursiveTree<Node> RecGetFastHeuristicTree(Node[] nodes, HashSet<Node> ancestors, int left, int right, Stopwatch timer, bool fast) // Left inclusive, right exclusive
        {
            if (timer.Elapsed.TotalSeconds >= Program.MAXTIMESECONDS)
            {
                return CreateLine(nodes.Skip(left).Take(right - left).ToList());
            }

            IEnumerable<Node> subArray = nodes.Skip(left).Take(right - left);
            Node selectedNode = null;
            if (fast) selectedNode = subArray.Max();
            else
            {
                int maxDegree = -1;
                HashSet<Node> nodesAsHash = new HashSet<Node>(subArray);
                foreach (Node n in subArray) //TODO implement heuristic instead of this (incorporate this in heuristic)
                {
                    int nRemainingDegree = n.RemainingDegree(nodesAsHash);
                    if (nRemainingDegree > maxDegree)
                    {
                        maxDegree = nRemainingDegree;
                        selectedNode = n;
                    }
                }
            }
            RecursiveTree<Node> newTree = new RecursiveTree<Node>(selectedNode);
            if (left - right == 1) return newTree;
            ancestors.Add(selectedNode);
            HashSet<Node> beenList = new HashSet<Node>(ancestors);
            List<List<Node>> connectedComponents = new List<List<Node>>();
            for (int i = left; i < right; i++)
            {
                if (beenList.Contains(nodes[i])) continue;
                List<Node> connectedNodes = DFS.All(nodes[i], (nn) => { return true; }, beenList);
                connectedComponents.Add(connectedNodes);
            }

            (int left, int right)[] borders = new (int, int)[connectedComponents.Count];
            int index = left;
            for (int i = 0; i < connectedComponents.Count; i++)
            {
                borders[i].left = index;
                List<Node> component = connectedComponents[i];
                for (int j = 0; j < component.Count; j++)
                {
                    nodes[index] = component[j];
                    index++;
                }
                borders[i].right = index;
            }

            for (int i = 0; i < borders.Length; i++)
            {
                RecursiveTree<Node> ChildTree = RecGetFastHeuristicTree(nodes, ancestors, borders[i].left, borders[i].right, timer, fast);
                ChildTree.Parent = newTree;
                newTree.AddChild(ChildTree);
            }
            ancestors.Remove(selectedNode);
            return newTree;
        }


        /// <summary>
        /// Calculates the treedepth using a heuristic
        /// </summary>
        /// <returns>The resulting tree</returns>
        public RecursiveTree<Node> GetHeuristicTree()
        {
            return RecGetHeuristicTree(allNodes.ToList(), new HashSet<Node>());
        }

        private RecursiveTree<Node> RecGetHeuristicTree(List<Node> Nodes, HashSet<Node> ancestors)
        {
            Node selectedNode = null;
            int maxDegree = 0;
            HashSet<Node> nodesAsHash = new HashSet<Node>(Nodes);
            foreach (Node n in Nodes) //TODO implement heuristic instead of this (incorporate this in heuristic)
            {
                int nRemainingDegree = n.RemainingDegree(nodesAsHash);
                if (nRemainingDegree > maxDegree)
                {
                    maxDegree = nRemainingDegree;
                    selectedNode = n;
                }
            }
            //Node selectedNode = Nodes.Max(); 
            RecursiveTree<Node> newTree = new RecursiveTree<Node>(selectedNode);
            ancestors.Add(selectedNode);
            HashSet<Node> beenList = new HashSet<Node>(ancestors);
            foreach (Node n in Nodes)
            {
                if (beenList.Contains(n)) continue;
                List<Node> connectedNodes = DFS.All(n, (nn) => { return true; }, beenList);
                RecursiveTree<Node> ChildTree = RecGetHeuristicTree(connectedNodes, new HashSet<Node>(ancestors));
                ChildTree.Parent = newTree;
                newTree.AddChild(ChildTree);
            }
            return newTree;
        }

        /// <summary>
        /// Calculates a BigInteger representation of a set of nodes. This corresponds to a bitstring with 0 if the node is not present here, and 1 if it is present
        /// </summary>
        /// <param name="nodes">The nodes to be represented as bitstring</param>
        /// <param name="nodeNumber">The node that is split on</param>
        /// <returns>A BigInteger containing the "value" for this set of nodes</returns>
        private string NodeSubsetRepresentation(List<Node> nodes)
        {
            byte[] bytes = new byte[(int)Math.Ceiling(allNodes.Length / 8f)];
            for (int i = 0; i < nodes.Count; i++)
            {
                //if (i == nodeNumber) continue;
                int bitIndex = nodes[i].Number - 1;
                int byteIndex = bitIndex / 8;
                byte bitInByteIndex = (byte)(bitIndex % 8);
                bytes[byteIndex] |= (byte)(1 << bitInByteIndex);
            }

            string encodedString = Convert.ToBase64String(bytes);
            return encodedString;
        }

        /// <summary>
        /// Calculates the treedepth unsing an exact method
        /// </summary>
        /// <returns>The resulting tree</returns>
        public RecursiveTree<Node> GetBestTree()
        {
            int bestFoundSolution = allNodes.Length + 1;
            Dictionary<string, RecursiveTree<Node>> checkedSubsets = new Dictionary<string, RecursiveTree<Node>>();

            return RecGetBestTree(bestFoundSolution, allNodes.ToList(), new HashSet<Node>(), checkedSubsets);
        }

        private RecursiveTree<Node> RecGetBestTree(int bestFoundSolution, List<Node> Nodes, HashSet<Node> parents, Dictionary<string, RecursiveTree<Node>> checkedSubsets)
        {
            // BESTFOUNDSOLUTION IS HOEVEEL ONDER DIT NIVEAU HET GELIJK ZOU ZIJN AAN DE MINIMAAL GEVONDEN DEPTH

            if (bestFoundSolution < parents.Count + 1/*Nodes.Min(n => n.RemainingDegree(nodesAsHash))*/) //TODO not entirely sure if correct
            {
                return null;
            }

            string asBits = NodeSubsetRepresentation(Nodes);
            if (checkedSubsets.ContainsKey(asBits) && checkedSubsets[asBits] != null)
            {
                RecursiveTree<Node> orphan = new RecursiveTree<Node>(checkedSubsets[asBits]);
                return orphan;
            }

            //bestFoundSolution = allNodes.Length+ 1; // TODO: AAAAAAAAH, remove

            HashSet<Node> nodesAsHash = new HashSet<Node>(Nodes);
            Nodes = Nodes.OrderByDescending(n => n.RemainingDegree(nodesAsHash)).ToList();
            RecursiveTree<Node> bestTree = null;
            foreach (Node selectedNode in Nodes)
            {
                RecursiveTree<Node> newTree = new RecursiveTree<Node>(selectedNode);


                HashSet<Node> beenList = new HashSet<Node>(parents)
                {
                    selectedNode
                };
                bool broken = false;

                foreach (Node n in Nodes)
                {
                    if (beenList.Contains(n)) continue;
                    List<Node> connectedNodes = DFS.All(n, (nn) => { return true; }, beenList);
                    HashSet<Node> newHash = new HashSet<Node>(parents)
                    {
                        selectedNode
                    };

                    RecursiveTree<Node> ChildTree = RecGetBestTree(bestFoundSolution, connectedNodes, newHash, checkedSubsets);
                    if (ChildTree == null)
                    {
                        broken = true;
                        break;
                    }
                    ChildTree.Parent = newTree;
                    newTree.AddChild(ChildTree);
                }
                if (!broken)
                {
                    int newDepth = newTree.Depth;
                    if (newDepth + parents.Count < bestFoundSolution)
                    {
                        bestFoundSolution = newDepth + parents.Count;
                        bestTree = newTree;
                    }
                }
            }
            checkedSubsets[asBits] = bestTree;
            return bestTree;
        }

        /// <summary>
        /// Creates a subtree in the form of a single line from a list of nodes
        /// </summary>
        /// <param name="nodes">The nodes to be in the subtree</param>
        /// <returns>A subtree with the nodes in a single line</returns>
        private RecursiveTree<Node> CreateLine(List<Node> nodes)
        {
            RecursiveTree<Node> root = new RecursiveTree<Node>(nodes[0]);
            RecursiveTree<Node> parent = root;
            for (int i = 1; i < nodes.Count; i++)
            {
                RecursiveTree<Node> node = new RecursiveTree<Node>(nodes[i])
                {
                    Parent = parent
                };
                parent.AddChild(node);
                parent = node;
            }
            return root;
        }
    }
}