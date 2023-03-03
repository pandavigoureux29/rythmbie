using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongDataSO : ScriptableObject {

	[SerializeField] protected string m_songName;

	[SerializeField] protected  List< SongDataSegment > m_segments;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public List<NoteData> GetAllNotes(){
		List<NoteData> notes = new List<NoteData> ();
		for (int i = 0; i < m_segments.Count; i ++) {
			notes.AddRange(m_segments[i].m_notes);
		}
		return notes;
	}

	[System.Serializable]
	public class SongDataSegment{
		public string name;
		public List<NoteData> m_notes;
	}
}
