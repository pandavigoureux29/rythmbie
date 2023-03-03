using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleDamageTextManager : MonoBehaviour {

	List<TextMesh> m_texts;
	List<TextMesh> m_freeTexts;
	List<TextMesh> m_toKillTexts;

	[SerializeField] Color m_playerDamageColor;
	[SerializeField] Color m_enemyDamageColor;
	[SerializeField] Color m_cureColor;
	
	// Use this for initialization
	void Start () {
		TextMesh[] texts = GetComponentsInChildren<TextMesh> ();
		m_texts = new List<TextMesh> ();
		m_texts.AddRange (texts);
		//set free texts
		m_freeTexts = new List<TextMesh> ();
		m_freeTexts.AddRange (m_texts);

		m_toKillTexts = new List<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LaunchDamage(GameObject _go, int _value, bool _isPlayer ){
		TextMesh text = GetText ();
		if (text == null) {
			Debug.LogError("No Damage Text Found");
			return;
		}
		text.text = "" + _value;
		if (_isPlayer) {
			text.color = m_playerDamageColor;
		} else {
			text.color = m_enemyDamageColor;
		}
		LaunchText (_go, text);
	}

	void LaunchText(GameObject _go, TextMesh _text){
		//poisition text on gameobject
		Utils.SetPositionX (_text.transform, _go.transform.position.x +Random.Range (-0.8f, 0.8f));
		Utils.SetPositionY (_text.transform, _go.transform.position.y);
		//make it appear
		Utils.SetAlpha (_text,1.0f);
		//prepare and launch tween
		Vector3 dest = _text.transform.localPosition;
		dest.y += 0.5f + (float)Random.Range (0.0f, 0.2f);
		TweenEngine.TweenTransform tween =
			TweenEngine.instance.PositionTo (_text.transform, dest, 0.1f, "OnTweenTextEnded");
		tween.CallbackObject = this.gameObject;
		m_toKillTexts.Add (_text);
	}

	public void OnTweenTextEnded(object _target){
		TimerEngine.instance.AddTimer (1.0f, "KillText", gameObject);
	}

	//Called by TimerEngine after waiting
	void KillText(){
		if (m_toKillTexts.Count > 0) {
			TextMesh t = m_toKillTexts[0];
			m_toKillTexts.RemoveAt(0);
			KillText( t );
		}
	}
	
	void KillText(TextMesh _text){
		Utils.SetAlpha (_text, 0.0f);
		m_freeTexts.Add (_text);
	}

	public TextMesh GetText(){
		if (m_freeTexts.Count <= 0)
			return null;
		TextMesh res = m_freeTexts[0];
		m_freeTexts.RemoveAt (0);
		return res;
	}

}
