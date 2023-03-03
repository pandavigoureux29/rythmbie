using UnityEngine;
using System.Collections;

public class BattleCharacter : BattleActor {

	[SerializeField] BattleCharacterAnimator m_charAnimator;
    [SerializeField] CharacterBuild m_build;

    string m_charId = null;

	override protected void Start () {
		base.Start ();
		m_type = ActorType.CHARACTER;

        m_lifeGauge.ChangeOrientation(LR.UI.ORIENTATION.HORIZONTAL, LR.UI.ALIGN.LEFT);
        m_manaGauge.ChangeOrientation(LR.UI.ORIENTATION.HORIZONTAL, LR.UI.ALIGN.LEFT);

    }

	#region LOADING 
	override public void Load(string _id){
        base.Load(_id);
        
        //Characer Data form the profile
        var charData = ProfileManager.instance.GetCharacter(_id);

        //Load equipement and looks
        m_build.Load(charData);

        //Get Stats
        var stats = DataManager.instance.CharacterManager.ComputeStats(charData);
        if(charData != null )
        {
            m_maxStats = new Stats(stats);
            m_currentStats = new Stats(stats);
        }
        RefreshUI();

        //Load Attacks & Magics
        string weaponId = charData.GetEquipmentId(EquipmentType.WEAPON);
        if (weaponId != null)
        {
            AddAttack(weaponId);
        }
        if( charData.Skills != null && charData.Skills.Count > 0)
        {
            for (int i = 0; i < charData.Skills.Count; i++)
            {
                var magic = charData.Skills[i];
                AddMagic(magic.Id);
            }
        }
    }
	#endregion

	override protected void UpdateAttacking(){

	}

    #region ACTION

    public override void Attack(BattleActor _target,int _damage)
    {
        base.Attack(_target,_damage);
        m_charAnimator.Attack();
    }

	override public void TakeDamage(int _damage){
		
		if (_damage < 0)
			_damage = 0;
		CurrentStats.HP -=  _damage;

		m_charAnimator.TakeHit ();

		RefreshLifeGauge ();

		CheckDeath ();
        
	}

	#endregion

	override protected bool Die(){
		base.Die ();
		Utils.SetAlpha(m_sprite,0.0f);
		return true;
	}

    public string CharId
    {
        get { return m_charId; }
        set { m_charId = value; }
    }
}
