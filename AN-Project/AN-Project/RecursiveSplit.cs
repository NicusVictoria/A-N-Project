using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Collections;

namespace AN_Project
{
    class RecursiveSplit
    {
        private readonly Node[] allNodes;

        public RecursiveSplit(Node[] allNodes)
        {
            this.allNodes = allNodes;
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
        private BigInteger NodeSubsetRepresentation(List<Node> nodes, int nodeNumber)
        {
            // TODO: nodenumber wordt nu niet gebruikt, is dat ook de bedoeling?

            byte[] bytes = new byte[(int)Math.Ceiling(allNodes.Length / 8f)];
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i == nodeNumber) continue;
                int bitIndex = nodes[i].Number - 1;
                int byteIndex = bitIndex / 8;
                byte bitInByteIndex = (byte)(bitIndex % 8);
                bytes[byteIndex] |= (byte)(1 << bitInByteIndex);
            }
            return new BigInteger(bytes);
        }

        /// <summary>
        /// Calculates the treedepth unsing an exact method
        /// </summary>
        /// <returns>The resulting tree</returns>
        public RecursiveTree<Node> GetBestTree()
        {
            int bestFoundSolution = allNodes.Length + 1;
            Dictionary<(BigInteger, int), RecursiveTree<Node>> checkedSubsets = new Dictionary<(BigInteger, int), RecursiveTree<Node>>();

            return RecGetBestTree(bestFoundSolution, allNodes.ToList(), new HashSet<Node>(), checkedSubsets);
        }

        private RecursiveTree<Node> RecGetBestTree(int bestFoundSolution, List<Node> Nodes, HashSet<Node> parents, Dictionary<(BigInteger, int), RecursiveTree<Node>> checkedSubsets)
        {
            // BESTFOUNDSOLUTION IS HOEVEEL ONDER DIT NIVEAU HET GELIJK ZOU ZIJN AAN DE MINIMAAL GEVONDEN DEPTH

            if (bestFoundSolution < 1/*Nodes.Min(n => n.RemainingDegree(nodesAsHash))*/) //TODO not entirely sure if correct
            {
                return null;
            }

            //bestFoundSolution = allNodes.Length+ 1; // TODO: AAAAAAAAH, remove

            HashSet<Node> nodesAsHash = new HashSet<Node>(Nodes);
            Nodes.OrderByDescending(n => n.RemainingDegree(nodesAsHash));
            RecursiveTree<Node> bestTree = null;
            foreach (Node selectedNode in Nodes)
            {
                BigInteger asBits = NodeSubsetRepresentation(Nodes, selectedNode.Number);
                (BigInteger, int) key = (asBits, selectedNode.Number);
                if (checkedSubsets.ContainsKey(key) && checkedSubsets[key] != null)
                {
                    RecursiveTree<Node> orphan = new RecursiveTree<Node>(checkedSubsets[key]);
                    return orphan;
                }

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

                    /*
                    if (newTree.Children.Count > 0 && connectedNodes.Count < newTree.Depth - 1)//TODO: MAY NOT WORK
                    {
                        RecursiveTree<Node> ChildTree = CreateLine(connectedNodes);
                        ChildTree.Parent = newTree;
                        newTree.AddChild(ChildTree);
                    }
                    else
                    {
                    */
                    RecursiveTree<Node> ChildTree = RecGetBestTree(bestFoundSolution, connectedNodes, newHash, checkedSubsets);
                        if (ChildTree == null)
                        {
                            broken = true;
                            break;
                        }
                        ChildTree.Parent = newTree;
                        newTree.AddChild(ChildTree);
                    //}
                }
                if (!broken)
                {
                    checkedSubsets[(asBits, selectedNode.Number)] = newTree;
                    int newDepth = newTree.Depth;
                    if (newDepth + parents.Count < bestFoundSolution)
                    {
                        bestFoundSolution = newDepth + parents.Count;
                        bestTree = newTree;
                    }
                }
            }
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