using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class BattleEndManager : MonoBehaviour {

    [SerializeField] UIBattleEndScoreScrollSequence m_scoreSequence;
    [SerializeField] UIBattleEndXpSequence m_xpSequence;

    [SerializeField] List<ScoreInfo> m_scoresInfos;

    [SerializeField] Text m_totalXpText;
    [SerializeField] UITextNumberScroller m_totalScoreText;

    [SerializeField] List<BattleEndCharInfoUI> m_characters;

    [SerializeField] GameObject m_mapButton;

	[SerializeField] float m_timeDisplayScore = 0.5f;
    enum State { IDLE, SCORE, XP };
    State m_state = State.IDLE;

    [SerializeField]  float m_uiCharaScale = 40;

    BattleData m_battleData;

    DataCharManager m_charManager;
    
    //UI Value
    float m_time = 0;

    int m_count = 0;
	[SerializeField] float m_scoreTimeByUnits = 0.05f;

	// Use this for initialization
	void Start () {
        //m_mapButton.SetActive(false);
        m_charManager = DataManager.instance.CharacterManager;
        m_battleData = ProfileManager.instance.BattleData;

        //Debug
        if (m_battleData.Characters.Count == 0)
        {
            DebugFillBattleData();
        }

        InitCharacters();

        m_scoreSequence.Launch(OnAccuraciesScrollingEnd, m_battleData.NotesCountByAccuracy, 100); 
        //m_xpSequence.Launch(OnXpScrollingEnd);
    }

    void Update()
    {
    }

    #region XP
        
    void InitCharacters()
    {
        var teamMates = ProfileManager.instance.GetCurrentTeam();
        for (int i = 0; i < m_characters.Count; ++i)
        {
            var chara = m_characters[i];
            var mate = teamMates[i];            
            var levelUpData = m_charManager.GetNextLevelByXp(mate.Job, mate.Xp);
            var battleCharData = m_battleData.Characters[i];

            if (mate != null)
            {
                //Set xp before battle
                chara.XpText.text = "" + battleCharData.XpStart;
                float prog = 1.0f;
                if (levelUpData.XpNeeded != 0)
                    prog = (float)battleCharData.XpStart / levelUpData.XpNeeded;
                if( chara.Gauge != null )
                    chara.SetGaugeValue(prog);
            }

            //create UI characters
            var go = GameUtils.CreateCharacterUIObject(mate, m_uiCharaScale);
            go.transform.SetParent(chara.CharacterObject.transform, false);
        }
    }
    
    void SetTotalXp(int _totalXp)
    {
        m_totalXpText.text = "" + _totalXp;
    }
    #endregion

    #region SCORE

    void ApplyScore()
    {
	    m_totalScoreText.ScrollFromTo(0, m_battleData.TotalScore, m_timeDisplayScore);

        foreach(var scoreInfo in m_scoresInfos)
        {
            int count = m_battleData.NotesCountByAccuracy[scoreInfo.Accuracy];
            scoreInfo.Scroller.ScrollFromTo(0,count, m_timeDisplayScore);
        }
        TimerEngine.instance.AddTimer(m_timeDisplayScore + 0.5f, "OnScoreTargetReached", this.gameObject);
        m_state = State.SCORE;
    }

    void OnXpScrollerEnded(UIXpScrollerManager _scroller)
    {
        m_mapButton.SetActive(true);
    }

    public void OnAccuraciesScrollingEnd(UISequence sequence)
    {
        m_xpSequence.Launch(OnXpScrollingEnd);
    }

    public void OnXpScrollingEnd(UISequence sequence)
    {
        Debug.Log("SCrOLL END FOR XP");
    }

    #endregion

    void OnGoToMap()
    {
        string mapSceneName = PlayerPrefs.GetString("current_map_scene");
        SceneManager.LoadScene(mapSceneName);
    }

    /// <summary>
    /// Used only in debug when launching the scene directly
    /// </summary>
    void DebugFillBattleData()
    {
        m_battleData = new BattleData();
        foreach (var chara in ProfileManager.instance.GetCurrentTeam())
        {
            m_battleData.AddPlayerData(chara.Id, 2, 48);
            ProfileManager.instance.AddCharacterXp( chara.Id, 50 );
        }
        m_battleData.TotalXp = 48;

        m_battleData.NotesCountByAccuracy = new Dictionary<HitAccuracy, int>();
        int totalNotes = 0;
        for(int i=0; i < Utils.EnumCount(HitAccuracy.GOOD); ++i)
        {
            HitAccuracy acc = (HitAccuracy)i;
            int count = Random.Range(0, 20);
            m_battleData.NotesCountByAccuracy[acc] = count;
            totalNotes += count;
        }
        m_battleData.NotesCount = totalNotes;
        m_battleData.TotalScore = Random.Range(0, 1000);
        ProfileManager.instance.BattleData = m_battleData;
    }

    [System.Serializable]
    public class ScoreInfo
    {
        [SerializeField] public GameObject UIObject;
        [SerializeField] public Text ScoreText;
        [SerializeField] public UITextNumberScroller Scroller;
        [SerializeField] public HitAccuracy Accuracy;
    }
            
}
