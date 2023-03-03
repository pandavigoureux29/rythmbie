using UnityEngine;
using System.Collections;

public class BattleNote : MonoBehaviour {
	protected NoteData m_data;
	protected float m_speed;

	public enum State { LAUNCHED, HIT, MISS, DEAD };
	protected State m_state = State.DEAD;

    public enum HIT_METHOD { PRESS, RELEASE, SLIDE, NONE };

    private bool m_offensive = false;
    
	[SerializeField] protected HIT_METHOD m_hitMethod = HIT_METHOD.PRESS;

	[SerializeField] protected NoteData.NoteType m_type = NoteData.NoteType.SIMPLE;

	[SerializeField] protected Sprite m_attackSprite;
	[SerializeField] protected Sprite m_defendSprite;
	
	/** Sprite that follow the note if it is a magic note */
	[SerializeField] protected SpriteRenderer m_magicEffect;

	protected BattleTrack m_track;

	//references to components used in updates
	[SerializeField] protected SpriteRenderer m_renderer;
    protected Animator m_animator;
    protected Transform m_transform;
    
    /// <summary>
    /// Distance done by the note from its starting point 
    /// </summary>
    protected float m_distanceDone = 0;

	public float Accuracy{ get; set; }
    
    /// <summary>
    /// Distance where the alpha of the note will reach 1.Of
    /// </summary>
    [SerializeField] protected float m_alphaDist = 3.0f;

	protected Vector3 m_startPos;
    protected float m_startTime;
    protected float m_direction = 1.0f;

	/// <summary>
	/// The note is on a track that has been disabled
	/// </summary>
	protected bool m_isFinal = false;

	protected bool m_paused = false;

	protected bool m_canSlide = true;

	// Use this for initialization
	virtual protected void Start () {
		m_transform = transform;
        m_animator = GetComponent<Animator>();
		Die ();
	}
	
	// Update is called once per frame
	virtual protected void Update () {
		if (m_paused)
			return;
		switch (m_state) {
			case State.LAUNCHED : 
					UpdateSpeed();
					UpdateAlpha();
				break;
		}
	}

	#region UPDATES

	protected void UpdateSpeed(){
		Vector3 pos = m_transform.localPosition;

		//compute note position
		pos.x = ComputePosition();

		//compute total distance done
		m_distanceDone += Mathf.Abs( pos.x - m_transform.localPosition.x );

		m_transform.localPosition = pos;

		//Effect
		if (m_magicEffect != null && m_data.Subtype == NoteData.NoteSubtype.MAGIC) {
			m_magicEffect.transform.position = m_transform.position;
			Utils.SetPositionZ( m_magicEffect.transform, m_transform.position.z +1);
		}
	}

	protected void UpdateAlpha(){
		//compute alpha from the beginning
		float newAlpha = (m_distanceDone / m_alphaDist) * 1.0f;
		Utils.SetAlpha (m_renderer, newAlpha);

		//Apply on Magic Effect
		if (m_magicEffect != null && m_data.Subtype == NoteData.NoteSubtype.MAGIC) {
			Utils.SetAlpha( m_magicEffect,newAlpha );
		}
	}

    float ComputePosition()
    {
        //time of the music
        float t = BattleEngine.instance.MusicTimeElapsed;
        //difference betwen target time and start time
        float percent = (t - m_startTime) / (Data.Time - m_startTime) ; //(t - ti) / (tf - ti)
        //total distance to go
        float d = m_track.Length;

        float x = m_startPos.x + m_direction * ( d * percent );
        return x;
    }
    
	#endregion

	public bool Launch( Vector3 _startPos, Vector3 _targetPos, BattleTrack _track){	
		m_track = _track;
		transform.position = _startPos;
		m_startPos = m_transform.localPosition;
		m_distanceDone = 0;
        m_startTime = BattleEngine.instance.MusicTimeElapsed;
        //direction
        m_direction = (_targetPos.x - _startPos.x) > 0 ? 1 : -1;
		return Launch ();
	}

