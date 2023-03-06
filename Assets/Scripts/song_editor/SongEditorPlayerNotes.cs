using UnityEngine;
using System.Collections;

public class SongEditorPlayerNotes : MonoBehaviour {

	[SerializeField] SongEditorManager m_manager;
	[SerializeField] AudioClip m_noteHitSound;
	[SerializeField] AudioClip m_noteLongTailSound;

	SongEditorNote m_currentNote = null;
	
	AudioSource m_audioSource;

	// Use this for initialization
	void Start () {
	
	}

	public void Play(){
		if (m_audioSource == null) {
			m_audioSource = GetComponent<AudioSource>();
			m_audioSource.clip = m_noteHitSound;
		}
		m_currentNote = null;
		m_currentNote = GetNextNote ();
	}
	
	// Update is called once per frame
	public void ManualUpdate () {
		//Debug.Log (m_currentNote.time + " " + m_manager.AudioComponent.time);
		if( m_manager.CurrentMode == SongEditorManager.Mode.PLAY ){
			if( m_currentNote && m_currentNote.time <= m_manager.AudioComponent.time ){
				PlayNote();
				m_currentNote = GetNextNote ();
			}
		}	
	}

	void PlayNote(){
		if (m_currentNote.type == NoteType.LONG && m_currentNote.head == false) {
			m_audioSource.clip = m_noteLongTailSound;
		} else {
			m_audioSource.clip = m_noteHitSound;
		}
		m_audioSource.Play ();
	}

	SongEditorNote GetNextNote(){
		float bestTime = float.MaxValue;
		SongEditorNote note = null;
		float time = m_manager.AudioComponent.time + 0.001f;
		if (m_currentNote)
			time = m_currentNote.time+ 0.001f;

		for (int i=0; i < m_manager.Tracks.Count; i++) {
			SongEditorNote tmpNote = m_manager.Tracks[i].GetNextNoteAfterTime(time);

			if( tmpNote && tmpNote.time < bestTime ){
				note = tmpNote;
				bestTime = tmpNote.time;
			}
			                          
		}		                   
		return note;
	}
}
