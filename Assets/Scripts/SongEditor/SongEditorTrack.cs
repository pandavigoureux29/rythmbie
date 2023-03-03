using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SongEditorTrack : MonoBehaviour {
	[SerializeField] SongEditorManager m_manager;
	[SerializeField] GameObject m_notesGroup;

	[SerializeField] List<SongEditorNote> m_notes;
	[SerializeField] int m_id;
	Transform m_transform;

	public void AddNote(SongEditorNote _note){
		_note.ChangeTrack (this);
		m_notes.Add (_note);
		_note.Transf.parent = m_notesGroup.transform;
	}

	public void RemoveNote( SongEditorNote _note){
		m_notes.Remove (_note);
	}

	/*Called at least once before adding the first note
	 * This checks notes in the scene that are not stored in the list ( at boot ) */
	public void CheckNotes(){
		CleanNotes ();
		SongEditorNote[] notes = m_notesGroup.GetComponentsInChildren<SongEditorNote> ();
		for (int i = 0; i < notes.Length; i++) {
			SongEditorNote n = notes[i];
			if(! m_notes.Contains(n) )
				AddNote(n);
		}
		for (int i=0; i < m_notes.Count; i++)
			m_notes [i].ChangeTrack (this);
		SortNotes ();
	}

	/** Sort notes in the right order in case they might have been in disorder during the creation/edition */
	public void SortNotes(){
		CleanNotes ();
		List<SongEditorNote> newList = new List<SongEditorNote> ();
		while (m_notes.Count > 0) {
			float bestTime = float.MaxValue;
			int noteIndex = 0;
			for(int i=0; i < m_notes.Count; i ++){
				if( m_notes[i].time < bestTime ){
					noteIndex = i;
					bestTime = m_notes[i].time;
				}
			}
			SongEditorNote seNote = m_notes[noteIndex];
			m_notes.RemoveAt(noteIndex);
			newList.Add ( seNote );
		}
		m_notes = newList;
	}

	//Remove nulls from notes array
	void CleanNotes(){
		for(int i= m_notes.Count -1 ; i > 0; i--){
			if( m_notes[i] == null )
				m_notes.RemoveAt(i);
		}
	}

	public void ClearAll(){
		for (int i=m_notes.Count - 1; i >= 0; i--) {
			DestroyImmediate( m_notes[i].gameObject);
		}
		m_notes.Clear ();
	}

	public SongEditorNote GetNextNoteAfterTime(float _time){
		for (int i=0; i < m_notes.Count; i++) {
			if( m_notes[i].time > _time ){
				return m_notes[i];
			}
		}		 
		return null;
	}

	public List<SongEditorNote> Notes {
		get {
			return m_notes;
		}
		set {
			m_notes = value;
		}
	}

	public float WorldY{
		get{
			if( m_transform == null )
				m_transform = transform;
			return m_transform.position.y;
		}
	}

	public SongEditorManager Manager {
		get {
			return m_manager;
		}
	}

	public int Id {
		get {
			return m_id;
		}
	}
}