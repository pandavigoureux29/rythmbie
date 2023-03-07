using System.Collections;
using System.Collections.Generic;
using LR;
using UnityEngine;

public class NotesPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject m_notePrefab;
    private Pool<NoteComponent> m_pool;

    private int m_count = 0;
    public void Initialize()
    {
        m_pool = new Pool<NoteComponent>(CreateNoteComponent, CleanUp, Dispose);
        m_pool.Capacity = 5;
    }

    public NoteComponent Take()
    {
        return m_pool.Take();
    }

    public void Return(NoteComponent noteComponent)
    {
        m_pool.Return(noteComponent);
    }

    public void CleanUp()
    {
        m_pool.ReturnAll();
    }

    NoteComponent CreateNoteComponent()
    {
        var noteObject = Instantiate(m_notePrefab);
        noteObject.name += "("+ m_count+")";
        noteObject.transform.SetParent(transform);
        noteObject.SetActive(false);
        m_count++;
        return noteObject.GetComponent<NoteComponent>();
    }

    void CleanUp(NoteComponent noteComponent)
    {
        noteComponent.gameObject.SetActive(false);
        noteComponent.CleanUp();
    }

    void Dispose(NoteComponent noteComponent)
    {
        noteComponent.Dispose();
        Destroy(noteComponent);
    }
}
