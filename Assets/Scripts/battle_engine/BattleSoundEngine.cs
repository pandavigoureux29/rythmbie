using UnityEngine;
using System.Collections;

public class BattleSoundEngine : MonoBehaviour {
	
	private static BattleSoundEngine _instance;

	[SerializeField] AudioListener m_audioListener;
	[SerializeField] bool m_mute = false;

	[SerializeField] float m_mainVolume = 1.0f;

	[SerializeField] public NoteSound noteSimple;
	[SerializeField] public NoteSound noteLongTail ;

	// Use this for initialization
	void Start () {
		_instance = this;
		if( m_audioListener == null)
			m_audioListener = FindObjectOfType(typeof(AudioListener)) as AudioListener;
		if (m_mute)
			Mute ();
		else
			UnMute ();
		MainVolume = m_mainVolume;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Mute(){
		AudioListener.volume = 0;
	}

	public void UnMute(){
		AudioListener.volume = m_mainVolume;
	}

		
	public static BattleSoundEngine instance {
		get{
			if( _instance == null ){
				GameObject newGO = new GameObject("BattleSoundEngine");
				_instance = newGO.AddComponent<BattleSoundEngine>();
			}
			return _instance;
		}
	}

	public float MainVolume {
		get {
			return m_mainVolume;
		}
		set {
			m_mainVolume = value;
			AudioListener.volume = m_mainVolume;
		}
	}

	[System.Serializable]
	public class NoteSound{
		public float volume = 0.5f;
		public AudioClip clip;
	}
}
