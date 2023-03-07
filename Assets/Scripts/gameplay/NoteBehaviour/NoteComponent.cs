using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteComponent : MonoBehaviour
{
    private NoteData m_noteData;
    private Track m_track;
    private float m_timeCreated;

    public void Initialize(NoteData noteData, Track track, float timeCreated)
    {
        m_noteData = noteData;
        m_track = track;
        transform.position = track.Begin.position;
        m_timeCreated = timeCreated;
    }

    public void ManualUpdate(float time)
    {
        //time progression of the note on the track [0,1]
        var timeProgression = Mathf.InverseLerp(m_timeCreated, m_noteData.Time, time);
        var distanceDone = Mathf.Lerp(0, m_track.Distance, timeProgression);

        var position = m_track.Begin.position + m_track.Direction * distanceDone;
        transform.position = position;
    }

    public void CleanUp()
    {
    }

    public void Dispose()
    {
    }
}