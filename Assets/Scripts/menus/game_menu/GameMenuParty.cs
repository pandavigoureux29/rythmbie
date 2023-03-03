using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMenuParty : GameMenu {

    [SerializeField] List<CharacterInfos> m_characters;
    [SerializeField] Transform m_charUIParent;
    [SerializeField] float m_itemScale = 10.0f;
    
    DataCharManager m_charManager;
    ProfileManager.Profile m_profile;

    List<GameObject> m_charactersUI;

	// Use this for initialization
	void Awake () {
        m_profile = ProfileManager.instance.GetProfile();
        m_charManager = DataManager.instance.CharacterManager;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void Activate()
    {
        m_profile = ProfileManager.instance.GetProfile();
        m_charManager = DataManager.instance.CharacterManager;
        m_charactersUI = new List<GameObject>();
        base.Activate();
        for(int i=0; i < m_profile.CurrentTeam.Count; i++)
        {
            var container = GameUtils.CreateCharacterUIObject(m_profile.CurrentTeam[i], m_itemScale);
            container.transform.SetParent(m_charUIParent, false);

            m_charactersUI.Add(container);
        }
    }

    protected override void Deactivate()
    {
        base.Deactivate();
        Clear();
    }
    
    void Clear()
    {
        if (m_charactersUI == null)
            return;
        foreach(var chara in m_charactersUI)
        {
            Destroy(chara);
        }
        m_charactersUI.Clear();
    }

    [System.Serializable]
    public class CharacterInfos
    {
        public CharacterBuild m_build;
        public StatsFiller m_stats;
    }
}
