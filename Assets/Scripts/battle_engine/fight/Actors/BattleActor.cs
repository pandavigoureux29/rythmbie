using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleActor : MonoBehaviour {

    //Main Sprite
    [SerializeField] protected SpriteRenderer m_sprite;
    //UI
    [SerializeField] protected UIBattleLifeBar m_lifeGauge;
    [SerializeField] protected UIBattleLifeBar m_manaGauge;

    [SerializeField] protected Transform m_attacksGroup; 

    public enum State{ IDLE, ATTACKING, DEFENDING, HIT, DEAD };
	protected State m_state = State.IDLE;

	public enum ActorType { CHARACTER, ENEMY };
	protected ActorType m_type = ActorType.CHARACTER;

	//Battle Fight references
	protected BattleFightManager m_fightManager;
    
    //STATS
    protected Stats m_currentStats = new Stats();
    protected Stats m_maxStats = new Stats();

    //MOVES
    protected BattleAction m_attack;
    protected List<BattleMagic> m_magics = new List<BattleMagic>();
    protected BattleMagic m_currentMagic = null;
    
    protected bool m_dead = false;

	protected Transform m_transform;

    void Awake()
    {
        Init();
    }

	// Use this for initialization
	virtual protected void Start ()
    {
        //Transform
		m_transform = transform;
		RefreshLifeGauge();
		RefreshManaGauge();
    }

    void Init()
    {
        m_magics = new List<BattleMagic>(2) { null, null };
        //find fight manager and ui stuff
        m_fightManager = BattleFightManager.instance;

		//ui gauges ( when instanciating the actors, it hasnt a parent yet )
		if ( (m_lifeGauge == null || m_manaGauge == null) && transform.parent != null )
		{
			var bars = transform.parent.GetComponentsInChildren<UIBattleLifeBar>();
			foreach (var bar in bars)
			{
				if (bar.IsMana)
					m_manaGauge = bar;
				else
					m_lifeGauge = bar;
			}
		}
    }

    virtual public void Load(string _name){
        Init();
        m_transform = transform;
    }
	
	// Update is called once per frame
	virtual protected void Update () {
		switch (m_state) {
		    case State.ATTACKING : UpdateAttacking(); break; 
		}
	}
    
	#region UPDATES
	
	virtual protected void UpdateAttacking(){
	}
	
	#endregion
	
	#region ACTIONS

    virtual public void Attack(BattleActor _target, int _damage)
    {
        m_state = State.ATTACKING;
        //launch prefab attack
        if (m_attack)
            m_attack.Launch(this, _target, _damage);
        _target.TakeDamage(_damage);
        BattleFightManager.instance.RaiseActorDamageEvent(_target, _damage);
    }
	
	virtual public int GetAppliedAttackingPower( NoteData _noteData ){
		m_state = State.ATTACKING;
		return CurrentStats.Attack;
	}

	virtual public void OnAttackEnded(){
		this.m_state = State.IDLE; 
	}

	virtual public void TakeDamage(int _damage){
		if (_damage < 0)
			_damage = 0;
		CurrentStats.HP -=  _damage;
		RefreshLifeGauge ();

		//Notify manager if dead
//		bool dead = false;
		if (CurrentStats.HP <= 0) {
			Die ();
		}
	}

	virtual public int TakeMagicDamage( int _damage, BattleMagic _magic){

		_damage -= CurrentStats.Magic ;
		if (_damage < 0)
			_damage = 0;
		CurrentStats.HP -=  _damage;
		RefreshLifeGauge ();
		
		//Notify manager if dead
		CheckDeath ();
		
		return _damage;
	}

	/** Adds MP and return true if full */
	protected bool AddMP(int _mp){
		//TODO
		if (isCasting)
			return false;
		bool full = false;
		CurrentStats.MP += _mp;
		if( CurrentStats.MP >= MaxStats.MP){
			CurrentStats.MP = MaxStats.MP;
			full = true;
		}
		RefreshManaGauge ();
		return full;
	}

	virtual protected void CheckDeath(){
		if ( m_dead == false && CurrentStats.HP <= 0) {
			Die ();
		}
	}

	virtual protected bool Die(){
		m_dead = true;
        BattleFightManager.instance.OnActorDead(this);
		return true;
	}
	
	#endregion

	#region MAGIC

    /// <summary>
    /// Launches the magic on the target
    /// </summary>
	virtual public BattleMagic LaunchMagic(BattleActor _target, int _damage, bool _offensive){
        Debug.Log("LAUNCH MAGIC");
        m_currentMagic = GetMagic(_offensive);
        if (m_currentMagic == null)
            return null;
        
        m_currentMagic.Launch (this, _target,_damage);
        
        //drain mp
        CurrentStats.MP -= m_currentMagic.CostByUse;
        RefreshManaGauge ();
        return m_currentMagic;
	}    

    /// <summary>
    /// Called when the magic is dismissed
    /// </summary>
	virtual public void OnDismissMagic(){
		RefreshManaGauge ();
		m_currentMagic.Die ();
	}

    public BattleMagic GetMagic(bool _attacking)
    {
        if( _attacking)
        {
            return m_magics[1];
        }
        return m_magics[0];
    }

    protected void AddAttack(string _weaponId)
    {
        if (!string.IsNullOrEmpty(_weaponId))
        {
            //get weapon prefab
            GameObject go = DataManager.instance.CharacterManager.LoadAttackPrefab(_weaponId);
            if (go != null)
            {
                go.transform.SetParent(m_attacksGroup, false);
                m_attack = go.GetComponent<BattleAction>();
            }
        }
    }

    protected void AddMagic(string _magicId)
    {
        var inventory = DataManager.instance.InventoryManager;
        if (!string.IsNullOrEmpty(_magicId))
        {
            var magicData = inventory.GetMagicActionData(_magicId);
            var pathToPrefab = "prefabs/battle/" + "magic/" + magicData.Prefab;
            //Load prefab
            var go = Instantiate(Resources.Load(pathToPrefab)) as GameObject;
            var magic = go.GetComponent<BattleMagic>();
            magic.ActionData = magicData;
            if (go != null)
            {
                go.transform.SetParent(m_attacksGroup, false);
                if(magicData.Offense)
                    m_magics[1] = magic;
                else
                    m_magics[0] = magic;
            }
        }
    }

	#endregion

	#region UI
	protected void RefreshLifeGauge(){
		if (m_lifeGauge == null)
			return;

		float hpPercent = MaxStats.HP == 0 ? 0 : (float)CurrentStats.HP / (float)MaxStats.HP;
		m_lifeGauge.SetValue( hpPercent );
	}

	protected void RefreshManaGauge(){
		if (m_manaGauge == null)
			return;

		float mpPercent = MaxStats.MP == 0 ? 0 : (float)CurrentStats.MP / (float)MaxStats.MP;
		m_manaGauge.SetValue( mpPercent );
	}

    public void RefreshUI()
    {
        RefreshLifeGauge();
        RefreshManaGauge();
    }

    #endregion ui

    

    #region PROPERTIES
    public ActorType Type {
		get {
			return m_type;
		}
	}
    
	public bool isCasting{
		get{
			return m_currentMagic != null && m_currentMagic.IsLaunched;
		}
	}

	public bool isDead{
		get{
			return m_dead;
		}
	}

	public SpriteRenderer MainSprite{
		 get{
			return m_sprite;
		}
	}

	public BattleMagic CurrentMagic {
		get {
			return m_currentMagic;
		}
	}

    protected Stats MaxStats
    {
        get
        {
            return m_maxStats;
        }
    }

    public Stats CurrentStats
    {
        get
        {
            return m_currentStats;
        }        
    }

    public BattleAction AttackAction
    {
        get { return m_attack; }
    }
    #endregion
}
