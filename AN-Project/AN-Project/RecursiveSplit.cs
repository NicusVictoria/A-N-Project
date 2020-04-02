using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AN_Project
{
    class RecursiveSplit
    {
        Node[] allNodes;
        public RecursiveSplit(Node[] allNodes)
        {
            this.allNodes = allNodes;
            
            
        }

        public RecursiveTree<Node> getHeuristicTree()
        {
            return RecFun(allNodes.ToList(), new HashSet<Node>());
            RecursiveTree<Node> RecFun(List<Node> Nodes, HashSet<Node> parents)
            {
                Node selectedNode = null;
                int maxDegree = 0;
                HashSet<Node> nodesAsHash = new HashSet<Node>();
                foreach(Node n in Nodes)
                {
                    int nRemainingDegree = n.RemainingDegree(nodesAsHash);
                    if (nRemainingDegree > maxDegree)
                    {
                        maxDegree = nRemainingDegree;
                        selectedNode = n;
                    }
                }
                RecursiveTree<Node> newTree = new RecursiveTree<Node>(selectedNode);
                parents.Add(selectedNode);
                HashSet<Node> beenList = new HashSet<Node>(parents);
                foreach (Node n in Nodes)
                {
                    if (beenList.Contains(n)) continue;
                    List<Node> connectedNodes = DFS.All(n, (n) => { return true; }, beenList);
                    RecursiveTree<Node> ChildTree = RecFun(connectedNodes, new HashSet<Node>(parents));
                    ChildTree.Parent = newTree;
                    newTree.Children.Add(ChildTree);
                }
                return newTree;
            }
        }

        public RecursiveTree<Node> getBestTree()
        {
            return RecFun(allNodes.ToList(), new HashSet<Node>());

            RecursiveTree<Node> RecFun(List<Node> Nodes, HashSet<Node> parents)
            {
                HashSet<Node> nodesAsHash = new HashSet<Node>();
                Nodes.OrderBy(n => n.RemainingDegree(nodesAsHash));
                int bestDepth = allNodes.Length;
                RecursiveTree<Node> bestTree = null;
                foreach (Node selectedNode in Nodes)
                {
                    RecursiveTree<Node> newTree = new RecursiveTree<Node>(selectedNode);
                    HashSet<Node> beenList = new HashSet<Node>(parents);
                    beenList.Add(selectedNode);
                    foreach (Node n in Nodes)
                    {
                        if (beenList.Contains(n)) continue;
                        List<Node> connectedNodes = DFS.All(n, (n) => { return true; }, beenList);
                        HashSet<Node> newHash = new HashSet<Node>(parents);
                        newHash.Add(selectedNode);
                        RecursiveTree<Node> ChildTree = RecFun(connectedNodes, newHash);
                        ChildTree.Parent = newTree;
                        newTree.Children.Add(ChildTree);
                    }
                    int newDepth = newTree.Depth;
                    if (newDepth < bestDepth)
                    {
                        bestDepth = newDepth;
                        bestTree = newTree;
                    }
                }
                return bestTree;
            }
        }
    }

    static class RecursiveTreePrinter
    {
        public static string PrintTree(RecursiveTree<Node> tree)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(tree.Depth);
            stringBuilder.Append("\n");

            stringBuilder.Append("0");

            foreach (RecursiveTree <Node> subTree in tree.Children)
            {
                PrintTree(subTree, stringBuilder);
            }

            return stringBuilder.ToString();
        }

        static void PrintTree(RecursiveTree<Node> tree, StringBuilder stringBuilder)
        {
            stringBuilder.Append(tree.Parent.value.Number);
            stringBuilder.Append("\n");

            foreach (RecursiveTree<Node> subTree in tree.Children)
            {
                PrintTree(subTree, stringBuilder);
            }
        }
    }

    class RecursiveTree<V> : ITree<RecursiveTree<V>>, ITreeNode<RecursiveTree<V>>
    {
        public V value { get; set; }

        public RecursiveTree(V n)
        {
            value = n;
            Children = new List<RecursiveTree<V>>();
        }
        

        RecursiveTree<V> ITree<RecursiveTree<V>>.Root => this;

        public int Depth => (Children.Max(c => (int?)c.Depth) ?? 0) + 1;

        public int NumberOfNodes => Children.Sum(c => c.NumberOfNodes + 1);

        public List<RecursiveTree<V>> Children { get; set; }

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

        public int RemainingDegree(HashSet<Node> subGraph) => ConnectedNodes.Count(n => subGraph.Contains(n));

        public double Heuristic { get { return Degree; } } //TODO improve this

        int IComparable<Node>.CompareTo(Node other)
        {
            return Heuristic.CompareTo(other.Heuristic); 
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
                    stack.Push(n);
                }
            }
            return retList;
        }
    }
}
