using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public GameObject start;
	public GameObject end;

	private Tile _startTile;
	private Tile _endTile;
	private List<Astar.Node> _path;

	#region Updates

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			_startTile = GridController.Instance.Tiles[(int)start.transform.position.z, (int)start.transform.position.x];
			_endTile = GridController.Instance.Tiles[(int)end.transform.position.z, (int)end.transform.position.x];

			/*
			for (var y = 0; y < GridController.Instance.Tiles.GetLength(0); y++)
			{
				string line = "";
				for (var x = 0; x < GridController.Instance.Tiles.GetLength(1); x++)
				{
					Tile tile = GridController.Instance.Tiles[y, x];
					string content = "";
					//Debug.Log($"[{x},{y}]");
					//line += $"[{x},{y}]";

					if (tile == null)
						content = "[X]";
					else if (tile == _startTile)
						content = "[S]";
					else if (tile == _endTile)
						content = "[E]";
					else
						content = "[ ]";
					line += content;
				}
				Debug.Log(line);
			}
			*/

			_path = AstarContext.Instance.FindPath(_startTile, _endTile);
			Debug.Log($"Path length : {_path.Count}");

			//_path = Astar.FindPath(_startTile.Node, _endTile.Node);

			/*
			if (_path != null)
			{
				foreach (var item in _path)
				{
					Debug.Log("[" + item.GridPos.X + ", " + item.GridPos.Y + "]");
				}
			}
			*/
		}
	}

	#endregion

	#region Logic

	#endregion

	#region Gizmos

	private void OnDrawGizmos()
	{
		if (_path == null)
			return;

		foreach (Astar.Node node in _path)
		{
			DrawGizmoSphere(node, 0.2f, Color.cyan);
		}
	}

	private void DrawGizmoSphere(Astar.Node node, float radius, Color color)
	{
		Gizmos.color = color;
		Gizmos.DrawSphere(node.WorldPos, radius);
	}

	#endregion
}
