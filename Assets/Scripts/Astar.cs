using System.Collections.Generic;
using UnityEngine;

public static class Astar
{
	#region Structures

	public class Node
	{
		public IntVector2 GridPos;
		public Vector3 WorldPos;
		public Enums.NavState NavState;
	}

	public struct IntVector2
	{
		public readonly int X;
		public readonly int Y;

		public IntVector2(int coordX, int coordY)
		{
			X = coordX;
			Y = coordY;
		}
	}

	/*
	private static bool Equals(this Node nodeA, Node nodeB)
	{
		return (nodeA.GridPos.X == nodeB.GridPos.X) && (nodeB.GridPos.Y == nodeB.GridPos.Y);
	}
	*/

	#endregion

	#region Fields

	public static Node[,] Nodes { get; set; }
	//public static bool AllowDiagonals { get; set; }

	#endregion

	#region Methods

	public static List<Node> FindPath(Node startNode, Node endNode, float jumpHeight = 0)
	{
		if (startNode == null || endNode == null)
			return null;

		var closedSet = new List<Node>();
		var openSet = new List<Node>();
		var fromList = new Dictionary<Node, Node>();
		var gScores = new Dictionary<Node, float>();
		var hScores = new Dictionary<Node, float>();
		var fScores = new Dictionary<Node, float>();

		openSet.Add(startNode);
		gScores[startNode] = 0.0f;
		hScores[startNode] = GetCost(startNode, endNode);
		fScores[startNode] = hScores[startNode];

		while (openSet.Count != 0)
		{
			Node currentNode = LowestScore(openSet, fScores);

			if (currentNode.Equals(endNode))
			{
				List<Node> finalPath = new List<Node>();
				ReconstructPath(fromList, currentNode, ref finalPath);
				return finalPath;
			}

			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			List<Node> neighbours = GetNeighbourNodes(currentNode);

			foreach (Node neighbour in neighbours)
			{
				if (neighbour == null)
					continue;

				//if (neighbour.NavigationState == Enums.NavigationState.Unwalkable || closedSet.Contains(neighbour))

				if (closedSet.Contains(neighbour))
					continue;

				float verticalDifference = Mathf.Abs(currentNode.WorldPos.y - neighbour.WorldPos.y);
				if (verticalDifference > jumpHeight)
					continue;

				float tentativeGScore = gScores[currentNode] + GetCost(currentNode, neighbour);
				bool tentativeIsBetter = false;

				if (!openSet.Contains(neighbour))
				{
					openSet.Add(neighbour);
					tentativeIsBetter = true;
				}
				else if (tentativeGScore < gScores[neighbour])
					tentativeIsBetter = true;

				if (!tentativeIsBetter)
					continue;

				fromList[neighbour] = currentNode;
				gScores[neighbour] = tentativeGScore;
				hScores[neighbour] = GetCost(neighbour, endNode);
				fScores[neighbour] = gScores[neighbour] + hScores[neighbour];
			}
		}

		return null;
	}

	private static List<Node> GetNeighbourNodes(Node currentNode)
	{
		var neighbours = new List<Node>();

		var coordX = currentNode.GridPos.X;
		var coordY = currentNode.GridPos.Y;
		Node node;

		// 🡩
		if (TryGetNodeAtCoords(coordX, coordY + 1, out node))
			neighbours.Add(node);
		// 🡪
		if (TryGetNodeAtCoords(coordX + 1, coordY, out node))
			neighbours.Add(node);
		// 🡫
		if (TryGetNodeAtCoords(coordX, coordY - 1, out node))
			neighbours.Add(node);
		// 🡨
		if (TryGetNodeAtCoords(coordX - 1, coordY, out node))
			neighbours.Add(node);

		/*
		// Diagonals
		if (!AllowDiagonals)
			return neighbours;

		// 🡭
		if (TryGetNodeAtCoords(coordX + 1, coordY + 1, out node))
			neighbours.Add(node);
		// 🡮
		if (TryGetNodeAtCoords(coordX + 1, coordY - 1, out node))
			neighbours.Add(node);
		// 🡯
		if (TryGetNodeAtCoords(coordX - 1, coordY - 1, out node))
			neighbours.Add(node);
		// 🡬
		if (TryGetNodeAtCoords(coordX - 1, coordY + 1, out node))
			neighbours.Add(node);
		*/

		return neighbours;
	}

	public static List<Node> FindPathsInRange(Node originNode, int range, float jumpHeight)
	{
		List<Node> nodes = new List<Node>();

		for (var y = 0; y < Nodes.GetLength(0); y++)
		{
			for (var x = 0; x < Nodes.GetLength(1); x++)
			{
				Node destinationNode = Nodes[y, x];

				var path = FindPath(originNode, destinationNode, jumpHeight);
				if (path != null && path.Count <= range + 1)
					nodes.Add(destinationNode);
			}
		}

		return nodes;
	}

	private static bool TryGetNodeAtCoords(int coordX, int coordY, out Node outputNode)
	{
		if (CoordsAreValid(coordX, coordY))
		{
			outputNode = Nodes[coordY, coordX];
			return true;
		}

		outputNode = default;
		return false;
	}

	private static bool CoordsAreValid(int coordX, int coordY)
	{
		return ((coordY >= 0) && (coordY < Nodes.GetLength(0))) && ((coordX >= 0) && (coordX < Nodes.GetLength(1)));
	}

	private static float GetCost(Node fromNode, Node toNode)
	{
		return Vector3.Distance(fromNode.WorldPos, toNode.WorldPos);
	}

	private static void ReconstructPath(Dictionary<Node, Node> fromList, Node currentNode, ref List<Node> finalPath)
	{
		if (fromList.ContainsKey(currentNode))
		{
			finalPath.Add(currentNode);
			ReconstructPath(fromList, fromList[currentNode], ref finalPath);
		}
		else
		{
			finalPath.Add(currentNode);
			finalPath.Reverse();
		}
	}

	private static Node LowestScore(List<Node> nodes, Dictionary<Node, float> scores)
	{
		var index = 0;
		var lowestScore = float.MaxValue;

		for (var i = 0; i < nodes.Count; i++)
		{
			if (scores[nodes[i]] > lowestScore)
				continue;

			index = i;
			lowestScore = scores[nodes[i]];
		}

		return nodes[index];
	}

	#endregion
}