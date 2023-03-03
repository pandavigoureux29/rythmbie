using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBattleManager : MonoBehaviour {
	
	[SerializeField] UIBattlePhaseTitle m_phaseTitle;

    [SerializeField] List<CharacterBuild> m_characters;

	// Use this for initialization
	void Start () {
        LoadCharas();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SwitchPhase(bool _attack){
		m_phaseTitle.Switch (_attack);
	}

    void LoadCharas()
    {
        //load team
        var teamCharsIds = ProfileManager.instance.GetProfile().CurrentTeam;
        for (int i = 0; i < teamCharsIds.Count; i++)
        {
            var charData = ProfileManager.instance.GetCharacter(teamCharsIds[i]);
            if(charData != null)
                m_characters[i].Load(charData);
        }
    }
}
