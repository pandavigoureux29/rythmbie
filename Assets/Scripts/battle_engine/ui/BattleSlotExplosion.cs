using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSlotExplosion : MonoBehaviour {
    
	TweenEngine.Tween m_currentLongTween;
    Transform m_transform;
    Animator m_animator;

    void Awake()
    {
        m_transform = transform;
        m_animator = GetComponent<Animator>();
    }
    	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(BattleNote _note)
    {
        Stop();

        if (_note.Type == NoteData.NoteType.SIMPLE) {
			PlaySimple();
		} else if (_note.Type == NoteData.NoteType.LONG) {
			BattleNoteLong nL = _note as BattleNoteLong;
			if( nL.IsHead ){
				PlayLong();
			}else{		
				PlaySimple();
			}
		}

	}

	void PlaySimple()
    {
        m_animator.ResetTrigger("stopTrigger");
        m_animator.SetTrigger("simpleTrigger");
    }

	void PlayLong()
    {
        m_animator.ResetTrigger("stopTrigger");
        m_animator.SetTrigger("rotateTrigger");
	}

	public void Stop()
    {
        m_animator.ResetTrigger("stopTrigger");
        m_animator.SetTrigger("stopTrigger");
        if (m_currentLongTween != null) {            
			m_currentLongTween.Stop(true);
            m_currentLongTween = null;
        }
        m_transform.localScale = new Vector3(0, 0, 1);
    }
    
    void OnTweenEnd(object _o){
        Stop();
	}
}
