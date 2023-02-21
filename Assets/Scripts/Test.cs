/*
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	#region Fields

	public Unit unitPlayer;

	private List<Astar.Node> _path;
	private List<Astar.Node> _moveTiles;

	#endregion

	#region Unity

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			Tile playerUnitTile = GridManager.Instance.Tiles[(int)unitPlayer.transform.position.z, (int)unitPlayer.transform.position.x];
			Tile endTile = GridManager.Instance.CurrentTile;

			if ((_moveTiles == null) || (!_moveTiles.Contains(endTile.Node)))
				return;

			_path = Astar.FindPath(playerUnitTile.Node, endTile.Node, playerUnitTile.Unit.jumpHeight);

			if (_path != null)
			{
				unitPlayer.MoveUnitAlongPath(_path);
				playerUnitTile.Unit = null;
				endTile.Unit = unitPlayer;
			}
		}
		else if (Input.GetKeyDown(KeyCode.F))
		{
			if (_moveTiles != null)
				_moveTiles = null;

			Tile tile = GridManager.Instance.CurrentTile;

			if (tile.Unit != null)
			{
				_moveTiles = Astar.FindNodesInRange(tile.Node, tile.Unit.moveRange, tile.Unit.jumpHeight);
				_moveTiles.Remove(tile.Node);
			}
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			_path = null;
			_moveTiles = null;
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

		if (_moveTiles != null)
		{
			foreach (Astar.Node node in _moveTiles)
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
*/