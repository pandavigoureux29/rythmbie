using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleNotesGenerator : MonoBehaviour {

	[SerializeField] protected BattleEngine m_engine;
	[SerializeField] protected BattleTracksManager m_tracksManager;

	[SerializeField] protected SongDataSO m_songData;
	[SerializeField] protected TextAsset m_songDataJSON;

	[SerializeField] protected bool m_loop = true;
	[SerializeField] protected float m_speedModifier = 1.0f;

    /// <summary>
    /// Computed time forward the music, with the timeshift needed for a note to arrive at the real elapsed time
    /// </summary>
	protected float m_computedTime = 0.0f;
    protected float m_lastTimeForward = 0.0f;
    /// <summary>
    /// Time for a note to complete its way to the end
    /// </summary>
    protected float m_timeShift = 0.0f;

	protected List< NoteData > m_notes;
	protected int m_index = 0;
	protected NoteData m_currentNote = null;
    /// <summary>
    /// Iteration of the music with time forward applied
    /// </summary>
	protected int m_iteration = 1;
	protected bool m_looper = true;

	/// <summary>
    /// Iteration increased when we spawn the last note of the list
    /// </summary>
	protected int m_notesIterations = 1;

	protected bool m_paused = true;
	protected bool m_finished = false;

	// Use this for initialization
	void Awake () {
#if !UNITY_STANDALONE && !UNITY_EDITOR
		//m_timeSynchroDelta = -0.15f;
#endif
	}

	public void Begin(float _timeShift, float _timeBegin){
        if (m_notes.Count <= 0)
            return;
        m_paused = false;
        m_timeShift = _timeShift;
        m_index = GetFirstNoteIndex(_timeBegin + _timeShift);
        m_currentNote = m_notes[m_index];
        //to ensure that notes & music are in the same iterations
        m_iteration = 1;
        m_notesIterations = 1;
    }
	
	// Update is called once per frame
	void Update () {
		if (m_paused || m_finished)
			return;

		m_computedTime = m_engine.MusicTimeElapsed + m_timeShift;
		
        //If the time elapsed is
		if (m_computedTime >= m_engine.AudioSrc.clip.length){
            //change time ( basically a modulo to begin the song if the computed time is exceeding the length of the song )
            m_computedTime -= m_engine.AudioSrc.clip.length;
            //check if we havent looped yet
            if (m_looper) {
                //Debug.Log("iteration");
				m_iteration++;
				m_looper = false;
			}
		} else {
			m_looper = true;
        }

        var debugText = GameObject.Find("DebugText").GetComponent<UnityEngine.UI.Text>();
        debugText.text = ""+ (int) m_computedTime + "  " + m_iteration;

        //if the current note time has been reached
        //we also check that we are in the same iteration
        bool nextNoteTimeReached = m_currentNote.Time <= m_computedTime;
        bool sameIteration = m_notesIterations == m_iteration;

        if ( sameIteration && nextNoteTimeReached ) {
			//Debug.Log (m_currentNote.TimeBegin + " " + m_timeElapsed + " " + m_currentNote.Type.ToString());
			//Send note
			m_tracksManager.LaunchNote(m_currentNote.Clone(),m_iteration);
			//go to next note or end
			m_index ++;
            //Next note 
			if( m_index < m_notes.Count ){
				m_currentNote = m_notes[m_index];
                //Debug.Log(" NEW NOTE " + m_currentNote.TimeBegin + " " + m_computedTime);
            //End of the loop
            }
            else{				
				m_index = 0;
				m_currentNote = m_notes [m_index];
                //Debug.Log(" FIRST NOTE " + m_currentNote.TimeBegin);
				m_notesIterations ++;
            }
		}
        
	}

    public void OnMusicLoop()
    {
    }

	public void LoadData(JSONObject _json){
		m_notes = new List<NoteData> ();
		//Debug.Log (jsonData);
		List<JSONObject> arrayNotes = _json.GetField ("notes").list;
		foreach(JSONObject noteJSON in arrayNotes){
			NoteData noteData = new NoteData();
			//Debug.Log( noteJSON);
			noteData.Type = (NoteData.NoteType) ((int)noteJSON.GetField("type").n);
			noteData.Time = noteJSON.GetField("time").f;
			noteData.Head = noteJSON.GetField("head").b;
			noteData.TrackID =(int) noteJSON.GetField("track").n;
			m_notes.Add( noteData);
		}
	}

	int GetFirstNoteIndex(float _beginTime){
		for(int i=0 ; i < m_notes.Count; i ++){
			if( m_notes[i].Time > _beginTime )
				return i;
		}
		return m_notes.Count-1;
	}
    
}
