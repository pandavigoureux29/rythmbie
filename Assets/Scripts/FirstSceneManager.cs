using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FirstSceneManager : MonoBehaviour {
	
	[SerializeField] Text m_text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (DataManager.instance.IsLoaded) {
			SceneManager.LoadScene( "main_menu" );
		}
	}
	
	public void Quit(){
        SceneManager.LoadScene("locker_room");
	}
	
	public void DeleteAndReset(){		
		PlayerPrefs.DeleteAll ();
        SceneManager.LoadScene("first_scene");
	}

}
