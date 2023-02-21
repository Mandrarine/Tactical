using UnityEngine;

public class GridController : MonoBehaviour
{
	#region Fields

	public static GridController Instance;

	[SerializeField] private AstarContext _astarContext;
	[SerializeField] private TargetFollow _targetFollow;
	[SerializeField] private GameObject _tileSelector;

	private Tile[,] _tiles;
	private Tile _currentTile;

	public Unit unitPlayer;
	public Unit unitEnemy;

	#endregion

	#region Properties

	public Tile[,] Tiles => _tiles;
	public Tile CurrentTile => _currentTile;

	#endregion

	#region Unity

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		InitGrid();

		Tile tilePlayer = _tiles[(int)unitPlayer.transform.position.z, (int)unitPlayer.transform.position.x];
		Tile tileEnemy = _tiles[(int)unitEnemy.transform.position.z, (int)unitEnemy.transform.position.x];

		tilePlayer.Unit = unitPlayer;
		tileEnemy.Unit = unitEnemy;

		_currentTile = tilePlayer;

		SelectTile(_currentTile);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
			NavigateToDirection(1, 0, true);
		else if (Input.GetKeyDown(KeyCode.RightArrow))
			NavigateToDirection(0, -1, true);
		else if (Input.GetKeyDown(KeyCode.DownArrow))
			NavigateToDirection(-1, 0, true);
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
			NavigateToDirection(0, 1, true);
	}

	#endregion

	#region Methods

	private void InitGrid()
	{
		Astar.Nodes = new Astar.Node[_astarContext.GridHeight, _astarContext.GridWidth];
		_tiles = new Tile[_astarContext.GridHeight, _astarContext.GridWidth];

		for (var y = 0; y < _tiles.GetLength(0); y++)
		{
			for (var x = 0; x < _tiles.GetLength(1); x++)
			{
				SampleGridAtCoords(x, y);
			}
		}
	}

	private void SampleGridAtCoords(int indexX, int indexY)
	{
		var position = transform.position;
		var posX = position.x + (indexX * _astarContext.NodeSize.x);
		var posY = position.z + (indexY * _astarContext.NodeSize.y);

		var lRayOrigin = new Vector3(posX, _astarContext.SamplingHeight, posY);
		var lRay = new Ray(lRayOrigin, Vector3.down);

		if (Physics.Raycast(lRay, out RaycastHit hit, _astarContext.SamplingHeight + 1))
		{
			var newNode = new Astar.Node
			{
				GridPos = new Astar.IntVector2(indexX, indexY),
				WorldPos = hit.point
			};

			Astar.Nodes[indexY, indexX] = newNode;

			var newTile = new Tile
			{
				Go = hit.collider.gameObject,
				Node = newNode
			};

			_tiles[indexY, indexX] = newTile;
		}
		else
		{
			Astar.Nodes[indexY, indexX] = null;
			_tiles[indexY, indexX] = null;
		}
	}

	private void SelectTile(Tile tile)
	{
		SelectTileAtCoords(tile.Node.GridPos.X, tile.Node.GridPos.Y);
	}

	private void SelectTileAtCoords(int coordX, int coordY)
	{
		_currentTile = _tiles[coordY, coordX];
		//_targetFollow.Target = currentTile.Go.transform;
		_tileSelector.transform.position = _currentTile.Go.transform.position;
		UIManager.Instance.DisplayUnitInfo(_currentTile.Unit);
	}

	private void NavigateToDirection(int dirX, int dirY, bool deepSearch)
	{
		int coordX = _currentTile.Node.GridPos.X;
		int coordY = _currentTile.Node.GridPos.Y;
		Tile tile = default;

		do
		{
			coordX += dirX;
			coordY += dirY;

			TryGetTileAtCoords(coordX, coordY, out tile);

			if (tile != null)
				SelectTileAtCoords(coordX, coordY);
		}
		while (deepSearch && (tile == null) && CoordsAreValid(coordX, coordY));
	}

	private bool TryGetTileAtCoords(int coordX, int coordY, out Tile tile)
	{
		tile = default;

		if (CoordsAreValid(coordX, coordY))
		{
			tile = _tiles[coordY, coordX];
			return true;
		}

		return false;
	}

	private bool CoordsAreValid(int coordX, int coordY)
	{
		return ((coordY >= 0) && (coordY < _tiles.GetLength(0))) && ((coordX >= 0) && (coordX < _tiles.GetLength(1)));
	}

	#endregion

	#region Gizmos

	private void OnDrawGizmos()
	{
		if (Tiles == null)
			return;

		foreach (Tile tile in Tiles)
		{
			if (tile == null)
				continue;

			DrawGizmoSphere(tile.Node, 0.1f, Color.white);
		}
	}

	private void DrawGizmoSphere(Astar.Node node, float radius, Color color)
	{
		Gizmos.color = color;
		Gizmos.DrawSphere(node.WorldPos, radius);
	}

	#endregion
}