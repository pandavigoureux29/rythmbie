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

    [SerializeField] private GameObject m_note;

    public void StartNote()
    {
        m_noteComponent.OnDied += OnNoteDied;
        m_noteComponent.OnHit += OnNoteHit;
        m_noteComponent.OnMissed += OnNoteMiss;
        ToggleNote(true);
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
    }
    
    void OnNoteMiss()
    {
        ToggleNote(false);
    }
    
    void OnNoteHit()
    {
        ToggleNote(false);
    }

    public void ToggleNote(bool toggle)
    {
        m_note.SetActive(toggle);
    }

    public void Reset()
    {
        m_noteComponent.OnDied -= OnNoteDied;
    }
}
