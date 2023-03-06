using UnityEngine;
using System.Collections;

[System.Serializable]
public class NoteData {

	[SerializeField] protected float m_timeBegin;
	
	[SerializeField] protected NoteType m_type = NoteType.SIMPLE;

	[SerializeField] protected string m_trackID= "";
	
	public enum NoteSubtype { REGULAR, MAGIC, NOTHING };
	[SerializeField] protected NoteSubtype m_subtype;

	/** Unused if not LONG */
	[SerializeField] protected bool m_head = true;

	/** Use to store the accuracy of a hit note. Unused outside battle */
	protected HitAccuracy m_hitAccuracy;

	public NoteData Clone(){
		NoteData noteData = new NoteData ();
		noteData.Time = this.Time;
		noteData.Type = this.Type;
		noteData.TrackID = this.TrackID;
		noteData.Subtype = this.Subtype;
		noteData.Head = this.Head;
		return noteData;
	}

	#region GETTERS_SETTERS

	public NoteType Type {
		get {
			return m_type;
		}
		set {
			m_type = value;
		}
	}

	public string TrackID {
		get {
			return m_trackID;
		}
		set {
			m_trackID = value;
		}
	}

    /// <summary>
    /// Time of the song at which the note has to be played
    /// </summary>
	public float Time {
		get {
			return m_timeBegin;
		}
		set {
			m_timeBegin = value;
		}
	}

	public bool Head {
		get {
			return m_head;
		}
		set {
			m_head = value;
		}
	}

	public NoteSubtype Subtype {
		get {
			return m_subtype;
		}
		set {
			m_subtype = value;
		}
	}
	

	public HitAccuracy HitAccuracy {
		get {
			return m_hitAccuracy;
		}
		set {
			m_hitAccuracy = value;
		}
	}
	#endregion

}
