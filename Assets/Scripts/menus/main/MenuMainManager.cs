using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuMainManager : MonoBehaviour {

	public string playSceneName = "mapTest";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPlayButtonClick(){
        SceneManager.LoadScene(playSceneName);
	}

	public void OnSelectionButtonClick(){
        SceneManager.LoadScene("music_select_menu");
	}

	public void OnLockerRoomClick(){
        SceneManager.LoadScene("locker_room");
	}
}
