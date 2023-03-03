using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleInputTouchManager : SpriteTouchManager {

    public enum SLIDE
    {
        RIGHT,LEFT,DOWN,UP
    }
	
	[SerializeField] private BattleTracksManager m_tracksManager;

	[SerializeField] private BoxCollider2D m_attackCollider;
	[SerializeField] private BoxCollider2D m_defendCollider;

    /// <summary>
    /// The id of the button pressed
    /// </summary>
	int m_inputDown = -1;

	// Use this for initialization
	void Start () {
        m_tracksManager.noteEventHandler += OnNoteHit;
	}

    protected override void OnPressed(Collider2D _collider)
    {
        base.OnPressed(_collider);
        m_inputDown = (m_attackCollider == _collider) ? 1 : -1;
        m_tracksManager.OnInputTriggered(m_inputDown, BattleNote.HIT_METHOD.PRESS);
    }

    protected override void OnReleased(Collider2D _collider)
    {
        base.OnReleased(_collider);
        m_inputDown = (m_attackCollider == _collider) ? 1 : -1;
        m_tracksManager.OnInputTriggered(m_inputDown, BattleNote.HIT_METHOD.RELEASE);
    }

    protected override void OnSlidReleased(Collider2D _startCollider, Collider2D _endCollider)
    {
        base.OnSlidReleased(_startCollider, _endCollider);
        m_inputDown = (m_attackCollider == _startCollider) ? 1 : -1;
        m_tracksManager.OnInputTriggered(m_inputDown, BattleNote.HIT_METHOD.SLIDE);
    }

    void OnNoteHit(object sender, BattleTracksManager.NoteEventInfo eventInfo) {
        if (eventInfo.NextNote == null)
            return;
    }
    
}
