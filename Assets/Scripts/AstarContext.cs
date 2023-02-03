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
    [SerializeField] private Vector2 _nodeSize = Vector2.one;
    [SerializeField] private int _samplingHeight = 5;
    [SerializeField] private bool _allowDiagonals = true;
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField, Range(0.0f, 0.5f)] private float _gizmosRadius = 0.1f;

    private Astar.Node _startNode;
    private Astar.Node _endNode;
    private List<Astar.Node> _path = new();

    #endregion

    #region Properties

    public int GridWidth => _gridWidth;
    public int GridHeight => _gridHeight;
    public Vector2 NodeSize => _nodeSize;
    public float SamplingHeight => _samplingHeight;

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
        if (Input.GetKeyDown(KeyCode.F))
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
    }

    #endregion
}