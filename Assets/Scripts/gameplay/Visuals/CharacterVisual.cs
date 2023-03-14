using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisual : MonoBehaviour
{
    [SerializeField] private Animator m_charAnimator;
    [SerializeField] private InputRegion m_region;
    [SerializeField] private ParticleSystem m_bloodSplatterVFX;

    [SerializeField] private WeaponVisual m_weaponVisual;
    
    public void Initialize()
    {
        LR.EventDispatcher.Instance.Subscribe<NoteHitEventData>(OnNoteHit);
        LR.EventDispatcher.Instance.Subscribe<NoteAttackEventData>(OnNoteAttack);
    }

    void OnNoteHit(NoteHitEventData noteEventData)
    {
        if (noteEventData.Note.Track.Region == m_region)
        {
            Fire();
        }
    }

    void OnNoteAttack(NoteAttackEventData eventData)
    {
        if(eventData.Region == m_region)
            m_bloodSplatterVFX.Play();
    }

    void Fire()
    {
        m_charAnimator.SetTrigger("Fire");
        m_weaponVisual.TriggerEffect();
    }
}
