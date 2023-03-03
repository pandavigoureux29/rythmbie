using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public partial class MapNodesManager : SpriteTouchManager {
    [SerializeField] string m_mapName = "World1";
    //nodes building
    [SerializeField] Transform m_poolNodesObject;
    List<MapNode> m_nodes;
    //paths building
    [SerializeField] GameObject m_pathsContainerObject;
    [SerializeField] GameObject m_pathTemplate;
    [SerializeField] float m_bodyScaleMult = 1.0f;

    //Nodes
    [SerializeField] MapNode m_starterNode;
    MapNode m_currentNode;
    MapNode m_targetNode;
    List<MapNode> m_nodesPath = null; // the path from the current node to the target (when moving)

    [SerializeField] MapCharacter m_player;

    public enum State { IDLE, MOVING }
    private State m_state;

    void Start() {
        ListNodes();
        BuildPaths();
        SetStartNode();
        DataCheck();
    }

    void SetStartNode()
    {
        m_currentNode = m_nodes[0];
        string id = PlayerPrefs.GetString("current_map_node", m_nodes[0].Id);
        //find node 
        foreach(var node in m_nodes)
        {
            if(node.Id == id)
            {
                m_currentNode = node;
                m_starterNode = m_currentNode;
                break;
            }
        }
        Utils.Set2DPosition(m_player.transform, m_starterNode.transform.position);
    }

    public void OnGoToGameMenu()
    {
        SceneManager.LoadScene("game_menu");
    }
    #region TOUCH

    protected override void OnReleased(Collider2D _collider)
    {
        base.OnReleased(_collider);
        MapNode node = GetNodeTouched(_collider);
        if( node != null )
            OnNodeTouchRelease(node);
    }

    void OnNodeTouchRelease(MapNode _node)
    {
        if (_node.Locked)
            return;

        //build the path of nodes
        m_nodesPath = new List<MapNode>();
        //Launch the walking
        m_nodesPath = CreatePathProcess(_node);
        m_targetNode = _node;
        if( m_currentNode == m_targetNode)
        {
            OnPlayerReachedNode(m_targetNode);
        }else
        {
            m_state = State.MOVING;
        }
    }

    MapNode GetNodeTouched(Collider2D _colliderTouched)
    {
        for (int i = 0; i < m_nodes.Count; ++i)
        {
            if (m_nodes[i].gameObject.GetComponent<Collider2D>() == _colliderTouched)
            {
                return m_nodes[i];
            }
        }
        return null;
    }

    #endregion

    void Update() {
        ProcessTouch();
        switch (m_state)
        {
            case State.MOVING: Moving(); break;
        }
    }

    void Moving()
    {
        if (m_currentNode == m_targetNode || m_nodesPath.Count == 0)
        {
            m_state = State.IDLE;
            m_currentNode = m_targetNode;
        }
        else if (m_player.IsMoving() == false)
        {
            m_currentNode = m_player.CurrentNode;
            MapNode nextNode = m_nodesPath[0];
            m_nodesPath.RemoveAt(0);
            m_player.GoTo(nextNode);
        }
    }

    public void OnPlayerReachedNode(MapNode _node)
    {
        if (_node == m_targetNode)
        {
            m_state = State.IDLE;
            var uiPopup = UIManager.instance.Popup();
            uiPopup.GetButton("ConfirmButton").Set("Fight", "OnBeginFight", gameObject, false);
            uiPopup.Open();
        }
    }

    public void OnBeginFight()
    {
        Debug.Log("FIGHT");
        BattleDataAsset data = m_targetNode.BattleData;
        DataManager.instance.BattleData = data;
        //Save map
        PlayerPrefs.SetString("current_map_scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetString("current_map_node", m_targetNode.Id);
        PlayerPrefs.SetString("current_map", m_mapName);
        PlayerPrefs.SetString("current_level", m_targetNode.Id);
        SceneManager.LoadScene(data.sceneName);
    }

    #region PATH

    /// <summary>
    /// Create the path that the character has to take to reach the target node
    /// </summary>
    List<MapNode> CreatePathProcess(MapNode _targetNode)
    {
        List<MapNode> nodes = new List<MapNode>();
        CreatePath(_targetNode, null, ref nodes);
        return nodes;
    }

    /// <summary>
    /// Recusively create the path to take
    /// </summary>
    bool CreatePath(MapNode processedNode, MapNode caller, ref List<MapNode> path)
    {
        //Debug.Log(processedNode.name);
        if (processedNode == m_currentNode)
        {
            path.Add(processedNode);
            return true;
        }
        //search from child
        foreach (var child in processedNode.Children)
        {
            if (child == null || child == caller)
                continue;
            if (CreatePath(child, processedNode, ref path))
            {
                path.Add(processedNode);
                return true;
            }
        }
        //search from parent
        foreach (var parent in processedNode.Parents)
        {
            if (parent == null || parent == caller)
                continue;
            if (CreatePath(parent, processedNode, ref path))
            {
                path.Add(processedNode);
                return true;
            }
        }
        return false;
    }

    #endregion

    #region NODES_BUILDING
    /// <summary>
    /// Search for nodes components and store them in the list
    /// </summary>
    void ListNodes()
    {
        if (m_nodes == null)
            m_nodes = new List<MapNode>();
        foreach(Transform t in m_poolNodesObject)
        {
            MapNode node = t.GetComponent<MapNode>();
            m_nodes.Add(node);
        }
    }

    void BuildPaths()
    {
        foreach( var node in m_nodes)
        {
            foreach (var nodeChild in node.Children)
            {
                if( nodeChild != null )
                    BuildPath(node, nodeChild);
            }
        }
    }

    void BuildPath(MapNode node1, MapNode node2)
    {
        //Compute path's center position
        Vector3 toNode2 = node2.transform.position - node1.transform.position ;
        float mag = toNode2.magnitude;
        Vector3 mid = node1.transform.position + toNode2.normalized * mag *0.5f;
        mid.z = 1;

        //Instantiate template in scene
        var go = Instantiate(m_pathTemplate);
        Transform pathTransform = go.transform;
        go.name = "Path_" + node1.name + "_" + node2.name;
        //position
        pathTransform.parent = m_pathsContainerObject.transform;
        pathTransform.position = mid;
        //Scale
        Utils.SetLocalScaleX(pathTransform, mag * m_bodyScaleMult);
        //Rotation
        float angle = Utils.AngleBetweenVectors(Vector3.right, toNode2 );
        Utils.SetLocalAngleZ(pathTransform, angle);

    }
    #endregion

    /// <summary>
    /// Unlocks levels
    /// </summary>
    public void DataCheck()
    {
        var mapData = ProfileManager.instance.GetMapData(m_mapName);
        foreach(var node in m_nodes)
        {
            var levelData = mapData.Levels.Find(x => x.Id == node.Id);
            if( levelData != null)
            {
                node.Score = levelData.Score;
                if( levelData.Score > 0 )
                {
                    node.Unlock();
                    node.Score = levelData.Score;
                }
            }
            if (node.Parents.Count == 0)
                node.Unlock();
        }
    }
}