	virtual protected bool Launch()
    {
        IsFinal = false; //failsafe : a note cannot be launched on a disabled track, so cant be launched as final
        this.CurrentState = State.LAUNCHED;
		//set sprite & color
		if (m_track.TracksManager.IsAttacking) {
			m_renderer.sprite = m_attackSprite;
		} else {
			m_renderer.sprite = m_defendSprite;
		}
		//magic note
		if (m_data.Subtype == NoteData.NoteSubtype.MAGIC) {
			//
		}

		Utils.SetAlpha (m_renderer, 0.0f);
		return true;
	}

	/** Hit the note */
	virtual public BattleNote[] Hit(BattleSlot _slot){
		this.CurrentState = State.HIT;
        m_animator.SetTrigger("hit");
        return new BattleNote[] { this };
    }

    /// <summary>
    /// Makes a note miss. Return the notes affected by this action ( ie head and tail for long notes )
    /// </summary>
	virtual public BattleNote[] Miss(){
		this.CurrentState = State.MISS;
        m_animator.enabled = true;
        m_animator.SetTrigger("die");
        //some notes (like long notes) needs to return several notes when then are missed
        return new BattleNote[] { this };
	}

	/// <summary>
    /// Makes the note die. Return the notes affected by this action ( ie head and tail for long notes )
    /// Called from animation event
    /// </summary>
	virtual public BattleNote[] Die(){
        //Debug.Log("DIE" + ( Data!=null ? Data.TimeBegin.ToString() : "nodata") + Utils.IsAnimationStateRunning(m_animator,"die") );
		this.CurrentState = State.DEAD;
        //m_animator.SetTrigger("stop");
        IsFinal = false;
		Utils.SetLocalPositionY (m_transform,-10000);
		Utils.SetAlpha (m_renderer, 0.0f);
		if (m_magicEffect)
			Utils.SetAlpha (m_magicEffect, 0.0f);
		return new BattleNote[] { this }; ;
	}

	public void Pause(){
		if (m_state == State.LAUNCHED) {
			m_paused = true;
		}
	}

	public void Resume(){
		m_paused = false;
	}

	void EnableMagicEffect(){

	}

    public void PlayHit()
    {
        m_animator.SetTrigger("hit");
    }

    public virtual void ChangeHitMethod(HIT_METHOD _method)
    {
        m_hitMethod = _method;
    }

	#region GETTERS_SETTERS

	public NoteData.NoteType Type {
		get {
			return m_type;
		}
	}

	public BattleNote.State CurrentState {
		get {
			return m_state;
		}
		set {
			m_state = value;
		}
	}

	virtual public bool IsOnTrack {
		get{
			return m_state == State.LAUNCHED;
		}
	}

	virtual public bool IsHittable {
		get{
			return m_state == State.LAUNCHED;
		}
	}

	public bool IsHit{
		get{ return m_state == State.HIT; }
	}

	public bool IsDead{
		get{
			return m_state == State.DEAD && Utils.IsAnimationStateRunning(m_animator, "idle",false);
		}
	}

	public HIT_METHOD HitMethod {
		get {
			return m_hitMethod;
		}
	}

	public float Speed{
		get{
			return m_speed;
		}
	}

	virtual public BattleNote GetPairNote(){
		return null;
	}

	public NoteData Data{
		get{
			return m_data;
		}
		set{
			m_data = value;
		}
	}

	/// <summary>
	/// Tells if the note is a final note of the track. This means the tracks has been disabled but the note still can be hit
	/// </summary>
	public bool IsFinal {
		get {
			return m_isFinal;
		}
		set {
			m_isFinal = value;
		}
	}

	public bool CanSlide{
		get{
			return m_canSlide;
		}
		set{
			m_canSlide = value;
		}
	}

    public bool Offensive
    {
        get { return m_offensive; }
        set { m_offensive = value; }
    }

    public Animator Animator { get { return m_animator; } }

	#endregion
}
