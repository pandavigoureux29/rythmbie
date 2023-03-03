using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNode : MonoBehaviour {

    [SerializeField] string m_id = "MapId";
    public bool Locked { get; set; }
    public int Score { get; set; }

    [SerializeField] SpriteRenderer m_sprite;

    [SerializeField] List<MapNode> m_parents;
    [SerializeField] List<MapNode> m_children;
    
    [SerializeField] BattleDataAsset m_battleData;

    void Awake()
    {
        Lock();
    }
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Lock()
    {
        Locked = true;
        m_sprite.color = Color.gray;
    }

    public void Unlock()
    {
        Locked = false;
        m_sprite.color = Color.white;
    }

    #region GETTERS-SETTERS

    public List<MapNode> Parents
    {
        get
        {
            return m_parents;
        }        
    }

    public List<MapNode> Children
    {
        get
        {
            return m_children;
        }        
    }

    public BattleDataAsset BattleData
    {
        get
        {
            return m_battleData;
        }        
    }

    public string Id
    {
        get
        {
            return m_id;
        }
    }
    #endregion
}
