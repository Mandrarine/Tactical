using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	#region Fields

	public GameObject start;
	public GameObject end;
	public Unit unit;

	private Tile _startTile;
	private Tile _endTile;
	private List<Astar.Node> _path;

	#endregion

	#region Unity

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			_startTile = GridController.Instance.Tiles[(int)start.transform.position.z, (int)start.transform.position.x];
			_endTile = GridController.Instance.Tiles[(int)end.transform.position.z, (int)end.transform.position.x];
			_path = AstarContext.Instance.FindPath(_startTile, _endTile);

			if (_path != null)
				unit.MoveUnitAlongPath(_path);
		}
	}

	#endregion

	#region Methods

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
