using UnityEngine;
using System.Collections;

/** Visual effect of the magic, such as a fire ball or cure drop */ 
public class BattleMagicEffect : BattleActionEffect {
    
	/** Set by the BattleFightMagic when started */
	protected BattleMagic m_magic;

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

	override public void Launch(Vector3 _origin, Vector3 _destination){
        base.Launch(_origin,_destination);
		transform.position = _destination;
	}

	override public void Die(){
        base.Die();
	}

	public BattleMagic Magic {
		get {
			return m_magic;
		}
		set {
			m_magic = value;
		}
	}
}
