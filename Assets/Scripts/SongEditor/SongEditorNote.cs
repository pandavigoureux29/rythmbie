using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class SongEditorNote : MonoBehaviour {

	[HideInInspector][SerializeField] protected NoteType m_type;
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
			case NoteType.SIMPLE : ChangeToSimple(); break;
			case NoteType.LONG : ChangeToLong(); break;
        }
	}

	public NoteType ToggleType(){
		NoteType newType = NoteType.SIMPLE;
		switch (m_type)
        {
            case NoteType.SIMPLE: newType = NoteType.LONG; break;
            case NoteType.LONG : newType = NoteType.SIMPLE; break;
		}
		m_type = newType;
		OnTypeChanged ();
		return newType;
	}

	void ChangeToLong(){
		GetComponentInChildren<Image>().color = Color.green;
	}

	void ChangeToSimple(){
		GetComponentInChildren<Image>().color = Color.red;
	}

	#endregion


	public void Select(){
		GetComponentInChildren<Image>().color = Color.yellow;
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

	public NoteType type {
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