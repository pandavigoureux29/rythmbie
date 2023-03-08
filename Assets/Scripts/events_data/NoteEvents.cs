using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NoteDiedEventData
{
    public NoteComponent Note;
}


public struct NoteMissedEventData
{
    public NoteComponent Note;
}


public struct NoteHitEventData
{
    public NoteComponent Note;
}