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

    private Vector3 m_animatorInitialPosition;
    private Quaternion m_animatorInitialRotation;

    public void Awake()
    {
        m_noteComponent.OnStarted += StartNote;
        m_animatorInitialPosition = m_animator.transform.localPosition;
        m_animatorInitialRotation = m_animator.transform.localRotation;
    }

    void StartNote()
    {
        m_noteComponent.OnDied += OnNoteDied;
        m_noteComponent.OnHit += OnNoteHit;
        m_noteComponent.OnMissed += OnNoteMiss;
        m_noteComponent.OnAttack += OnAttack;
        ToggleNote(true);
        SetDirection(m_noteComponent.Track.Direction);
        m_animator.enabled = true;
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
        ToggleNote(false);
    }
    
    void OnNoteHit()
    {
        ToggleNote(false);
    }

    void OnAttack()
    {
        m_animator.SetTrigger("Attack");
        StartCoroutine(WaitForEndAttackCoroutine());
    }

    IEnumerator WaitForEndAttackCoroutine()
    {
        yield return new WaitForSeconds(m_attackDuration);
        m_noteComponent.Die(false);
        Reset();
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

        m_animator.enabled = false;
        m_animator.transform.localPosition = m_animatorInitialPosition;
        m_animator.transform.localRotation = m_animatorInitialRotation;
    }

    private void OnDestroy()
    {
        Reset();
    }
}
