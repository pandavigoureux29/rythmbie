using UnityEngine;
using System.Collections;

public class BattleCharacterAnimator : MonoBehaviour {

	[SerializeField] Animator m_animator;
    [SerializeField] CharacterBuild m_build;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Plays the attack animation.
	/// </summary>
	public void Attack(){
		if (Utils.IsAnimationStateRunning (m_animator, "attack")) {
			m_animator.Play ("attack", 0, 0.0f);
		} else {
			m_animator.SetTrigger ("attackTrigger");
		}
	}

	/// <summary>
	/// Place the animation "takes a hit".
	/// </summary>
	public void TakeHit(){
		if (Utils.IsAnimationStateRunning (m_animator, "hit")) {
			m_animator.Play ("hit", 0, 0.0f);
		} else {
			m_animator.SetTrigger ("hitTrigger");
		}
	}
	
}
