using UnityEngine;
using System.Collections;
using System.Reflection;

public class Stats {

	protected int m_level = 1;

	protected int m_HP= 0;

	protected int m_MP = 0;

	protected int m_attack = 0;
	protected int m_defense = 0;
	protected int m_magic = 0;
    private int m_speed = 0;

    public float attackPerfectModifier = 1.4f;
	public float attackGreatModifier = 1.2f;
	public float attackGoodModifier = 1.0f;
	public float attackBadModifier = 0.8f;

    public float blockPerfectModifier = 0.8f;
    public float blockGreatModifier = 0.9f;
    public float blockGoodModifier = 1.0f;
    public float blockBadModifier = 1.1f;

    public Stats() { }

    public Stats(Stats stats)
    {
        Utils.CopyProperties(stats, this);
    }

    public Stats(JSONObject json)
    {
        //stats
        Level = json.GetField("level")!=null ? (int) json.GetField("level").f : 1;
        Attack = (int)json.GetField("attack").f;
        Defense = (int)json.GetField("defense").f;
        Magic = (int)json.GetField("magic").f;
        HP = (int)json.GetField("hp").f;
        MP = (int)json.GetField("mp").f;
        Speed = (int)json.GetField("speed").f;
    }

    public Stats Add(Stats _stats)
    {
        Attack += _stats.Attack;
        Defense += _stats.Defense;
        Magic += _stats.Magic;
        HP += _stats.HP;
        MP += _stats.MP;
        Speed += _stats.Speed;
        return this;
    }

    public Stats Subtract(Stats _stats)
    {
        Attack -= _stats.Attack;
        Defense -= _stats.Defense;
        Magic -= _stats.Magic;
        HP -= _stats.HP;
        MP -= _stats.MP;
        Speed -= _stats.Speed;
        return this;
    }

    public float GetBlockerBonus(HitAccuracy _accuracy)
    {
        switch (_accuracy)
        {
            case HitAccuracy.PERFECT:
                return blockPerfectModifier;
            case HitAccuracy.GREAT:
                return blockGreatModifier;
            case HitAccuracy.GOOD:
                return blockGoodModifier;
            case HitAccuracy.MISS:
                return blockBadModifier;
        }
        return 1.0f;
    }

    public float GetAttackingBonus(HitAccuracy _accuracy)
    {
        switch (_accuracy)
        {
            case HitAccuracy.PERFECT:
                return attackPerfectModifier;
            case HitAccuracy.GREAT:
                return attackGreatModifier;
            case HitAccuracy.GOOD:
                return attackGoodModifier;
            case HitAccuracy.MISS:
                return attackBadModifier;
        }
        return 1.0f;
    }

    public int Level {
		get {
			return m_level;
		}
		set {
			m_level = value;
		}
	}

	public int HP {
		get {
			return m_HP;
		}
		set {
			m_HP = value;
		}
	}

	public int MP {
		get {
			return m_MP;
		}
		set {
			m_MP = value;
		}
	}

	public int Attack {
		get {
			return m_attack;
		}
		set {
			m_attack = value;
		}
	}

	public int Defense {
		get {
			return m_defense;
		}
		set {
			m_defense = value;
		}
	}

	public int Magic {
		get {
			return m_magic;
		}
		set {
			m_magic = value;
		}
	}

    public int Speed
    {
        get
        {
            return m_speed;
        }

        set
        {
            m_speed = value;
        }
    }
}
