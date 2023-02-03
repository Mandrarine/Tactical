using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public class Tile
    {
        public GameObject Go;
        public Astar.Node Node;
    }

    [SerializeField] private AstarContext _astarContext;
    [SerializeField] private TargetFollow _targetFollow;
    [SerializeField] private GameObject _tileSelector;
    
    private Tile[,] tiles;
    private Tile currentTile;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            InitGrid();
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
            NavigateToDirection(0, 1);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            NavigateToDirection(1, 0);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            NavigateToDirection(0, -1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            NavigateToDirection(-1, 0);
    }
    
    private void InitGrid()
    {
        tiles = new Tile[_astarContext.GridHeight, _astarContext.GridWidth];
        Astar.Nodes = new Astar.Node[_astarContext.GridHeight, _astarContext.GridWidth];

        for (var y = 0; y < tiles.GetLength(0); y++)
        {
            for (var x = 0; x < tiles.GetLength(1); x++)
            {
                SamplePosition(x, y);
            }
        }

        SelectTileAtCoords(0, 0);
    }

    private void SamplePosition(int indexX, int indexY)
    {
        var position = transform.position;
        var posX = position.x + (indexX * _astarContext.NodeSize.x);
        var posY = position.z + (indexY * _astarContext.NodeSize.y);

        var lRayOrigin = new Vector3(posX, _astarContext.SamplingHeight, posY);
        var lRay = new Ray(lRayOrigin, Vector3.down);

        if (!Physics.Raycast(lRay, out RaycastHit hit, _astarContext.SamplingHeight + 1))
            return;
        
        var newNode = new Astar.Node
        {
            GridPos = new Astar.IntVector2(indexX, indexY),
            WorldPos = new Vector3(hit.point.x, 0, hit.point.z)
        };
        
        var newTile = new Tile
        {
            Go = hit.collider.gameObject,
            Node = newNode
        };

        switch (hit.collider.tag)
        {
            case "Ground":
                newNode.NavigationState = Enums.NavigationState.Walkable;
                break;
            /*
            case "Start":
                _startNode = newNode;
                newNode.NavigationState = Enums.NavigationState.Walkable;
                break;
            case "End":
                _endNode = newNode;
                newNode.NavigationState = Enums.NavigationState.Walkable;
                break;
            */
            default:
                newNode.NavigationState = Enums.NavigationState.Unwalkable;
                break;
        }

        tiles[indexY, indexX] = newTile;
        Astar.Nodes[indexY, indexX] = newNode;
    }

    private void SelectTileAtCoords(int coordX, int coordY)
    {
        currentTile = tiles[coordY, coordX];
        _targetFollow.Target = currentTile.Go.transform;
        _tileSelector.transform.position = currentTile.Go.transform.position + (Vector3.up * 0.01f);
    }

    private void NavigateToDirection(int dirX, int dirY)
    {
        int coordX = currentTile.Node.GridPos.X + dirX;
        int coordY = currentTile.Node.GridPos.Y + dirY;
        
        if (GetTileAtCoords(coordX, coordY, out Tile tile))
            SelectTileAtCoords(coordX, coordY);
    }
    
    private bool GetTileAtCoords(int coordX, int coordY, out Tile outputTile)
    {
        if (CoordsAreValid(coordX, coordY))
        {
            outputTile = tiles[coordY, coordX];
            return true;
        }

        outputTile = default;
        return false;
    }

    private bool CoordsAreValid(int coordX, int coordY)
    {
        return ((coordY >= 0) && (coordY < tiles.GetLength(0))) && ((coordX >= 0) && (coordX < tiles.GetLength(1)));
    }
}