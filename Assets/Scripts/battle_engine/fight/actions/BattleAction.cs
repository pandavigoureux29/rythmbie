using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleAction : MonoBehaviour {

    [SerializeField] protected List<BattleActionEffect> m_effects;

    [SerializeField] protected AudioClip m_mainSoundClip;
    protected AudioSource m_audioSource;

    protected DataInventoryManager.ActionData m_actionData;

    protected bool m_launched = false;
    protected bool m_dead = false;

    protected BattleActor m_caster;
    protected BattleActor m_target;

    public enum ActionType { ATTACK, SPECIAL };
    public ActionType m_type = ActionType.ATTACK;
    
    /// <summary>
    /// Damage computed in action phase ( it's not the raw power of the action, but the actual damage computed at launch). If damage < 0, it's a miss
    /// </summary>
    protected int m_damage = -1;
        
    protected float m_duration = 1.0f;

    // Use this for initialization
    virtual protected void Start () {
        m_audioSource = GetComponent<AudioSource>();
        m_effects = new List<BattleActionEffect>(this.GetComponentsInChildren<BattleActionEffect>(true));
        Die();
    }
	
	// Update is called once per frame
	virtual protected void Update () {
	}

    public void Launch(BattleActor _caster, BattleActor _target, int _damage)
    {
        gameObject.SetActive(true);
        m_launched = true;
        m_target = _target;
        m_caster = _caster;
        m_damage = _damage;
        Launch();
    }

    virtual protected void Launch()
    {
        //replace magic on the user
        /*Transform t = transform;
        float z = t.position.z;
        t.position = m_caster.transform.position;
        Utils.SetPositionZ(t, z);*/

        //Launch effect 
        var effect = GetFreeEffect();
        if (effect == null)
        {
            Debug.Log("no effect");
            return;
        }
        effect.Launch(m_caster.transform.position, m_target.transform.position);
    }

    /// <summary>
    /// Called by an effect when it has hit its target(s)
    /// </summary>
    virtual public void OnHit()
    {
        BattleFightManager.instance.RaiseActorDamageEvent(Target, m_damage);
        //play main sound if any
        if (m_mainSoundClip)
        {
            m_audioSource.clip = m_mainSoundClip;
            m_audioSource.Play();
        }
    }

    public BattleActionEffect GetFreeEffect()
    {
        for (int i = 0; i < m_effects.Count; i++)
        {
            if (!m_effects[i].IsLaunched)
            {
                return m_effects[i];
            }
        }
        return null;
    }

    virtual public void Die()
    {
        for (int i = 0; i < m_effects.Count; i++)
        {
            m_effects[i].Die();
        }
        m_launched = false;
    }

    #region PROPERTIES
    public bool IsLaunched
    {
        get
        {
            return m_launched;
        }
    }

    public BattleActor Caster
    {
        get
        {
            return m_caster;
        }
        set
        {
            m_caster = value;
        }
    }

    public BattleActor Target
    {
        get
        {
            return m_target;
        }
        set
        {
            m_target = value;
        }
    }

    public bool IsDead
    {
        get
        {
            return m_dead;
        }
    }
    
    public int Power
    {
        get
        {
            return m_actionData != null ? m_actionData.Power : 0;
        }
    }

    public float Duration
    {
        get
        {
            return m_duration;
        }
        set
        {
            m_duration = value;
        }
    }

    public int CostByUse
    {
        get
        {
            return m_actionData != null ? m_actionData.MpCost : 0;
        }
    }

    public ActionType Type
    {
        get { return m_type; }
    }

    public DataInventoryManager.ActionData ActionData
    {
        get { return m_actionData; }
        set { m_actionData = value; }
    }
    #endregion
}
