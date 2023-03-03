﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SongEditorNote : MonoBehaviour {

	[HideInInspector][SerializeField] protected NoteData.NoteType m_type;
	[HideInInspector][SerializeField] protected float m_time = 0;
	public bool head = false;

	Transform m_transform;
	[SerializeField] SongEditorTrack m_currentTrack;

	// Use this for initialization
	void Start () {		
		m_transform = transform;
	}
	
	// Update is called once per frame
	void Update () {		
		if (m_currentTrack != null) {
			Utils.SetPositionY (Transf, m_currentTrack.WorldY);
			if (Transf.localPosition.x < m_currentTrack.Manager.StartX)
				Utils.SetLocalPositionX (Transf, m_currentTrack.Manager.StartX);
			this.time = m_currentTrack.Manager.ComputeNoteTimeByPosition (this);

		}
	}

	public void ChangeTrack( SongEditorTrack _newTrack){
		m_currentTrack = _newTrack;

		Utils.SetPositionY (Transf,m_currentTrack.WorldY);
	}

	#region CHANGE_TYPE

	//Called by Custom Inspector
	public void OnTypeChanged(){
		switch( type ){
			case NoteData.NoteType.SIMPLE : ChangeToSimple(); break;
			case NoteData.NoteType.LONG : ChangeToLong(); break;
        }
	}

	public NoteData.NoteType ToggleType(){
		NoteData.NoteType newType = NoteData.NoteType.SIMPLE;
		switch (m_type)
        {
            case NoteData.NoteType.SIMPLE: newType = NoteData.NoteType.LONG; break;
            case NoteData.NoteType.LONG : newType = NoteData.NoteType.SIMPLE; break;
		}
		m_type = newType;
		OnTypeChanged ();
		return newType;
	}

	void ChangeToLong(){
		GetComponent<SpriteRenderer>().color = Color.green;
	}

	void ChangeToSimple(){
		GetComponent<SpriteRenderer> ().color = Color.red;
	}

	#endregion


	public void Select(){
		GetComponent<SpriteRenderer>().color = Color.yellow;
	}

	public void Unselect(){
		OnTypeChanged ();
	}

	void OnDestroy(){
		m_currentTrack.RemoveNote (this);
	}

	public Transform Transf{
		get{
			if( m_transform == null ){
				m_transform = transform;
			}
			return m_transform;
		}
	}

	public float time {
		get {
			return m_time;
		}
		set {
			if( value >= 0){
				m_time = value;	
				if( m_currentTrack){
					Utils.SetLocalPositionX( Transf,m_currentTrack.Manager.ComputeNoteXByTime(m_time));					
				}			
			}
		}
	}

	public NoteData.NoteType type {
		get {
			return m_type;
		}
		set {
			m_type = value;
		}
	}

	public SongEditorTrack CurrentTrack {
		get {
			return m_currentTrack;
		}
		set {
			m_currentTrack = value;
		}
	}
}
