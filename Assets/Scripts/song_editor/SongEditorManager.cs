using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Analytics;
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

	public enum Mode{ EDIT,PLAY, _COUNT };
	Mode m_mode = Mode.EDIT;

	[SerializeField] GameObject m_simpleNotePrefab;

	[SerializeField] SongEditorCamera m_camera;

	[HideInInspector][SerializeField] public float timeSpeed = 2.0f;


	bool m_bootCheck = false;

	AudioSource m_audioSource;

	SongEditorNote m_currentNote = null;

	[SerializeField] List<SongEditorNote> m_copiedNotes;
	float m_copiedBeginTime = 0;

    [SerializeField] BattleDataAsset m_battleData;
    [SerializeField] float m_timeDebugBegin;
    
    [Header("IMPORT/EXPORT")]
    [SerializeField] private SongDataSO m_songAsset;

	// Use this for initialization
	void Start () {
		BootCheck ();
		m_copiedNotes = new List<SongEditorNote> ();
        Import();
	}
	
	// Update is called once something has changed in the scene
	void Update () {

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
			
            SceneManager.LoadScene(m_battleData.sceneName);
        }*/
	}

	//Called by SongEditor
	public void OnSceneClick(Vector3 _mousePos){
		m_camera.ManualUpdate ();

		Ray ray = new Ray(_mousePos,new Vector3(0,0,1) );
		RaycastHit hit = new RaycastHit();
		bool hasHit = Physics.Raycast (ray, out hit, 1000.0f);

		if (hasHit)
		{
			var noteHit = CheckNoteHit(hit);
			//Creation
			if (noteHit == null) {
				CreateNewNote (hit, _mousePos);
				//Edition
			} else{
				if(IsDeleteKeyDown)
					DeleteNote(noteHit);
				else
					SelectNote (noteHit);
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
		string trackPressed = null;
		//if hit found
		if( _hit.collider){
			//Search in tracks to see if it's a track
			for(int i=0; i < m_tracks.Count; i++){
				if( m_tracks[i].gameObject == _hit.collider.gameObject){
					trackPressed = m_tracks[i].Id;
					break;
				}
			}	
		}

		if( trackPressed != null ){
			AddNote(trackPressed, _hit.point);
		}
	}

	SongEditorNote AddNote(string _trackID, Vector3 _position){
		if (m_bootCheck == false)
			BootCheck ();

		var track = m_tracks.FirstOrDefault(x => x.Id == _trackID);

		if (track == null)
		{
			Debug.LogError($"[IMPORT] Track of Id {_trackID} doesnt seem to be instantiated.");
		}

		GameObject go = Instantiate (m_simpleNotePrefab) as GameObject;
		go.transform.position = _position;

		SongEditorNote note = go.GetComponent<SongEditorNote> ();
		track.AddNote (note);
		SelectNote (note);

		return note;
	}
	
	SongEditorNote AddNote(string _trackID, float _time){
		if (m_bootCheck == false)
			BootCheck ();

		var track = m_tracks.FirstOrDefault(x => x.Id == _trackID);

		if (track == null)
		{
			Debug.LogError($"[IMPORT] Track of Id {_trackID} doesnt seem to be instantiated.");
		}

		var x = ComputeNoteXByTime(_time);
		var position = track.transform.position;
		position.x = x;

		return AddNote(_trackID, position);
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

	public void Export()
	{
		m_songAsset.Segments.Clear();
		
		for(int i=0; i < m_tracks.Count; i++)
		{
			var track = m_tracks[i];
			track.SortNotes();

			if (track.Notes.Count == 0)
			{
				Debug.LogError("[EXPORT] Track " + track.Id + " has not notes. Skipping.");
				continue;
			}

			//create new segment in asset
			var newSegment = new SongDataSO.SongDataSegment();
			newSegment.Id = track.Id;
			newSegment.Notes = new List<NoteData>();
			m_songAsset.Segments.Add(newSegment);
			
			//fill asset notes
			foreach (var note in track.Notes)
			{
				var newNote = new NoteData();
				newNote.Time = note.time;
				newNote.Head = note.head;
				newNote.Type = note.type;
				newNote.TrackID = track.Id;
				
				newSegment.Notes.Add(newNote);
			}
		}	
		
		Debug.Log("[IMPORT] FINISHED");
	}

	public void Import(){

		ClearAll ();

		for(int i=0; i < m_songAsset.Segments.Count; i++)
		{
			var segment = m_songAsset.Segments[i];
			var track = m_tracks[i];
			track.Id = segment.Id;
			foreach (var note in segment.Notes)
			{
				AddNote(segment.Id, note.Time);
			}
		}

	}

	#endregion

	#region MUSIC
	public void Play(){
		m_mode = Mode.PLAY;
		if (m_songAsset.Clip != null) {
			RecheckAll();

			AudioComponent.clip = m_songAsset.Clip;
			//compute playbar time
			float deltaX = m_camera.PlayBar.localPosition.x - m_startX;
			AudioComponent.timeSamples = (int)(deltaX / UnitsPerSeconds) * 120;// music.frequency;

			AudioComponent.Play();

			m_playerNotes.Play();
		}
	}
	public void Stop(){
		m_mode = Mode.EDIT;
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

	public bool IsDeleteKeyDown { get; set; } = false;

}
