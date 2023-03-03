using UnityEngine;
using System.Collections;

public class BattleMagicEffectHoming : BattleMagicEffect {

	[SerializeField] protected SpriteRenderer m_destructionSprite;

	Animator m_destructionAnimator;

	private string m_state = "idle";

	// Use this for initialization
	protected override void Awake () {
        base.Awake();
		m_destructionAnimator = m_destructionSprite.GetComponent<Animator> ();
		m_destructionSprite.enabled = false;
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
		if (m_launched) {
			switch( m_state ){
			    case "homing": UpdateHoming(); break;
			    case "exploding" : UpdateExplosion(); break;
			}
		}
	}

	void UpdateHoming(){
		//wait for the homing animation to end
		if ( Utils.IsAnimationStateRunning( m_animator, "idle" ) ) {
			m_magic.OnHit();
            m_effectSprite.enabled = false;
            m_state = "exploding";
            m_destructionSprite.enabled = true;
            m_destructionSprite.transform.position = m_destination;
			m_destructionAnimator.Play("explosion");
		}
	}

	void UpdateExplosion(){
		//wait for the animation of the explosion to finish
		if ( Utils.IsAnimationStateRunning(m_destructionAnimator, "idle") ) {			
			Die ();
		}
	}

	override public void Launch(Vector3 _origin, Vector3 _destination){
        base.Launch(_origin,_destination);
		transform.position = _origin;
        //hide explosion
        m_destructionSprite.enabled = false;
        m_effectSprite.enabled = true;
        //launch animation
        m_animator.SetTrigger("attack");
		m_state = "homing";
	}

	override public void Die(){
		base.Die ();
        m_state = "idle";
        m_destructionSprite.enabled = false;
        m_effectSprite.enabled = false;
	}
}
