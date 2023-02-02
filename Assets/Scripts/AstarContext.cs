using System.Collections.Generic;
using UnityEngine;

public class AstarContext : MonoBehaviour
{
    #region Attributes

    public static AstarContext Instance;

    [SerializeField] int m_gridWidth = 5;
    [SerializeField] int m_gridHeight = 5;
    [SerializeField] int m_samplingHeight = 2;
    [SerializeField] Vector2 m_nodeSize = Vector2.one;
    [SerializeField] bool m_allowDiagonals = true;
    [SerializeField] bool m_drawGizmos = true;
    [SerializeField, Range(0.0f, 0.5f)] float m_gizmosRadius = 0.1f;

    private Astar.Node m_startNode;
    private Astar.Node m_endNode;
    private List<Astar.Node> _path = new();

    #endregion

    #region Properties

    public int GridWidth => m_gridWidth;
    public int GridHeight => m_gridHeight;

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
            _path = FindPath(m_startNode, m_endNode);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            _path = null;
        }
    }

    #endregion

    #region Logic

    public List<Astar.Node> FindPath(Astar.Node p_start, Astar.Node p_end)
    {
        Astar.AllowDiagonals = m_allowDiagonals;
        return Astar.FindPath(m_startNode, m_endNode);
    }

    void InitGrid()
    {
        //Astar.Nodes = new Astar.Node[_gridWidth, _gridHeight];
        //Astar.Nodes = new Astar.Node[m_gridHeight, m_gridWidth];
        Astar.Nodes = new Astar.Node[m_gridHeight, m_gridWidth];
        Astar.AllowDiagonals = m_allowDiagonals;

        for (int y = 0; y < Astar.Nodes.GetLength(0); y++)
        {
            for (int x = 0; x < Astar.Nodes.GetLength(1); x++)
            {
                SamplePosition(x, y);
            }
        }
    }

    void SamplePosition(int p_indexX, int p_indexY)
    {
        //float l_posX = (p_indexX + transform.position.x) * m_nodeSize.x;
        //float l_posZ = (p_indexZ + transform.position.z) * m_nodeSize.y;
        
        float l_posX = transform.position.x + (p_indexX * m_nodeSize.x);
        float l_posZ = transform.position.z + (p_indexY * m_nodeSize.y);

        Vector3 lRayOrigin = new Vector3(l_posX, m_samplingHeight, l_posZ);
        Ray lRay = new Ray(lRayOrigin, Vector3.down);

        if (Physics.Raycast(lRay, out RaycastHit hit, m_samplingHeight + 1))
        {
            Astar.Node lNode = new Astar.Node();
            lNode.GridPos = new Astar.IntVector2(p_indexX, p_indexY);
            lNode.WorldPos = new Vector3(hit.point.x, 0, hit.point.z);
            
            Debug.Log("Sampling pos at [" + l_posX + "][" + l_posZ + "]");

            if (hit.collider.tag.Equals("Ground"))
            {
                lNode.NavigationState = Enums.NavigationState.Walkable;
            }
            else if (hit.collider.tag.Equals("Start"))
            {
                m_startNode = lNode;
                lNode.NavigationState = Enums.NavigationState.Walkable;
            }
            else if (hit.collider.tag.Equals("End"))
            {
                m_endNode = lNode;
                lNode.NavigationState = Enums.NavigationState.Walkable;
            }
            else
                lNode.NavigationState = Enums.NavigationState.Unwalkable;

            //Astar.Nodes[p_indexX, p_indexY] = lNode;
            Astar.Nodes[p_indexY, p_indexX] = lNode;
        }
    }

    #endregion

    #region Gizmos

    void OnDrawGizmos()
    {
        if (!m_drawGizmos || Astar.Nodes == null)
            return;

        foreach (Astar.Node node in Astar.Nodes)
        {
            if (node.NavigationState == Enums.NavigationState.Walkable)
                DrawGizmoSphere(node, m_gizmosRadius, Color.white);
            else
                DrawGizmoSphere(node, m_gizmosRadius, Color.red);
        }

        if (_path == null)
            return;

        foreach (var item in _path)
        {
            DrawGizmoSphere(item, m_gizmosRadius, Color.green);
        }
    }

    private void DrawGizmoSphere(Astar.Node pNode, float pRadius, Color pColor)
    {
        Gizmos.color = pColor;
        Gizmos.DrawSphere(pNode.WorldPos, pRadius);
        Gizmos.DrawLine(pNode.WorldPos + (Vector3.up * m_samplingHeight), pNode.WorldPos);
    }

    #endregion
}