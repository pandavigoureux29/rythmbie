using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_fireArmEffect;
    [SerializeField] private AudioSource m_audio;

    public void TriggerEffect()
    {
        m_fireArmEffect.Play();
        m_audio.Play();
    }
}
