using UnityEngine;
using System.Collections;

public class BattleEnemy : BattleActor {

	[SerializeField] Animator m_animator;

	[SerializeField] Animator m_smokeAnimator;

	// Use this for initialization
	override protected void Start () {
		base.Start ();
		m_type = ActorType.ENEMY;
        
		m_animator.Play("idle",0, Random.Range(0.0f,1.0f) );
    }    
	
	// Update is called once per frame
	override protected void Update () {
		base.Update ();
	}

	#region LOADING 
	public override void Load(string _name){
        base.Load(_name);
        var dataManager = DataManager.instance.EnemiesManager;
        var stats = dataManager.GetFullStats(_name);
        if (stats != null)
        {
            m_maxStats = new Stats(stats);
            m_currentStats = new Stats(stats);
        }
        CurrentStats.MP = 0;
    }
    #endregion

    #region ACTION

    public override void Attack(BattleActor _target, int _damage)
    {
        base.Attack(_target, _damage);

        if (Utils.IsAnimationStateRunning(m_animator, "attack"))
        {
            m_animator.Play("attack", 0, 0.0f);
        }
        else
        {
            m_animator.SetTrigger("attackTrigger");
        }
    }

    override public int GetAppliedAttackingPower(NoteData _noteData){
        return CurrentStats.Attack;
    }
		
	#endregion

	override public void TakeDamage(int _damage){
		base.TakeDamage (_damage);

		if( Utils.IsAnimationStateRunning(m_animator,"hit") ){
			m_animator.Play("hit",0,0.0f);
		}else{
			m_animator.SetTrigger ("hitTrigger");
		}

		CheckDeath ();
	}

	override protected bool Die(){
		m_smokeAnimator.SetTrigger ("explodeTrigger");
		base.Die ();
        gameObject.SetActive(false);
		return true;
	}
}
