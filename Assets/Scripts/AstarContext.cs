using System;
using System.Collections.Generic;
using UnityEngine;

public class AstarContext : MonoBehaviour
{
	#region Fields

	public static AstarContext Instance;

	[SerializeField] private int _gridWidth = 5;
	[SerializeField] private int _gridHeight = 5;
	[SerializeField] private Vector2 _nodeSize = Vector2.one;
	[SerializeField] private int _samplingHeight = 5;
	[SerializeField] private bool _allowDiagonals = true;
	[SerializeField] private bool _drawGizmos = true;
	[SerializeField, Range(0.0f, 0.5f)] private float _gizmosRadius = 0.1f;

	#endregion

	#region Properties

	public int GridWidth => _gridWidth;
	public int GridHeight => _gridHeight;
	public Vector2 NodeSize => _nodeSize;
	public float SamplingHeight => _samplingHeight;

	#endregion

	#region Unity

	private void OnEnable()
	{
		Astar.Nodes = null;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDisable()
	{
		Astar.Nodes = null;
	}

	#endregion

	#region Methods

	public List<Astar.Node> FindPath(Tile startTile, Tile endTile)
	{
		Astar.AllowDiagonals = _allowDiagonals;

		return Astar.FindPath(startTile.Node, endTile.Node);
	}

	#endregion
}