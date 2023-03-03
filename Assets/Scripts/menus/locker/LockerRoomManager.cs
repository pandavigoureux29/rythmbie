using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LockerRoomManager : MonoBehaviour {

	[SerializeField] SpriteRenderer m_bodySprite;
	[SerializeField] SpriteRenderer m_arm1Sprite;
	[SerializeField] SpriteRenderer m_arm2Sprite;
	[SerializeField] SpriteRenderer m_eyelidSprite;
	[SerializeField] GameObject m_clothesGO;

	//color Manager
	[SerializeField] Image m_colorObject;
	[SerializeField] List<Color> m_bodyColors;
	int m_currentColor = 0;

	//eyelids
	[SerializeField] Text m_eyelidIDText;
	[SerializeField] Transform m_eyelidsPool;
	int m_currentEyelid;

	// Use this for initialization
	void Start () {
		Load ();
		SetCurrentColor ();
		SetCurrentEyelid ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Quit(){
		//Set color of the player
		JSONObject colorObject = new JSONObject ();
		Color c = m_bodyColors [m_currentColor];
		colorObject.Add (c.r);
		colorObject.Add (c.g);
		colorObject.Add (c.b);
		colorObject.Add (m_currentColor);
		DataManager.instance.GameData.SetField ("playerColor", colorObject );
		//EYELID
		string eyeName = m_eyelidsPool.GetChild (m_currentEyelid).name;
		if (eyeName == "none")
			eyeName = null;
		DataManager.instance.GameData.SetField ("playerEyelid",eyeName);
		//SAVE & QUIT
		DataManager.instance.SaveGameData ();
        SceneManager.LoadScene("battle_scene");
	}

	void Load(){
		//COLOR
		JSONObject colorObject = DataManager.instance.GameData.GetField ("playerColor");
		if (colorObject) {
			m_currentColor = (int) colorObject[3].n;
		}
		//EYELID
		JSONObject eyelidObject = DataManager.instance.GameData.GetField ("playerEyelid");
		if (eyelidObject) {
			for (int i=0; i < m_eyelidsPool.childCount; i++) {
			 	if( m_eyelidsPool.GetChild(i).name == eyelidObject.str ) {
					m_currentEyelid = i;
					break;
				}
			}
		}
	}

	#region COLOR

	public void NextColor(){
		if (m_currentColor < m_bodyColors.Count-1) {
			m_currentColor ++;
			SetCurrentColor();
		}
	}

	public void PreviousColor(){
		if (m_currentColor > 0) {
			m_currentColor --;
			SetCurrentColor();
		}
	}

	void SetCurrentColor(){		
		Color c = m_bodyColors [m_currentColor];
		m_colorObject.color = c;
		m_bodySprite.color = c;
		m_arm1Sprite.color = c;
		m_arm2Sprite.color = c;
		SetEyelidsColor ();
	}

	#endregion

	#region EYELIDS

	public void NextEyelid(){
		if (m_currentEyelid < m_eyelidsPool.childCount -1) {
			m_currentEyelid ++;
			SetCurrentEyelid();
		}
	}

	public void PreviousEyelid(){
		if (m_currentEyelid > 0) {
			m_currentEyelid --;
			SetCurrentEyelid();
		}
	}

	void SetCurrentEyelid(){
		m_eyelidIDText.text = "" + m_currentEyelid;
		for (int i=0; i < m_eyelidsPool.childCount; i++) {
			m_eyelidsPool.GetChild(i).gameObject.SetActive(i == m_currentEyelid) ;
		}
	}

	void SetEyelidsColor(){
		for (int i=0; i < m_eyelidsPool.childCount; i++) {
			m_eyelidsPool.GetChild(i).GetComponent<SpriteRenderer>().color = m_bodyColors[m_currentColor];
		}
	}

	#endregion
	
}
