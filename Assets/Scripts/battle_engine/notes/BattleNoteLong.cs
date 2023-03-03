using UnityEngine;
using System.Collections;

public class BattleNoteLong : BattleNote {
	
	[SerializeField] protected Sprite m_blankSprite;

	[SerializeField] protected bool m_isHead = false;
	[SerializeField] protected BattleNoteLong m_pairNote;
	[SerializeField] protected SpriteRenderer m_bodySprite;

    /// <summary>
    /// Distance where the alpha of the body will reach 1.Of
    /// </summary>
    [SerializeField] protected float m_bodyAlphaDist = 1.0f;

	protected float m_bodyScaleMultiplier = 1.0f;
	protected Transform m_bodyTransform;

	// Use this for initialization
	override protected void Start () {
		m_canSlide = false;
		m_bodyScaleMultiplier = m_bodySprite.sprite.bounds.extents.x * 2;
		m_bodyTransform = m_bodySprite.gameObject.transform;
		base.Start ();
		m_transform.parent.position = new Vector3 ();
	}
	
	// Update is called once per frame
	override protected void Update () {
		base.Update ();
		if (!m_paused && IsHead) {
			UpdateBody();
		}
	}

	/** make the body follow the head */
	void UpdateBody(){
        //neither the head nor the tail is hittable
        if ( !IsHittable && !m_pairNote.IsHittable )
            return;
		float deltaX;
		Vector3 tmpVector = m_startPos;
		if (m_pairNote.CurrentState == State.LAUNCHED) {
			tmpVector = m_pairNote.transform.localPosition;
		}
		//diff between current pos and start pos of the note
		deltaX = m_transform.localPosition.x - tmpVector.x;
		//Change position
		Utils.SetLocalPositionX( m_bodyTransform, m_transform.localPosition.x - deltaX * 0.5f);
		//Change scale
		deltaX = Mathf.Abs (deltaX);
        deltaX += deltaX * m_bodyScaleMultiplier;

        Utils.SetLocalScaleX( m_bodyTransform, deltaX);
        //compute alpha from the beginning
        float newAlpha = (m_distanceDone / m_bodyAlphaDist) * 1.0f;
        Utils.SetAlpha (m_bodySprite, newAlpha);
	}

	#region ACTIONS

	//Hit : If HEAD place in slot center
	//     If TAIL , send second hit and Die
	override public BattleNote[] Hit(BattleSlot _slot){
		this.CurrentState = State.HIT;
        if (IsHead) {
			Utils.SetPositionX(m_transform, _slot.transform.position.x);
        }
        else
        {
            m_pairNote.TriggerHitAnimation();
            this.Die();
        }
        //if the tail is hit, the two notes can be removed from the track
        if( !IsHead )
            return new BattleNote[] { this, m_pairNote };
        return new BattleNote[0];
    }

	// Miss: If HEAD, kill itself and tail ( if launched )
	//		If TAIL : kill itself and head
	override public BattleNote[] Miss(){
		this.CurrentState = State.MISS;
        //Notify other
        if (IsHead)
        {
            Utils.SetAlpha(m_bodySprite, 0.0f);
        }
        if( m_pairNote.CurrentState != State.MISS)
        {
            m_pairNote.Miss();
        }
        m_animator.enabled = true;
        m_animator.SetTrigger("die");

        return new BattleNote[]{this, m_pairNote};
	}

    /// <summary>
    /// Makes the note die. Return the notes affected by this action ( ie head and tail for long notes )
    /// </summary>
    override public BattleNote[] Die(){
		this.CurrentState = State.DEAD;
        //hide body
		if (IsHead) {
			Utils.SetAlpha (m_bodySprite, 0.0f);
			Utils.SetLocalPositionY(m_bodyTransform,-10000);
        }
        //Hide note
        Utils.SetLocalPositionY(m_transform,-10000);
		Utils.SetAlpha (m_renderer, 0.0f);
        return new BattleNote[] { this, m_pairNote };
    }

	#endregion

	#region LAUNCH

	override protected bool Launch(){		
		//if this==TAIL and head is dead, do not launch
		if (IsHead == false && m_pairNote.IsDead) {
			Die ();
			return false;
        }

        this.CurrentState = State.LAUNCHED;
		Utils.SetLocalPositionY (m_bodyTransform, m_transform.localPosition.y);

        //color
        string colorName = m_track.TracksManager.IsAttacking ? "red_attack_bright" : "green_defense_bright";
        Color color = ColorManager.instance.GetColor(colorName);

        //set sprite
        if ( m_isHead )
        {
            UpdateBody();
            m_bodySprite.color = color;
            Utils.SetAlpha(m_bodySprite, 0.0f);
			m_renderer.sprite = m_track.TracksManager.IsAttacking ? m_attackSprite : m_defendSprite;
		} else {
            m_renderer.color = color;
        }

        m_renderer.color = color;
        Utils.SetAlpha (m_renderer, 0.0f);
		return true;
	}
	#endregion

    public void TriggerHitAnimation()
    {
        m_animator.SetTrigger("hit");
    }

	public bool IsHead {
		get {
			return m_isHead;
		}
	}

	public BattleNoteLong TailNote{
		get{
			if( IsHead ){
				return m_pairNote;
			}
			return null;
		}
	}

	override public bool IsOnTrack {
		get{
			return m_state == State.LAUNCHED || ( m_state == State.HIT && IsHead );
		}
	}
}
