using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_fireArmEffect;
    [SerializeField] private AudioSource m_audio;
    [SerializeField] private GameObject m_lightPrefab;
    [SerializeField] private Transform m_lightAnchor;

    private Animator m_lightAnimator;

    private void Awake()
    {
        var go = Instantiate(m_lightPrefab);
        m_lightAnimator = go.GetComponent<Animator>();
    }

    public void TriggerEffect()
    {
        m_fireArmEffect.Play();
        m_audio.Play();
        if (m_lightAnimator != null)
        {
            m_lightAnimator.transform.position = m_lightAnchor.position;
            m_lightAnimator.transform.rotation = m_lightAnchor.rotation;
            m_lightAnimator.SetTrigger("Fire");
        }
    }
}
