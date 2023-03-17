using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteVisual : MonoBehaviour
{
    [SerializeField] private NoteComponent m_noteComponent;
    [SerializeField] private Animator m_animator;

    [SerializeField] private float m_rightRotationY;
    [SerializeField] private float m_leftRotationY;

    [SerializeField] private GameObject m_noteBeacon;

    [SerializeField] private float m_attackDuration = 2;
    [SerializeField] private float m_hitDuration = 1;

    [SerializeField] private ParticleSystem m_hitVFX;
    [SerializeField] private MeshRenderer m_renderer;
    [SerializeField] private Color m_focusColor;

    private Color m_baseColor;

    private Vector3 m_animatorInitialPosition;
    private Quaternion m_animatorInitialRotation;

    public void Awake()
    {
        m_noteComponent.OnStarted += StartNote;
        m_animatorInitialPosition = m_animator.transform.localPosition;
        m_animatorInitialRotation = m_animator.transform.localRotation;
        m_baseColor = m_renderer.material.GetColor("_Zone_Color");
    }

    void StartNote()
    {
        m_noteComponent.OnDied += OnNoteDied;
        m_noteComponent.OnHit += OnNoteHit;
        m_noteComponent.OnMissed += OnNoteMiss;
        m_noteComponent.OnAttack += OnAttack;
        m_noteComponent.OnFocus += Focus;
        
        SetDirection(m_noteComponent.Track.Direction);
        
        ToggleNote(true);
        m_animator.enabled = true;
        m_animator.gameObject.SetActive(true);
    }
    
    public void SetDirection(Vector3 direction)
    {
        var rotationY = m_rightRotationY;
        if (direction.x < 0)
            rotationY = m_leftRotationY;
        var eulerAngles = transform.rotation.eulerAngles; 
        eulerAngles.y = rotationY;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }

    void OnNoteDied()
    {
        ToggleNote(false);
        Reset();
    }
    
    void OnNoteMiss()
    {
        UnFocus();
        ToggleNote(false);
    }
    
    void OnNoteHit()
    {
        UnFocus();
        ToggleNote(false);
        m_animator.gameObject.SetActive(false);
        m_hitVFX.Play();
        StartCoroutine(WaitAndDieCoroutine(m_hitDuration));
    }

    void OnAttack()
    {
        m_animator.SetTrigger("Attack");
        StartCoroutine(WaitAndDieCoroutine(m_attackDuration));
    }

    IEnumerator WaitAndDieCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        m_noteComponent.Die(false);
        Reset();
    }

    public void Focus()
    {
        m_renderer.material.SetColor("_Zone_Color", m_focusColor);
    }

    private void UnFocus()
    {
        m_renderer.material.SetColor("_Zone_Color", m_baseColor);
    }

    public void ToggleNote(bool toggle)
    {
        m_noteBeacon.SetActive(toggle);
    }

    public void Reset()
    {
        m_noteComponent.OnDied -= OnNoteDied;
        m_noteComponent.OnHit -= OnNoteHit;
        m_noteComponent.OnMissed -= OnNoteMiss;
        m_noteComponent.OnFocus -= Focus;

        m_animator.enabled = false;
        m_animator.transform.localPosition = m_animatorInitialPosition;
        m_animator.transform.localRotation = m_animatorInitialRotation;
        
        UnFocus();
    }

    private void OnDestroy()
    {
        Reset();
    }
}
