using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SongEditorManager : MonoBehaviour {
    
	[SerializeField] SongEditorPlayerNotes m_playerNotes;
	[SerializeField] List<SongEditorTrack> m_tracks;

	[SerializeField] float m_unitsPerSeconds = 0.01f;
	//[SerializeField] int m_tempo = 120;

	float m_startX = 0;

	public enum Mode{ EDIT, CREATE, DELETE, PLAY, _COUNT };
	Mode m_mode = Mode.EDIT;
	Mode m_lastMode = Mode.EDIT;

	[SerializeField] GameObject m_simpleNotePrefab;

	[SerializeField] SongEditorCamera m_camera;

	//Export Variables
	[HideInInspector][SerializeField] public AudioClip music;
	[HideInInspector][SerializeField] public string songName = "song";
	//[HideInInspector][SerializeField] public BattleEngine.Difficulty difficulty ;
	[HideInInspector][SerializeField] public float timeSpeed = 2.0f;

	//import Variables
	[HideInInspector][SerializeField] public string songImportName;
	//[HideInInspector][SerializeField] public BattleEngine.Difficulty songImportDifficulty;

	bool m_bootCheck = false;

	//SongExporter m_exporter;

	AudioSource m_audioSource;

	SongEditorNote m_currentNote = null;

	[SerializeField] List<SongEditorNote> m_copiedNotes;
	float m_copiedBeginTime = 0;

    //[SerializeField] BattleDataAsset m_battleData;
    [SerializeField] float m_timeDebugBegin;

	// Use this for initialization
	void Start () {
		BootCheck ();
		m_copiedNotes = new List<SongEditorNote> ();
        Import();
	}
	
	// Update is called once something has changed in the scene
	void Update () {
		if (m_mode == Mode.PLAY)
		{
			Debug.Log(m_audioSource.time);
		}
	}

	public void OnSceneGUI(){
		m_camera.ManualUpdate ();
		if (CurrentMode == Mode.PLAY) {
			m_playerNotes.ManualUpdate();
		}
	}

	public void OnGUI() {
		/*if (GUI.Button (new Rect (10, 10, 150, 100), "LaunchTest")) {
			TextAsset textAsset = Export("test",difficulty);
            m_battleData.Song = textAsset;
            m_battleData.TimeBegin = m_timeDebugBegin;
            DataManager.instance.BattleData = m_battleData;
			
            SceneManager.LoadScene(m_battleData.sceneName);
        }*/
	}

	//Called by SongEditor
	public void OnSceneClick(Vector3 _mousePos){
		m_camera.ManualUpdate ();

		Ray ray = new Ray(_mousePos,new Vector3(0,0,1) );
		RaycastHit hit = new RaycastHit();
		bool hasHit = Physics.Raycast (ray, out hit, 1000.0f);

		if (hasHit) {	
			//Creation
			if (m_mode == Mode.CREATE) {
				CreateNewNote (hit, _mousePos);
			//Edition
			} else if (m_mode == Mode.EDIT || m_mode == Mode.DELETE) {
				SongEditorNote note = CheckNoteHit (hit);
				if( m_mode == Mode.EDIT)
					SelectNote (note);
				else
					DeleteNote(note);
			}
		} else {
			SelectNote(null);
			m_camera.ReplacePlayBar(_mousePos);
		}
	}

	public void OnSceneRelease(Vector3 _mousePos){
	}

	#region NOTECREATION

	void CreateNewNote(RaycastHit _hit,Vector3 _mousePos){
		int trackPressed = -1;
		//if hit found
		if( _hit.collider){
			//Search in tracks to see if it's a track
			for(int i=0; i < m_tracks.Count; i++){
				if( m_tracks[i].gameObject == _hit.collider.gameObject){
					trackPressed = i;
					break;
				}
			}	
		}

		if( trackPressed >= 0 ){
			AddNote(trackPressed, _hit.point);
		}
	}

	SongEditorNote AddNote(int _trackID, Vector3 _position){
		if (m_bootCheck == false)
			BootCheck ();

		GameObject go = Instantiate (m_simpleNotePrefab) as GameObject;
		go.transform.position = _position;

		SongEditorNote note = go.GetComponent<SongEditorNote> ();
		m_tracks [_trackID].AddNote (note);
		SelectNote (note);

		return note;
	}

	#endregion

	SongEditorNote CheckNoteHit(RaycastHit hit){
		if (hit.collider) {
			return hit.collider.gameObject.GetComponent<SongEditorNote>();
		}
		return null;
	}

	#region GUI

	void OnDrawGizmos(){

	}

	#endregion gui

	#region IMPORT_EXPORT

	public TextAsset Export(string _songName, SongDifficulty _difficulty){
		RecheckAll ();
		//Export
		/*m_exporter = new SongExporter ();

		m_exporter.SetUp (this);*/

		List<List<SongEditorNote>> notesByTrack = new List<List<SongEditorNote>> ();
		//Sort Notes
		for(int i=0; i < m_tracks.Count; i++){
			m_tracks[i].SortNotes();
			notesByTrack.Add( new List<SongEditorNote>( m_tracks[i].Notes.ToArray() ) );
		}	
		List< SongEditorNote> allNotes = new List<SongEditorNote> ();
		bool done = false;
		//go through each track and get the earliest note till none is left
		while (!done) {
			int trackID = -1;
			float bestTime = float.MaxValue;
			//check each track and get best
			for( int t=0; t < notesByTrack.Count; t++){
				List< SongEditorNote> track = notesByTrack[t];
				//keep best track
				if( track.Count > 0 && track[0].time < bestTime ){
					bestTime = track[0].time;
					trackID = t;
				}
			}
			if( trackID < 0 ){
				done = true;
			}else{
				SongEditorNote bestNote = notesByTrack[trackID][0];
				notesByTrack[trackID].RemoveAt(0);
				allNotes.Add(bestNote);
			}
		}
		//now we have all notes. Set them to the exporter
		/*m_exporter.SetNotes (allNotes);
		return m_exporter.Export (_songName,_difficulty);*/
		return null;
	}

	public void Import(){
		/*string dataSongName = songImportName + "_" + songImportDifficulty.ToString ().ToLower ();
		TextAsset jsonFile = Resources.Load ("song_data/"+songImportName+"/"+dataSongName) as TextAsset;
		if (jsonFile == null) {
			Debug.LogError( "Failed to load song_data/"+songImportName+"/"+dataSongName);
			return;
		}

		ClearAll ();

		//PArse JSON
		JSONObject jsonData = new JSONObject (jsonFile.text);

		//variables
		timeSpeed = jsonData.GetField ("timeSpeed").n;

		//Create Notes
		List<JSONObject> arrayNotes = jsonData.GetField ("notes").list;
		foreach(JSONObject noteJSON in arrayNotes){
			SongEditorNote newNote = AddNote((int) noteJSON.GetField("track").n,new Vector3() );
			//Debug.Log( noteJSON);
			newNote.type = (NoteData.NoteType) ((int)noteJSON.GetField("type").n);
			newNote.time = noteJSON.GetField("time").f;
			newNote.head = noteJSON.GetField("head").b;
            newNote.OnTypeChanged();
		}

		//try loading audioClip
		music = Resources.Load ("songs/"+songImportName) as AudioClip;;*/
	}

	#endregion

	#region MUSIC
	public void Play(){
		m_lastMode = m_mode;
		m_mode = Mode.PLAY;
		if (music != null) {
			RecheckAll();

			AudioComponent.clip = music;
			//compute playbar time
			float deltaX = m_camera.PlayBar.localPosition.x - m_startX;
			//AudioComponent.timeSamples = (int) (deltaX / UnitsPerSeconds) * music.frequency;

			AudioComponent.Play();

			m_playerNotes.Play();
		}
	}
	public void Stop(){
		m_mode = m_lastMode;
		AudioComponent.Stop ();
	}
	#endregion

	#region COPY/PASTE

	//Copy all the notes from beginTime to endTime
	public void CopyNotes( float _beginTime, float _endTime){
		Debug.Log (_beginTime + " " + _endTime);
		RecheckAll ();
		m_copiedNotes.Clear ();
		m_copiedBeginTime = _beginTime;

		float currentTime = _beginTime - 0.01f;	
		float bestTime = float.MaxValue;
		SongEditorNote tempNote = null;
		SongEditorNote bestNote = null;
		while (currentTime < _endTime) {
			bestTime = float.MaxValue;
			tempNote = null;
			bestNote = null;
			for (int i=0; i < m_tracks.Count; i++) {
				tempNote = m_tracks[i].GetNextNoteAfterTime(currentTime);
				if( tempNote!=null && tempNote.time < bestTime ){
					bestTime = tempNote.time;
					bestNote = tempNote;
				}
			}
			currentTime = bestTime;
			m_copiedNotes.Add( bestNote);
		}		                                      
	}

	public void PasteNotes( float _beginTime ){
		SongEditorNote addedNote = null;
		SongEditorNote tempNote = null;
		float offsetBegin = ComputeNoteXByTime (_beginTime);
		for (int i=0; i < m_copiedNotes.Count; i++) {
			tempNote = m_copiedNotes[i];
			if( tempNote == null ){
				continue;
			}
			//compute offset of the note
			//compute new position
			Vector3 pos = tempNote.transform.position;
			pos.x = offsetBegin + ( pos.x - ComputeNoteXByTime( m_copiedBeginTime) ) ;

			addedNote = AddNote( tempNote.CurrentTrack.Id, pos);
			addedNote.type = tempNote.type;
			addedNote.head = tempNote.head;
		}
	}

	#endregion

	void SelectNote(SongEditorNote _note){
		if (m_currentNote)
			m_currentNote.Unselect ();
		if (_note)
			_note.Select ();
		m_currentNote = _note;
	}

	void DeleteNote(SongEditorNote _note){
		if (m_currentNote == _note)
			m_currentNote = null;
		if( _note != null)
		DestroyImmediate (_note.gameObject);
	}

	public float ComputeNoteTimeByPosition(SongEditorNote _note){
		float deltaX = _note.Transf.localPosition.x - m_startX;
		return deltaX / UnitsPerSeconds;
	}

	public float ComputeNoteXByTime(float _time){
		return UnitsPerSeconds * _time;
	}

	public void BootCheck(){
		m_bootCheck = true;
		for(int i=0; i < m_tracks.Count; i++){
			m_tracks[i].CheckNotes();
		}	
	}

	public void ClearAll(){
		m_currentNote = null;
		for (int i=0; i < m_tracks.Count; i++) {
			m_tracks[i].ClearAll();
		}		                                    
	}

	public void RecheckAll(){
		for (int i=0; i < m_tracks.Count; i++) {
			m_tracks[i].CheckNotes();
			m_tracks[i].SortNotes();
		}
	}

    public void Reset()
    {
        m_camera.Reset();
    }

    public void ChangeMode(){
		if (m_mode == Mode.PLAY)
			return;
		switch (m_mode) {
		case Mode.CREATE : m_mode = Mode.EDIT; break;
		case Mode.EDIT : m_mode = Mode.DELETE; break;
		case Mode.DELETE : m_mode = Mode.CREATE; break;
			
		}
	}

	public float UnitsPerSeconds {
		get {
			return m_unitsPerSeconds;
		}
		set {
			m_unitsPerSeconds = value;
		}
	}

	public float StartX {
		get {
			return m_startX;
		}
	}

	public Mode CurrentMode {
		get {
			return m_mode;
		}
	}

	public AudioSource AudioComponent {
		get {
			if( m_audioSource == null)
				m_audioSource = GetComponent<AudioSource>();
			return m_audioSource;
		}
	}

	public SongEditorNote CurrentNote{
		get{
			return m_currentNote;
		}
	}

	public List<SongEditorTrack> Tracks{
		get{
			return m_tracks;
		}		 
	}

}
