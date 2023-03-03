using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DebugFirstScene : MonoBehaviour {

	public static DebugFirstScene instance = null;

	bool m_launched = false;
	bool m_generatorFound = false;

	float m_btnSize;
    
	void Awake(){
		m_btnSize = Screen.width * 0.1f;
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (m_launched) {
			if( m_generatorFound == false){
				BattleNotesGenerator gen = FindObjectOfType( typeof( BattleNotesGenerator) ) as BattleNotesGenerator;
				if( gen != null ){
					m_generatorFound = true;
				}
			}
		}
	}

	void OnGUI(){
		if (m_launched == true) {
			if (GUI.Button (new Rect (0, 0, m_btnSize, m_btnSize), "<")) {
				Reboot ();
			}
		} else {
			if (GUI.Button (new Rect (Screen.width - m_btnSize*2f, Screen.height - m_btnSize*2f, m_btnSize*2f, m_btnSize*2f), "Launch")) {
				LaunchScene ();
			}                                    
		}
	}

	void LaunchScene(){
		m_launched = true;
        SceneManager.LoadScene("test");
	}

	public void Reboot(){
		m_launched = false;
		m_generatorFound = false;
        SceneManager.LoadScene("debugFirstScene");
	}
}
