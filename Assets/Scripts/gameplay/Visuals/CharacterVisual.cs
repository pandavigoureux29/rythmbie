using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisual : MonoBehaviour
{
    [SerializeField] private Animator m_charAnimator;
    [SerializeField] private InputRegion m_region;

    [SerializeField] private ParticleSystem m_fireArmEffect;
    
    public void Initialize()
    {
        LR.EventDispatcher.Instance.Subscribe<NoteHitEventData>(OnNoteHit);
    }

    void OnNoteHit(NoteHitEventData noteEventData)
    {
        if (noteEventData.Note.Track.Region == m_region)
        {
            Fire();
        }
    }

    void Fire()
    {
        m_charAnimator.SetTrigger("Fire");
        m_fireArmEffect.Play();
    }
}
