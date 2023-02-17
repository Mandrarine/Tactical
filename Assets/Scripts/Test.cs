using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	#region Fields

	public GameObject end;
	public Unit unit;

	private Tile _endTile;
	private List<Astar.Node> _path;
	private List<Astar.Node> _tilesInRange;

	#endregion

	#region Unity

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			_endTile = GridController.Instance.Tiles[(int)end.transform.position.z, (int)end.transform.position.x];

			Tile tile = GridController.Instance.CurrentTile;
			if (tile.Unit != null)
			{
				_path = Astar.FindPath(tile.Node, _endTile.Node, tile.Unit.jumpHeight);

				if (_path != null)
					unit.MoveUnitAlongPath(_path);
			}
		}
		else if (Input.GetKeyDown(KeyCode.T))
		{
			Tile tile = GridController.Instance.CurrentTile;

			if (tile.Unit != null)
				_tilesInRange = Astar.FindPathsInRange(tile.Node, tile.Unit.moveRange, tile.Unit.jumpHeight);
		}
	}

	#endregion

	#region Methods

	#endregion

	#region Gizmos

	private void OnDrawGizmos()
	{
		if (_path != null)
		{
			foreach (Astar.Node node in _path)
			{
				DrawGizmoSphere(node, 0.25f, Color.magenta);
			}
		}

		if (_tilesInRange != null)
		{
			foreach (Astar.Node node in _tilesInRange)
			{
				DrawGizmoSphere(node, 0.2f, Color.cyan);
			}
		}
	}

	private void DrawGizmoSphere(Astar.Node node, float radius, Color color)
	{
		Gizmos.color = color;
		Gizmos.DrawSphere(node.WorldPos, radius);
	}

	#endregion
}
