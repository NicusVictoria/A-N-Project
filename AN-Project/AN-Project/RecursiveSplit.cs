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
        Node[] allNodes;

        public RecursiveSplit(Node[] allNodes)
        {
            this.allNodes = allNodes;
        }

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

        private BigInteger NodeSubset(List<Node> nodes, int nodeNumber)
        {
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

        public RecursiveTree<Node> GetBestTree()
        {
            int bestFoundSolution = allNodes.Length + 1;
            Dictionary<(BigInteger, int), RecursiveTree<Node>> checkedSubsets = new Dictionary<(BigInteger, int), RecursiveTree<Node>>();

            return RecGetBestTree(ref bestFoundSolution, allNodes.ToList(), new HashSet<Node>(), checkedSubsets);
        }

        private RecursiveTree<Node> RecGetBestTree(ref int bestFoundSolution, List<Node> Nodes, HashSet<Node> parents, Dictionary<(BigInteger, int), RecursiveTree<Node>> checkedSubsets)
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
                BigInteger asBits = NodeSubset(Nodes, selectedNode.Number);
                (BigInteger, int) key = (asBits, selectedNode.Number);
                if (checkedSubsets.ContainsKey(key) && checkedSubsets[key] != null)
                {
                    RecursiveTree<Node> orphan = new RecursiveTree<Node>(checkedSubsets[key]);
                    return orphan;
                }

                RecursiveTree<Node> newTree = new RecursiveTree<Node>(selectedNode);
                HashSet<Node> beenList = new HashSet<Node>(parents);
                beenList.Add(selectedNode);
                bool broken = false;

                int passDownWards = bestFoundSolution - 1;
                int maxChildDepth = 0;

                foreach (Node n in Nodes)
                {
                    if (beenList.Contains(n)) continue;
                    int maxChildHeight;
                    List<Node> connectedNodes = DFS.All(n, (nn) => { return true; }, beenList);
                    HashSet<Node> newHash = new HashSet<Node>(parents);
                    newHash.Add(selectedNode);

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
                        int singleDownwardPass = passDownWards;
                        RecursiveTree<Node> ChildTree = RecGetBestTree(ref singleDownwardPass, connectedNodes, newHash, checkedSubsets);
                        maxChildDepth = Math.Max(singleDownwardPass, maxChildDepth);
                        if (ChildTree == null)
                        {
                            broken = true;
                            break;
                        }
                        ChildTree.Parent = newTree;
                        newTree.AddChild(ChildTree);
                    //}
                }
                bestFoundSolution = Math.Min(passDownWards, maxChildDepth);
                if (!broken)
                {
                    //checkedSubsets[(asBits, selectedNode.Number)] = newTree;
                    int newDepth = newTree.Depth;
                    if (newDepth + parents.Count < bestFoundSolution)
                    {
                        bestFoundSolution = newDepth + parents.Count;
                        bestTree = newTree;
                    }
                }
                bestFoundSolution++;
            }

            //checkedSubsets[asBits] = bestTree;
            return bestTree;
        }

        private RecursiveTree<Node> CreateLine(List<Node> nodes)
        {
            RecursiveTree<Node> root = new RecursiveTree<Node>(nodes[0]);
            RecursiveTree<Node> parent = root;
            for (int i = 1; i < nodes.Count; i++)
            {
                RecursiveTree<Node> node = new RecursiveTree<Node>(nodes[i]);
                node.Parent = parent;
                parent.AddChild(node);
                parent = node;
            }
            return root;
        }
    }

    static class RecursiveTreePrinter
    {
        public static string PrintTree(RecursiveTree<Node> tree)
        {
            int[] nodeArray = new int[tree.NumberOfNodes];

            nodeArray[tree.value.Number - 1] = 0;

            foreach (RecursiveTree <Node> subTree in tree.Children)
            {
                PrintTree(subTree, nodeArray);
            }


            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(tree.Depth);
            stringBuilder.Append("\n");

            for (int i = 0; i < nodeArray.Length; i++)
            {
                stringBuilder.Append(nodeArray[i]);
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        static void PrintTree(RecursiveTree<Node> tree, int[] nodeArray)
        {
            nodeArray[tree.value.Number - 1] = tree.Parent.value.Number;

            foreach (RecursiveTree<Node> subTree in tree.Children)
            {
                PrintTree(subTree, nodeArray);
            }
        }
    }

    class RecursiveTree<V> : ITree<RecursiveTree<V>>, ITreeNode<RecursiveTree<V>>
    {
        public V value { get; set; }

        public RecursiveTree(V n)
        {
            value = n;
            ChildrenList = new List<RecursiveTree<V>>();
        }

        public RecursiveTree(RecursiveTree<V> original)
        {
            value = original.value;
            ChildrenList = original.ChildrenList;
        }



        int depth;
        bool depthCalculated;

        RecursiveTree<V> ITree<RecursiveTree<V>>.Root => this;

        public int Depth
        {
            get
            {
                if (!depthCalculated)
                {
                    depth = (Children.Max(c => (int?)c.Depth) ?? 0) + 1;
                    depthCalculated = true;
                }
                return depth;
            }
        }

        public void RecursivelyUpdateDepth()
        {
            depthCalculated = false;
            if (Parent != null)
            {
                Parent.RecursivelyUpdateDepth();
            }
        }

            

        public int NumberOfNodes => Children.Sum(c => c.NumberOfNodes) + 1;

        public void AddChild(RecursiveTree<V> child)
        {
            ChildrenList.Add(child);
            RecursivelyUpdateDepth();
        }

        public void AddChildren(List<RecursiveTree<V>> children)
        {
            ChildrenList.AddRange(children);
            RecursivelyUpdateDepth();
        }

        private List<RecursiveTree<V>> ChildrenList { get; set; }

        public ReadOnlyCollection<RecursiveTree<V>> Children
        {
            get
            {
                return new ReadOnlyCollection<RecursiveTree<V>>(ChildrenList);
            }
        }

            

        public RecursiveTree<V> Parent { get; set; }

    }




    public interface INode<N> where N : INode<N>
    {
        /// <summary>
        /// All nodes that are connected to this node
        /// </summary>
        List<N> ConnectedNodes { get; }
    }

    public class Node : INode<Node>, IComparable<Node>
    {
        public Node(int number)
        {
            this.Number = number;
        }

        public int Number { get; private set; }

        public List<Node> ConnectedNodes { get; set; }

        public int Degree { get { return ConnectedNodes.Count; } }

        public int RemainingDegree(HashSet<Node> subGraph) => ConnectedNodes.Select(n => !subGraph.Contains(n)).Count();

        public double Heuristic { get { return Degree; } } //TODO improve this

        int IComparable<Node>.CompareTo(Node other)
        {
            return Heuristic.CompareTo(other.Heuristic); 
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }

    public static class DFS
    {
        /// <summary>
        /// Look for nodes that fulfill the criteria, stopping when you have found one. O(n).
        /// </summary>
        /// <param name="startNode">the origin from which will be searched</param>
        /// <param name="targetSelection">the function that will determine whether a node gets returned; 
        /// will be returned when the function returns true, not returned when the function returns false</param>
        public static N First<N>(N startNode, Func<N, bool> targetSelection) where N : INode<N>
        {
            Stack<N> stack = new Stack<N>();
            HashSet<N> beenList = new HashSet<N>();
            stack.Push(startNode);
            while (stack.Count != 0)
            {
                N node = stack.Pop();
                beenList.Add(node);
                if (targetSelection(node)) return node;
                foreach (N n in node.ConnectedNodes)
                {
                    stack.Push(n);
                }
            }
            throw new Exception("there were no nodes that matched the criteria");
        }

        /// <summary>
        /// Look for nodes that fulfill the criteria, stopping when the entire graph has been explored. O(n) with n being the amount of nodes reachable from the startNode.
        /// </summary>
        /// <param name="startNode">the origin from which will be searched</param>
        /// <param name="targetSelection">the function that will determine whether a node gets returned; 
        /// will be returned when the function returns true, not returned when the function returns false</param>
        public static List<N> All<N>(N startNode, Func<N, bool> targetSelection, HashSet<N> beenList = null) where N : INode<N>
        {
            List<N> retList = new List<N>();
            Stack<N> stack = new Stack<N>();
            if(beenList == null) beenList = new HashSet<N>();
            stack.Push(startNode);
            while (stack.Count != 0)
            {
                N node = stack.Pop();
                if (beenList.Contains(node)) continue;
                beenList.Add(node);
                if (targetSelection(node)) retList.Add(node);
                foreach (N n in node.ConnectedNodes)
                {
                    //if (!beenList.Contains(n))
                    //{
                    //    beenList.Add(n);
                    //}
                    stack.Push(n);
                }
            }
            return retList;
        }

        /*
        public static List<Node> ArticulationPoints(int numberOfNodes, Node node, HashSet<Node> beenList = null)
        {
            List<Node> retList = new List<Node>();
            int time = 0;
            int[] discoveryTime = new int[numberOfNodes];
            int[] low = new int[numberOfNodes];

            return RecArticulationPoints(discoveryTime, low, time, node, null, beenList);
        }

        private static List<Node> RecArticulationPoints(int[] discoveryTime, int[] low, int time, Node node, Node parent, HashSet<Node> beenList = null)
        {
            beenList.Add(node);
            discoveryTime[node.Number] = time + 1;
            low[node.Number] = time + 1;

            int childNumber = 0;
            foreach (Node neighbour in node.ConnectedNodes)
            {
                if (neighbour == parent) continue;

                if (!beenList.Contains(neighbour))
                {
                    childNumber++;
                    time++;

                    List<Node> resultList = RecArticulationPoints(discoveryTime, low, time, neighbour, node, beenList);

                    low[neighbour.Number] = Math.Min(low[neighbour.Number], low[node.Number]);
                    if (parent == null && childNumber > 1)
                    {
                        resultList.Add(neighbour);
                    }
                    else if (parent != null && low[node.Number] >= discoveryTime[neighbour.Number])
                    {
                        resultList.Add(neighbour);
                    }
                }
                else
                {
                    low[neighbour.Number] = Math.Min(low[neighbour.Number], discoveryTime[node.Number]);
                }
            }
        }
        */
    }
}
