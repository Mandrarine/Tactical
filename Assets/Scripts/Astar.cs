using System.Collections.Generic;
using UnityEngine;

public static class Astar
{
    #region Structures

    public struct Node
    {
        public float Cost;
        public IntVector2 GridPos;
        public Vector3 WorldPos;
        public Enums.NavigationState NavigationState;
    }

    public struct IntVector2
    {
        public int X;
        public int Y;

        public IntVector2(int pCoordX, int pCoordY)
        {
            X = pCoordX;
            Y = pCoordY;
        }
    }

    #endregion

    #region Attributes

    public static Node[,] Nodes { get; set; }
    public static bool AllowDiagonals { get; set; }

    #endregion

    #region Logic

    public static List<Node> FindPath(Node p_start, Node p_end)
    {
        List<Node> l_closedSet = new List<Node>();

        List<Node> l_openSet = new List<Node>();
        l_openSet.Add(p_start);

        Dictionary<Node, Node> l_fromList = new Dictionary<Node, Node>();

        Dictionary<Node, float> g_scores = new Dictionary<Node, float>();
        g_scores[p_start] = 0.0f;

        Dictionary<Node, float> h_scores = new Dictionary<Node, float>();
        h_scores[p_start] = GetCost(p_start, p_end);

        Dictionary<Node, float> f_scores = new Dictionary<Node, float>();
        f_scores[p_start] = h_scores[p_start];

        while (l_openSet.Count != 0)
        {
            Node l_currentNode = LowestScore(l_openSet, f_scores);

            if (l_currentNode.Equals(p_end))
            {
                List<Node> l_result = new List<Node>();
                ReconstructPath(l_fromList, l_currentNode, ref l_result);
                return l_result;
            }

            l_openSet.Remove(l_currentNode);
            l_closedSet.Add(l_currentNode);

            List<Node> l_neighbours = GetNodeNeighbours(l_currentNode);

            foreach (Node neighbour in l_neighbours)
            {
                if (neighbour.NavigationState == Enums.NavigationState.Unwalkable || l_closedSet.Contains(neighbour))
                {
                    continue;
                }

                float l_tentativeGScore = g_scores[l_currentNode] + GetCost(l_currentNode, neighbour);
                bool l_tentativeIsBetter = false;
                
                if (!l_openSet.Contains(neighbour))
                {
                    l_openSet.Add(neighbour);
                    l_tentativeIsBetter = true;
                }
                else if (l_tentativeGScore < g_scores[neighbour])
                {
                    l_tentativeIsBetter = true;
                }

                if (l_tentativeIsBetter)
                {
                    l_fromList[neighbour] = l_currentNode;
                    g_scores[neighbour] = l_tentativeGScore;
                    h_scores[neighbour] = GetCost(neighbour, p_end);
                    f_scores[neighbour] = g_scores[neighbour] + h_scores[neighbour];
                }
            }
        }

        return new List<Node>();
    }

    public static List<Node> GetNodeNeighbours(Node pNode)
    {
        List<Node> lNeighboursList = new List<Node>();

        int X = pNode.GridPos.X;
        int Y = pNode.GridPos.Y;

        Node l_node;

        // Top
        if (GetNodeAtCoords(X, Y - 1, out l_node))
            lNeighboursList.Add(l_node);
        // Down
        if (GetNodeAtCoords(X, Y + 1, out l_node))
            lNeighboursList.Add(l_node);
        // Left
        if (GetNodeAtCoords(X - 1, Y, out l_node))
            lNeighboursList.Add(l_node);
        // Right
        if (GetNodeAtCoords(X + 1, Y, out l_node))
            lNeighboursList.Add(l_node);

        // Diagonals
        if (AllowDiagonals)
        {
            if (GetNodeAtCoords(X - 1, Y - 1, out l_node))
                lNeighboursList.Add(l_node);
            if (GetNodeAtCoords(X + 1, Y - 1, out l_node))
                lNeighboursList.Add(l_node);
            if (GetNodeAtCoords(X - 1, Y + 1, out l_node))
                lNeighboursList.Add(l_node);
            if (GetNodeAtCoords(X + 1, Y + 1, out l_node))
                lNeighboursList.Add(l_node);
        }

        return lNeighboursList;
    }
    
    private static bool GetNodeAtCoords(int pCoordX, int pCoordY, out Node pNode)
    {
        if (CoordsAreValid(pCoordX, pCoordY))
        {
            //pNode = Nodes[pCoordX, pCoordY];
            pNode = Nodes[pCoordY, pCoordX];
            return true;
        }

        pNode = default;
        return false;
    }

    private static bool CoordsAreValid(int p_coordX, int p_coordY)
    {
        return (p_coordY >= 0 && p_coordY < Nodes.GetLength(0)) && (p_coordX >= 0 && p_coordX < Nodes.GetLength(1));
    }

    private static float GetCost(Node pFrom, Node pTo)
    {
        float l_rawDistance = Vector3.Distance(pFrom.WorldPos, pTo.WorldPos);
        float l_distance = l_rawDistance + pTo.Cost;
        return l_distance;
    }

    private static void ReconstructPath(Dictionary<Node, Node> pFromList, Node pCurrentNode, ref List<Node> pResult)
    {
        if (pFromList.ContainsKey(pCurrentNode))
        {
            pResult.Add(pCurrentNode);
            ReconstructPath(pFromList, pFromList[pCurrentNode], ref pResult);
        }

        // This is the start node
        pResult.Add(pCurrentNode);
    }

    private static Node LowestScore(List<Node> p_nodes, Dictionary<Node, float> p_scores)
    {
        int l_index = 0;
        float l_lowestScore = float.MaxValue;

        for (int i = 0; i < p_nodes.Count; i++)
        {
            if (p_scores[p_nodes[i]] > l_lowestScore)
            {
                continue;
            }
            l_index = i;
            l_lowestScore = p_scores[p_nodes[i]];
        }

        return p_nodes[l_index];
    }

    #endregion
}