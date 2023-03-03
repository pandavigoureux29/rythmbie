using UnityEngine;
using System.Collections;

/// <summary>
/// This class is a prototype, not to be used on device
/// </summary>
public class BattleInputKeyboard : MonoBehaviour {

	[SerializeField] private BattleTracksManager m_tracksManager;

	[SerializeField] string m_inputAttack = "right";
	[SerializeField] string m_inputDefend = "left";
    [SerializeField] string m_inputSlide = "up";
	// Use this for initialization
	void Start () {
		if (Application.platform == RuntimePlatform.Android) {
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
        if (CheckInput(m_inputAttack, 1))
        {

        }
        else if (CheckInput(m_inputDefend, -1))
        {

        }
        else
        {
            CheckSlide();
        }
	}

	bool CheckInput( string _input, int _index){
		if (Input.GetKeyDown (_input)) {
            m_tracksManager.OnInputTriggered(_index, BattleNote.HIT_METHOD.PRESS);
			return true;
		} else if( Input.GetKeyUp(_input) )
        {
            m_tracksManager.OnInputTriggered(_index, BattleNote.HIT_METHOD.RELEASE);
			return true;
		}
		return false;
	}

    bool CheckSlide()
    {
        if(Input.GetKeyDown(m_inputSlide) ){
            int phase = m_tracksManager.PhaseState == BattleTracksManager.BattleState.ATTACK ? 1 : -1;
            m_tracksManager.OnInputTriggered(phase, BattleNote.HIT_METHOD.PRESS);
            m_tracksManager.OnInputTriggered(phase, BattleNote.HIT_METHOD.SLIDE);
            m_tracksManager.OnInputTriggered(phase, BattleNote.HIT_METHOD.RELEASE);
            return true;
        }
        return false;
    }
}
