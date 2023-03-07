using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private string m_id;
    public string Id => m_id;
    
    [SerializeField] private Transform m_begin;
    public Transform Begin => m_begin;

    [SerializeField] private Transform m_end;

    [SerializeField] private Transform m_slot;

    private float m_distance;
    public float Distance => m_distance;
    
    private Vector3 m_direction;
    public Vector3 Direction => m_direction;

    private List<NoteComponent> m_notes = new List<NoteComponent>();

    public void Initialize()
    {
        var delta = m_end.position - m_begin.position;
        m_distance = delta.magnitude;
        m_direction = delta.normalized;
    }

    public void AddNote(NoteComponent note)
    {
        m_notes.Add(note);
    }

    public void ManualUpdate(float time)
    {
        foreach (var note in m_notes)
        {
            note.ManualUpdate(time);
        }
    }
}
