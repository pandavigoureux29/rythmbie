using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Song", menuName = "SongData/Song")]
public class SongDataSO : ScriptableObject {

	[SerializeField] protected string m_songName;
	[SerializeField] protected GameDifficulty m_difficulty;
	[SerializeField] protected AudioClip m_music;

	[SerializeField] protected  List< SongDataSegment > m_segments;

	public List<NoteData> GetAllNotes(){
		List<NoteData> notes = new List<NoteData> ();
		for (int i = 0; i < m_segments.Count; i ++) {
			notes.AddRange(m_segments[i].m_notes);
		}
		return notes;
	}

	[System.Serializable]
	public class SongDataSegment{
		public string Id;
		public List<NoteData> m_notes;
	}

	public string SongName => m_songName;

	public GameDifficulty Difficulty => m_difficulty;
	
	public AudioClip Clip => m_music;
	
	public List< SongDataSegment > Segments
	{
		get { return m_segments; }
		set { m_segments = value; }
	}  
}
