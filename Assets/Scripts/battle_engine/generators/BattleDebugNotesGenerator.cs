using UnityEngine;
using System.Collections;

public class BattleDebugNotesGenerator : BattleNotesGenerator {

	public float m_deltaTimeSpawn;
	public float m_speed;

	public NoteData.NoteType m_mainType = NoteData.NoteType.SIMPLE;

	protected float m_nextTimeSpawn = 0.0f;

	void Awake(){
	}

	// Use this for initialization
	void Start () {
		m_nextTimeSpawn = m_deltaTimeSpawn;
	}
	
	// Update is called once per frame
	void Update () {
		
		m_computedTime += Time.deltaTime;

		if (m_computedTime >= m_nextTimeSpawn) {
			m_nextTimeSpawn += m_deltaTimeSpawn;
			GenerateNote();
		}
	}

	void GenerateNote(){
		NoteData data = new NoteData ();
		data.Type = m_mainType;
		m_tracksManager.LaunchNote (data,0);
	}
}
