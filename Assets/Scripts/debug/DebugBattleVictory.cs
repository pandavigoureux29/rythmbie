using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

public class DebugBattleVictory : MonoBehaviour {

    bool m_ended = false;
    	
	// Update is called once per frame
	void Update () {
        if (!m_ended)
            EndMatch();
	}

    void EndMatch()
    {
        m_ended = true;

        BattleScoreManager scoreManager = Component.FindObjectOfType<BattleScoreManager>();

        var keys = scoreManager.NotesCountByAccuracy.Keys.ToList() ;
        int totalNotes = Random.Range(0, 100);
        foreach (var acc in keys)
        {
            int accFloat = Random.Range(0, 100);
            for (int i = 0; i < accFloat; ++i)
                scoreManager.AddNote(accFloat);
        }

        BattleFightManager.instance.EndBattle(true);
    }
}
