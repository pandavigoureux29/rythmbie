using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSlot : MonoBehaviour {

	public bool m_attack = false;
	public bool m_active = true;

	[SerializeField] protected BattleTrack m_track;
	[SerializeField] protected List<BattleSlotTextAccuracy> m_textsAcc;
	[SerializeField] protected BattleSlotExplosion m_explosion;

    [SerializeField] protected float m_slideErrorDelay = 0.6f;

    /// <summary>
    /// Basically : when a colliding is that far from the slot, it dies (miss)
    /// </summary>
    [SerializeField] protected float m_noteDeltaLifeDistance = 1;

    BattleNote.HIT_METHOD m_lastInputMethod = BattleNote.HIT_METHOD.RELEASE;    
	BattleNote m_pendingNote;

	protected float m_diameter;

	/** Notes currently colliding with the slot */
	private List<BattleNote> m_collidingNotes;

	//Accuacy Text
	protected SpriteRenderer m_textSprite;
    
	// Use this for initialization
	void Start () {
		m_collidingNotes = new List<BattleNote> ();
		ComputeDiameter ();
	}
	
	// Update is called once per frame
	void Update () {
        CheckCollidingNotes();
	}

	public void Activate(){
		m_active = true;
	}

	public void Deactivate(){
		m_active = false;
		m_collidingNotes.Clear ();
	}

	public void ResetInput(){
		m_lastInputMethod = BattleNote.HIT_METHOD.NONE;
		m_pendingNote = null;
	}

	public void OnInputTriggered(BattleNote.HIT_METHOD _inputMethod){

        //var debugText = GameObject.Find("DebugText").GetComponent<UnityEngine.UI.Text>();
        //Debug.Log(_inputMethod);
        if (_inputMethod == BattleNote.HIT_METHOD.RELEASE)
            _inputMethod.ToString();

        if (m_active == false ) 
			return;
		//EXCEPTION CASES
		//if no long note is currently being hit, an error shouldn't be send ( just releasing after a hit/swipe )
		if( (_inputMethod == BattleNote.HIT_METHOD.RELEASE && ( CurrentLongNote == null && m_pendingNote == null) )){
			AbortPendingSlide ();
            Debug.Log(m_track.Id + " abort input : released but no long note / pending note");
            //debugText.text = "Error1";

            return;
		}
		//if a slide is done but no press, this is a remain from a previous track input
		if (_inputMethod == BattleNote.HIT_METHOD.SLIDE && m_lastInputMethod != BattleNote.HIT_METHOD.PRESS)
        {
            Debug.Log(m_track.Id + "abord input : slide but no press done " );
            //debugText.text = "Error2";
            return;
		}

		//else we hit the first note that has collided
		BattleNote note = WaitingForSlide ? m_pendingNote : GetFirstAliveCollidingNote();

		//if no note is colliding (miss)
		if (note == null)
        {
            Debug.Log(m_track.Id + "apply error : no note active/colliding/pending");
            //debugText.text = "Error3";
            //send an error to BattleTrack
            ApplyError(_inputMethod);
			return;
		}

        //Either it's a miss or a hit, we can clean the past note of the queue (past the slot)
        RemovePastNotes(note);

		//hit note and compute accuracy
		float accuracy = ComputeAccuracy (note);
		//set accuracy if note isn't dead
		if( !note.IsDead )
			note.Accuracy = accuracy;

		//If the note can be slid 
		if (note.CanSlide) {
			switch (_inputMethod) {
				//but it's just pressed, put it in the buffer
				case BattleNote.HIT_METHOD.PRESS:
					//Debug.Log ("press" + transform.parent.parent.name);
					LaunchPendingSlide (note);
					m_lastInputMethod = _inputMethod;
					break;
				//it's slid and the note is valid => magic
				case BattleNote.HIT_METHOD.SLIDE: 
					//launch magic
					if (m_pendingNote == note) {
                        //Debug.Log("slide " + transform.parent.parent.name + " pending:" + m_pendingNote);
						AbortPendingSlide ();
						m_track.OnNoteTriggerAction (note,this,true);
					}
					return;
				//release : the user released its touch, if any pending note is still there, it means a simple tap
				case BattleNote.HIT_METHOD.RELEASE:
					if (WaitingForSlide) {
						m_track.OnNoteTriggerAction (note, this, false);
						AbortPendingSlide ();
					}
					return;
			}
		//Classic check
		} else {
			//check input method of the note
			if (note.HitMethod != _inputMethod) {
				
				ApplyError (_inputMethod,note);
				return;
			}
		}
        
        //Debug.Log("HIT " + _inputMethod + transform.parent.parent.name + " fr" );
        //We auto remove the note if it's a long note's head ( we don't want further collisions with it )
        bool forceRemove = CurrentLongNote != null;
        m_track.OnNoteHit (note,this, forceRemove );

        //Trigger action if the current long note is on ( note tail is hit )
        if(forceRemove && _inputMethod == BattleNote.HIT_METHOD.RELEASE)
        {
            m_track.OnNoteTriggerAction(note, this, false);
        }

        m_collidingNotes.Remove(note);

		//play explosion
		m_explosion.Play (note);
	}

    #region ERROR_HANDLING

	public void ApplyError(BattleNote.HIT_METHOD _method, BattleNote _note = null)
	{
		Debug.Log ("APPLY error"+ this.gameObject.name+ " " + _method + m_lastInputMethod + " " + ( _note != null ? _note.HitMethod.ToString() : "") );

		m_track.OnInputError(_method, _note);

		//Reset
		ResetInput();
		AbortPendingSlide ();

        m_explosion.Stop();
    }

	public void LaunchPendingSlide(BattleNote _note)
    {
        //clean just in case
		TimerEngine.instance.StopAll("OnPendingSlideTimerOver", this.gameObject);
		m_pendingNote = _note;
		TimerEngine.instance.AddTimer(m_slideErrorDelay, "OnPendingSlideTimerOver", this.gameObject);
    }    

    public void OnPendingSlideTimerOver()
    {
		//if timer is over, the user didn't slide the note ( while still touching the screen ) 
		// that means he didn't attempt a slide, so we trigger a basic action
		if (WaitingForSlide)
        {
			m_track.OnNoteTriggerAction (m_pendingNote, this, false);
			AbortPendingSlide ();
        }
    }

	/// <summary>
	/// Abort pending slide if any, and call the onKill method of BattleTrack
	/// </summary>
    public void AbortPendingSlide()
    {
        m_explosion.Stop();

		if (m_pendingNote != null) {
			m_pendingNote = null;            
        }
        //clean timers
        TimerEngine.instance.StopAll("OnPendingSlideTimerOver", this.gameObject);
    }

    #endregion

    #region COLLISIONS

    void CheckCollidingNotes()
    {
        if (m_active == false)
            return;
        for(int i= m_collidingNotes.Count -1; i >= 0; --i)
        {
            var collidingNote = m_collidingNotes[i];
            //distance between the note and slot centers
            float diff = collidingNote.transform.position.x - transform.position.x;
            bool tooLong = Mathf.Abs(diff) >= m_noteDeltaLifeDistance;
            bool pastRight = tooLong && m_attack && diff > 0;
            bool pastLeft = tooLong && !m_attack && diff < 0;

            if (pastRight || pastLeft)
            {
                m_track.OnNoteMiss(collidingNote);
                m_collidingNotes.RemoveAt(i);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D _collider){
		if (m_active == false ) 
			return;
        //when a colliding note leaves the area of collision with the slot
		if( _collider.gameObject.layer == 8 ){
			BattleNote note = _collider.gameObject.GetComponent<BattleNote>();
			if( note && !m_collidingNotes.Contains(note) ){
				//Debug.Log( "Adding New note");
				m_collidingNotes.Add(note);
			}
		}
	}

	void OnTriggerExit2D(Collider2D _collider){
		if (m_active == false ) 
			return;
		if( _collider.gameObject.layer == 8 ){
			BattleNote note = _collider.gameObject.GetComponent<BattleNote>();
			if( note && note.IsHittable && m_collidingNotes.Contains(note)){
				m_track.OnNoteMiss(note);
                m_explosion.Stop();
				m_collidingNotes.Remove(note);
			}
		}
	}

	BattleNote GetFirstAliveCollidingNote(){
        //here we 'll try to find the first colliding note, but also the note that has the best accuracy
        //that is to say, the note that is more collinding with the slot
        float bestAccuracy = float.MinValue;
        BattleNote bestNote = null;
		foreach (var note in m_collidingNotes) {
            var acc = ComputeAccuracy(note);
            if (!note.IsDead && acc > bestAccuracy) {
                bestAccuracy = acc;
                bestNote = note;
			}
		}
		return bestNote;
	}

    /// <summary>
    /// Checks all the notes and remove the ones that are past the one given in parameter.
    /// Best called when a note is hit/missed to clean the queue.
    /// </summary>
    void RemovePastNotes(BattleNote _note)
    {
        for (int i = m_collidingNotes.Count - 1; i >= 0; --i)
        {
            var note = m_collidingNotes[i];
            if (note != CurrentLongNote && note.Data.Time < _note.Data.Time)
            {
                m_collidingNotes.RemoveAt(i);
                m_track.OnNoteMiss(note);
            }
        }
    }

    #endregion

    public void PlayTextAccuracy(HitAccuracy _accuracy){
		//Find dead text
		for (int i=0; i < m_textsAcc.Count; i++) {
			if( m_textsAcc[i].IsAvailable ){
				m_textsAcc[i].Play(_accuracy);
				return;
			}
		}
	}

	float ComputeAccuracy(BattleNote _note){
		//distance between the note and slot centers
		float diff = _note.transform.position.x - transform.position.x;
		float delta = Mathf.Abs( diff );

		//add accuracy for notes past the slot. This prevents illusions for the eye because of the speed
		bool attacking = m_track.TracksManager.IsAttacking;
		if( (attacking && diff > 0) || ( !attacking && diff < 0) ){
			delta = delta - (m_diameter * 0.1f );
		}

		//compute accuracy
		float percent = delta / m_diameter;

		return 100 - (percent * 100);
	}
    ///<summary>
	/// Compute Accuracy Values 
    /// </summary>
	void ComputeDiameter(){		
		CircleCollider2D coll = GetComponent<CircleCollider2D> ();
		m_diameter = coll.radius * transform.localScale.x * 2 ;
	}

    public BattleNoteLong CurrentLongNote
    {
        get { return m_track.CurrentLongNote; }
    }

	public bool WaitingForSlide{
		get{
			return m_pendingNote != null;
		}
	}
}
