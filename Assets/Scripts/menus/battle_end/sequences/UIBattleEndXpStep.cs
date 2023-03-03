using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleEndXpStep : UIStep {

    [SerializeField] List<BattleEndManager.ScoreInfo> m_scoresInfos;
    [SerializeField] List<UIXpScrollerManager> m_scrollers;

    [SerializeField] List<BattleEndCharInfoUI> m_characters;

    int m_count = 0;

    public override void Launch(OnStepEndDelegate _del)
    {
        base.Launch(_del);

        var team = ProfileManager.instance.GetCurrentTeam();

        var battleData = ProfileManager.instance.BattleData;

        for (int i = 0; i < m_characters.Count; ++i)
        {
            var chara = m_characters[i];
            var charaProfile = team[i];
            var job = charaProfile.Job;

            //Get battle data for the character
            var charBattleData = battleData.GetCharacter(charaProfile.Id);

            chara.XpScroller.Scroll(charBattleData.XpStart, charaProfile.Xp, job, 2.0f, OnXpScrollerEnd);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Skip()
    {
        base.Skip();

        for (int i = 0; i < m_characters.Count; ++i)
        {
            var chara = m_characters[i];
            chara.XpScroller.Skip();
        }
    }

    public void OnXpScrollerEnd(UIXpScrollerManager _xpScroller)
    {
        Stop();
    }
}
