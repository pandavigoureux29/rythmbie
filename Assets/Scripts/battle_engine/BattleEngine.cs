using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BattleEngine : MonoBehaviour {

	public static BattleEngine _instance = null;
	public enum Difficulty { EASY, MEDIUM, HARD};

	[SerializeField] protected BattleTracksManager m_tracksManager;
	[SerializeField] protected BattleFightManager m_fightManager;

	[SerializeField] protected BattleScoreManager m_scoreManager;
	[SerializeField] protected GameObject m_notesGeneratorObject;
	protected BattleNotesGenerator m_notesGenerator;

	[SerializeField] protected UIBattleManager m_ui;
    
	int m_switchCount = 0;
	int m_nextSwitchCount = 0;
    
	bool m_debug = false;

    [SerializeField] BattleDataAsset m_battleDataAsset;

	//Audio
	protected AudioSource m_audioSource;
	protected AudioClip m_audioClip;
	/** Time for a note from its launch to its arrival */
	protected float m_timeShift;
	protected float m_sampleRateToTimeModifier;

    /// <summary>
    /// Time elapsed at the last frame
    /// </summary>
    protected float m_lastTimeElapsed = 0.0f;

	void Awake(){
		_instance = this;
		//TODO remove this. ensures that the data is loaded 
		DataManager.instance.ToString();
	}

	// Use this for initialization
	void Start () {
		m_audioSource = GetComponent<AudioSource> ();

		BattleNotesGenerator[] gens = m_notesGeneratorObject.GetComponents<BattleNotesGenerator> ();
		for (int i=0; i < gens.Length; i++) {
			if (gens [i].enabled)
				m_notesGenerator = gens [i];
		}
		TimerEngine.instance.AddTimer (1.0f, "BeginFight", gameObject);

		//events
		m_fightManager.endBattleEventHandler += OnFightEnded;
	}

	void OnDestroy(){
		//events
		m_fightManager.endBattleEventHandler -= OnFightEnded;
	}

	void BeginFight(){
        if (DataManager.instance.BattleData != null)
        {
            m_battleDataAsset = DataManager.instance.BattleData;
        }
        Load(m_battleDataAsset);
        m_notesGenerator.Begin(m_timeShift, m_battleDataAsset.TimeBegin);
        //Play song
        m_audioSource.clip = m_audioClip;
		m_audioSource.Play ();
        //change begin time
        if(m_battleDataAsset.TimeBegin > m_audioClip.length) {
            Debug.LogError("Error with begin time setting (" + m_battleDataAsset.TimeBegin + ") . Total song length = " + m_audioClip.length);
        }else {
            m_audioSource.time = m_battleDataAsset.TimeBegin;
        }

        SwitchPhase ();
		m_nextSwitchCount = m_battleDataAsset.AttackNotesCount;        
	}

    public void OnFightEnded(object sender, BattleFightManager.EndBattleEventInfo eventInfo)
    {
        BattleData battleData = new BattleData();
        ProfileManager.instance.BattleData = battleData;
        //total Xp
        int totalXp = 0;
        foreach(var enemy in m_battleDataAsset.Enemies)
        {
            totalXp += DataManager.instance.EnemiesManager.GetEnemy(enemy.Id).XpGranted; 
        }
        battleData.TotalXp = totalXp;
        //add xp and store it in BattleData
        foreach(var charData in ProfileManager.instance.GetCurrentTeam())
        {
            var oldXp = charData.Xp;
            charData.Xp += totalXp;
            battleData.AddPlayerData(charData.Id, oldXp, totalXp);
        }
        //Score
        battleData.NotesCount = m_scoreManager.NotesCount;
        battleData.NotesCountByAccuracy = m_scoreManager.NotesCountByAccuracy;
        battleData.TotalScore = m_scoreManager.TotalScore;
        //Add Shard

        //EndLevel
        string mapName = PlayerPrefs.GetString("current_map");
        string levelName = PlayerPrefs.GetString("current_level");
        ProfileManager.instance.EndLevel(mapName, levelName, 200, true);
        ProfileManager.instance.SaveProfile();

        SceneManager.LoadScene("battle_end");
    }

    public void OnQuitBattle(){
        SceneManager.LoadScene("main_menu");
	}
	
	// Update is called once per frame
	void Update () {
        CheckMusicLoop();
	}
    
    #region LOADING
    
    void Load(BattleDataAsset battleData)
    {
        //Load Battle Data
        TextAsset jsonFile = battleData.Song;
        JSONObject jsonData = new JSONObject(jsonFile.text);

        //Load Note Generator
        m_notesGenerator.LoadData(jsonData);

        //Load song music
        ///string clipPath = jsonData.GetField("clipPath").ToString();
        string clipName = jsonData.GetField("clipName").str;
        m_audioClip = Resources.Load("songs/" + clipName) as AudioClip;
        m_sampleRateToTimeModifier = 1.0f / m_audioClip.frequency;

        //time used by the generator to spawn notes. Sets the speed of notes.
        m_timeShift = jsonData.GetField("timeSpeed").n;

        m_fightManager.Load(battleData);
    }

    #endregion

    #region SWITCH_ATTACK_DEFENSE

    /// <summary>
    ///  Called from tracks manager when a note is launched. Checks how many notes has been launched is the current phase and switch if necessary
    /// </summary>
    public void OnNoteLaunched(NoteData _data){
		m_switchCount ++;
        //Don't switch between a long note
		if (_data.Type == NoteData.NoteType.LONG && _data.Head) {
			return;
		}
		//when it's time to switch
		if (!m_debug && m_switchCount >= m_nextSwitchCount) {
			SwitchPhase();
		}
	}

	void SwitchPhase(){
		m_tracksManager.SwitchPhase ();		
	}

	public void OnSwitchSuccessful(){
		if (m_tracksManager.PhaseState == BattleTracksManager.BattleState.ATTACK)
			m_nextSwitchCount = m_battleDataAsset.AttackNotesCount;
		else
			m_nextSwitchCount = m_battleDataAsset.DefenseNotesCount;
		m_switchCount = 0;
		//UI
		m_ui.SwitchPhase (m_tracksManager.IsAttacking);
	}

	#endregion

	#region FIGHT

	public void OnLaunchMagic(BattleMagic _magic){
	}

	public void OnMagicEnded(BattleMagic _magic){
	}

	/** Called by the FightManager when a group of actor is dead on one side/
	* 	Disables the track specified */ 
	public void OnDisableTrack(int _index, int _replacementTrack){
		m_tracksManager.DisableTrack (_index, _replacementTrack);
	}

    #endregion

    void CheckMusicLoop()
    {
        if( m_lastTimeElapsed > MusicTimeElapsed)
        {
            m_notesGenerator.OnMusicLoop();
        }
        m_lastTimeElapsed = MusicTimeElapsed;
    }
        
    #region GETTERS

    public static BattleEngine instance {
		get{
			return _instance;
		}
	}

	public BattleTracksManager.BattleState BattleState{
		get{
			return m_tracksManager.PhaseState;
		}
	}

	public AudioSource AudioSrc {
		get {
			return m_audioSource;
		}
	}

    /// <summary>
    /// Time elapsed in the music. Always between 0 and music length
    /// </summary>
	public float MusicTimeElapsed{
		get{
			return m_audioSource.timeSamples * m_sampleRateToTimeModifier;
		}
	}
	                      
	public bool IsAttacking{
		get{
			return m_tracksManager.IsAttacking;
		}
	}

	#endregion
}
