using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBattlePhaseTitle : MonoBehaviour {

	[SerializeField] Text m_titleRenderer;

	[SerializeField] string m_attackString;
	[SerializeField] string m_defenseString;

    float leftPositionX = -305.0f;
    float rightPositionX = 305.0f;
	Vector3 m_centerPosition;

	bool m_attack = false;

	enum State { IDLE, LEAVING, COMING };
	State m_state = State.COMING;

	// Use this for initialization
	void Start () {		
		m_centerPosition = transform.localPosition;
        rightPositionX = Screen.currentResolution.width * 0.6f;
        leftPositionX = Screen.currentResolution.width * -0.6f;
        //Utils.SetAlpha (m_titleRenderer, 0.0f);
        Utils.SetLocalPositionX (transform, leftPositionX);
	}

	public void Switch( bool _attack ){
		m_attack = _attack;
		if (m_state == State.IDLE) {
			Leave ();
		} else {
			ComeBack();
		}
	}

	void Leave(){
		m_state = State.LEAVING;
		Vector3 pos = transform.localPosition;
		pos.x = rightPositionX;
		TweenEngine.instance.PositionTo (transform, pos, 1f);
	}

	void ComeBack(){
		m_state = State.COMING;
		if (m_attack) {
			m_titleRenderer.text = m_attackString;
            m_titleRenderer.color = ColorManager.instance.GetColor("red_attack_bright");        
		} else {
			m_titleRenderer.text = m_defenseString;
            m_titleRenderer.color = ColorManager.instance.GetColor("green_defense_bright");
        }
		Utils.SetLocalPositionX (transform, leftPositionX);
		TweenEngine.instance.PositionTo (transform, m_centerPosition, 1f, "OnCentered");
    }

	void OnCentered(){
		m_state = State.IDLE;
        TimerEngine.instance.AddTimer(2.0f, "Leave", gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
