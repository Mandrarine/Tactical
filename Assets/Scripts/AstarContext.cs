using System;
using System.Collections.Generic;
using UnityEngine;

public class AstarContext : MonoBehaviour
{
    #region Members

    public static AstarContext Instance;
    
    [SerializeField] private int _gridWidth = 5;
    [SerializeField] private int _gridHeight = 5;
    [SerializeField] private Vector2 _nodeSize = Vector2.one;
    [SerializeField] private int _samplingHeight = 5;
    [SerializeField] private bool _allowDiagonals = true;
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField, Range(0.0f, 0.5f)] private float _gizmosRadius = 0.1f;

    //public Astar.Node StartNode { get; set; }
    //public Astar.Node EndNode { get; set; }
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

    #region Logic

    public List<Astar.Node> FindPath(Tile startTile, Tile endTile)
    {
        //StartNode = startTile.Node;
        //EndNode = endTile.Node;

        Astar.AllowDiagonals = _allowDiagonals;

        return Astar.FindPath(startTile.Node, endTile.Node);
    }

    #endregion

    /*
    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!_drawGizmos || Astar.Nodes == null)
            return;

        foreach (Astar.Node node in Astar.Nodes)
        {
            //if (node.NavigationState == Enums.NavigationState.Walkable)
            //    DrawGizmoSphere(node, _gizmosRadius, Color.white);
            //else
            //    DrawGizmoSphere(node, _gizmosRadius, Color.red);
            DrawGizmoSphere(node, _gizmosRadius, Color.white);
        }
    }

    private void DrawGizmoSphere(Astar.Node node, float radius, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(node.WorldPos, radius);
    }

    #endregion
*/
}