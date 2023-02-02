using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AstarContext : MonoBehaviour
{
    #region Attributes

    public static AstarContext Instance;

    [SerializeField] private int _gridWidth = 5;
    [SerializeField] private int _gridHeight = 5;
    [SerializeField] private int _samplingHeight = 5;
    [SerializeField] private Vector2 _nodeSize = Vector2.one;
    [SerializeField] private bool _allowDiagonals = true;
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField, Range(0.0f, 0.5f)] private float _gizmosRadius = 0.1f;

    private Astar.Node _startNode;
    private Astar.Node _endNode;
    private List<Astar.Node> _path = new();

    #endregion

    #region Unity Callbacks

    private void OnDisable()
    {
        Astar.Nodes = null;
        _path = null;
    }    

    #endregion

    #region Initialization

    void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Updates

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            InitGrid();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            _path = FindPath(_startNode, _endNode);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Astar.Nodes = null;
            _path = null;
        }
    }

    #endregion

    #region Logic

    public List<Astar.Node> FindPath(Astar.Node startNode, Astar.Node endNode)
    {
        Astar.AllowDiagonals = _allowDiagonals;
        return Astar.FindPath(_startNode, _endNode);
    }

    private void InitGrid()
    {
        Astar.Nodes = new Astar.Node[_gridHeight, _gridWidth];
        Astar.AllowDiagonals = _allowDiagonals;

        for (var y = 0; y < Astar.Nodes.GetLength(0); y++)
        {
            for (var x = 0; x < Astar.Nodes.GetLength(1); x++)
            {
                SamplePosition(x, y);
            }
        }
    }

    private void SamplePosition(int indexX, int indexY)
    {
        var position = transform.position;
        var posX = position.x + (indexX * _nodeSize.x);
        var posY = position.z + (indexY * _nodeSize.y);

        var lRayOrigin = new Vector3(posX, _samplingHeight, posY);
        var lRay = new Ray(lRayOrigin, Vector3.down);

        if (!Physics.Raycast(lRay, out RaycastHit hit, _samplingHeight + 1))
            return;
        
        var newNode = new Astar.Node
        {
            GridPos = new Astar.IntVector2(indexX, indexY),
            WorldPos = new Vector3(hit.point.x, 0, hit.point.z)
        };

        switch (hit.collider.tag)
        {
            case "Ground":
                newNode.NavigationState = Enums.NavigationState.Walkable;
                break;
            case "Start":
                _startNode = newNode;
                newNode.NavigationState = Enums.NavigationState.Walkable;
                break;
            case "End":
                _endNode = newNode;
                newNode.NavigationState = Enums.NavigationState.Walkable;
                break;
            default:
                newNode.NavigationState = Enums.NavigationState.Unwalkable;
                break;
        }

        Astar.Nodes[indexY, indexX] = newNode;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!_drawGizmos || Astar.Nodes == null)
            return;

        foreach (Astar.Node node in Astar.Nodes)
        {
            if (node.NavigationState == Enums.NavigationState.Walkable)
                DrawGizmoSphere(node, _gizmosRadius, Color.white);
            else
                DrawGizmoSphere(node, _gizmosRadius, Color.red);
        }

        if (_path == null || _path.Count <= 1)
            return;

        foreach (var item in _path)
        {
            DrawGizmoSphere(item, _gizmosRadius, Color.green);
        }
    }

    private void DrawGizmoSphere(Astar.Node node, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(node.WorldPos, radius);
        Gizmos.DrawLine(node.WorldPos + (Vector3.up * _samplingHeight), node.WorldPos);
    }

    #endregion
}