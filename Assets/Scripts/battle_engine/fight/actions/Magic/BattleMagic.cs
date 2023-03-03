using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleMagic : BattleAction {
    
	/** Mana used by second when the magic is active */
	protected int m_decreaseRate = 1;

	// Use this for initialization
	override protected void Start () {
        base.Start();
		for (int i = 0; i < m_effects.Count; i++) {
			((BattleMagicEffect)m_effects[i]).Magic = this;
		}
	}
	
	// Update is called once per frame
	override protected void Update () {
        base.Update();
	}
    
    override public void Die()
    {
        base.Die();
    }
    
	#region GETTERS-SETTERS

	#endregion
}
