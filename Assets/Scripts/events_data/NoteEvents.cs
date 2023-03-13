using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NoteCreatedEventData
{
    public NoteComponent Note;
    public float TimeCreated;
}

public struct NoteDiedEventData
{
    public NoteComponent Note;
    public bool IsHit;
}


public struct NoteMissedEventData
{
    public NoteComponent Note;
}


public struct NoteHitEventData
{
    public NoteComponent Note;
}
