using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteComponent : MonoBehaviour
{
    private NoteData m_noteData;
    private Track m_track;
    private float m_timeCreated;

    private float m_distanceDone = 0;
    
    //time from the slot (perfect time) to the end of the track.basically the extra time the player has to hit the note after the perfect time.
    public float m_extraTime = 0;

    private float m_totalTime = 0;

    public void Initialize(NoteData noteData, Track track, float timeCreated)
    {
        m_noteData = noteData;
        m_track = track;
        transform.position = track.Begin.position;
        m_timeCreated = timeCreated;

        m_extraTime = (track.DistanceTotal - track.DistanceToSlot) / GameManager.Instance.SongData.Speed;
        m_totalTime = m_noteData.Time + m_extraTime;
    }

    public void ManualUpdate(float time)
    {
        //time progression of the note on the track [0,1]
        var timeProgression = Mathf.InverseLerp(m_timeCreated, m_totalTime, time);

        if (timeProgression >= 1)
        {
            Die();
        }
        else
        {
            m_distanceDone = Mathf.Lerp(0, m_track.DistanceTotal, timeProgression);

            var position = m_track.Begin.position + m_track.Direction * m_distanceDone;
            transform.position = position;
        }
    }

    public void CleanUp()
    {
        m_track.RemoveNote(this);
        m_noteData = null;
        m_track = null;
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    public void Die()
    {
        LR.EventDispatcher.Instance.Publish(new NoteDiedEventData{Note = this});
        Debug.Log("DIE");
    }
}