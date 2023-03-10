using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private string m_id;
    public string Id => m_id;
    
    [SerializeField] private Transform m_begin;
    public Transform Begin => m_begin;

    [SerializeField] private Transform m_end;

    [SerializeField] private Transform m_slot;

    [SerializeField] private InputRegion m_region;
    public InputRegion Region => m_region;

    private float m_distanceTotal;
    public float DistanceTotal => m_distanceTotal;

    private float m_distanceToSlot;
    public float DistanceToSlot => m_distanceToSlot;
    
    private Vector3 m_direction;
    public Vector3 Direction => m_direction;

    private List<INote> m_notes = new List<INote>();
    private List<INote> m_notesToRemove = new List<INote>();

    // Notes can be still updated but are not hittable anymore
    private List<INote> m_activeNotes = new List<INote>();
    
    public INote CurrentNote => m_activeNotes.Count > 0 ? m_activeNotes[0] : null;

    public void Initialize()
    {
        m_distanceToSlot = (m_slot.position - m_begin.position).magnitude;
        
        var beginToEndVector = m_end.position - m_begin.position;
        m_distanceTotal = beginToEndVector.magnitude;
        m_direction = beginToEndVector.normalized;
    }

    public void AddNote(INote note)
    {
        m_notes.Add(note);
        m_activeNotes.Add(note);
    }

    public void DeactivateNote(INote note)
    {
        m_activeNotes.Remove(note);
    }

    public void RemoveNote(INote note)
    {
        DeactivateNote(note);
        m_notes.Remove(note);
    }

    public void ManualUpdate(float time)
    {
        //we might remove a note from the list while we're iterating on it, so we want to create a copy before that
        foreach (var note in m_notes.ToList())
        {
            note.ManualUpdate(time);
        }
    }
}
