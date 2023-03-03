using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameMenuMixiGenerator : GameMenu
{
    [SerializeField] GameObject m_buttonGen;
    [SerializeField] GameObject m_mixiGO;
    GameObject m_currentMixi;

    [SerializeField] float m_mixiScale = 5;

    [SerializeField] Transform m_shardsPanel;
    [SerializeField] GameObject m_shardPrefab;

    MixiGenerator generator;

    Dictionary<string, UIShard> m_uiShards = new Dictionary<string, UIShard>();

    string m_selectedShardId;
    int m_selectedShardCount = 0;

	// Use this for initialization
	void Start () {
        generator = new MixiGenerator();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void Activate()
    {
        base.Activate();
        m_mixiGO.SetActive(false);
        m_buttonGen.SetActive(false);
        CreateShardList();
    }

    void CreateShardList()
    {
        m_uiShards.Clear();
        //clear transform
        for(int i=0; i < m_shardsPanel.childCount; ++i)
        {
            Destroy(m_shardsPanel.GetChild(i).gameObject);
        }
        //Create as much shards as we have in the profile
        foreach( var shard in ProfileManager.instance.profile.Shards)
        {
            GameObject instance = Instantiate(m_shardPrefab);

            var shardComponent = instance.GetComponent<UIShard>();
            shardComponent.LoadShard(shard.Id, shard.Quantity,this);
            m_uiShards[shard.Id] = shardComponent;

            instance.transform.SetParent(m_shardsPanel,false);
        }
    }

    public void OnGenerateMixiButtonClicked()
    {
        m_buttonGen.SetActive(false);
        m_mixiGO.SetActive(true);

        //generate the data for the mixi
        var chara = generator.Generate(m_selectedShardId);
        ProfileManager.instance.AddCharacter(chara);
        
        //Create the ui object to display
        m_currentMixi = GameUtils.CreateCharacterUIObject(chara, m_mixiScale, false);
        Destroy(m_currentMixi.GetComponent<UIInventoryDraggableItem>());
        m_currentMixi.transform.SetParent(m_mixiGO.transform, false);

        m_selectedShardCount = 0;
        m_selectedShardId = null;

        foreach(var shard in ProfileManager.instance.profile.Shards)
        {
            var uishard = m_uiShards[shard.Id];
            uishard.RefreshQuantity(shard.Quantity);
        }
    }

    public void OnSelectShard(string _shardId)
    {
        m_selectedShardId = _shardId;
        m_selectedShardCount = 1;
        m_shardsPanel.gameObject.SetActive(false);
        m_buttonGen.SetActive(true);
    }

    public void OnMixiClicked()
    {
        //Delete current Mixi
        if (m_currentMixi != null)
        {
            Destroy(m_currentMixi);
        }
        m_mixiGO.SetActive(false);
        m_shardsPanel.gameObject.SetActive(true);
    }
}
