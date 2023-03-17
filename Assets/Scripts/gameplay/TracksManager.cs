using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TracksManager : MonoBehaviour
{
    [SerializeField] private List<Track> m_leftTracks;
    [SerializeField] private List<Track> m_rightTracks;
    private List<Track> m_tracks;

    private NoteComponent m_currentFocusNote = null;
    
    public void Initialize()
    {
        m_tracks = m_leftTracks.ToList();
        m_tracks.InsertRange(0, m_rightTracks);
        
        m_tracks.ForEach(x=>x.Initialize());
    }

    public void ManualUpdate(float time)
    {
        foreach (var track in m_tracks)
        {
            track.ManualUpdate(time);
        }
        
        CheckFocusNote();
    }

    public void CheckTracks(GameplayInputActionInfos inputActionInfos, float currentTime)
    {
        var tracks = inputActionInfos.inputRegion == InputRegion.LEFT ? m_leftTracks : m_rightTracks;
        NoteComponent bestNote = GetCurrentClosestNoteInTracks(tracks);
        
        NoteInputResult inputResult = bestNote != null ? bestNote.CheckInput(inputActionInfos, currentTime) : NoteInputResult.None;

        switch (inputResult.HitResult)
        {
            case NoteHitResult.HIT :
                bestNote.Hit(inputResult.Accuracy);
                break;
            case NoteHitResult.MISSED:
                bestNote.Miss();
                break;
        }
    }

    public NoteComponent GetCurrentClosestNoteInTracks(List<Track> tracks)
    {
        float bestTime = 1000;
        NoteComponent bestNote = null, tempNote = null;
        //find note with the minimum time in tracks ( closest to its slot )
        foreach (var track in tracks)
        {
            tempNote = track.CurrentNote as NoteComponent;
            if (tempNote != null && tempNote.Data.Time < bestTime)
            {
                bestNote = tempNote;
                bestTime = bestNote.Data.Time;
            }
        }

        return bestNote;
    }

    void CheckFocusNote()
    {
        var bestLeft = GetCurrentClosestNoteInTracks(m_leftTracks);
        var bestRight = GetCurrentClosestNoteInTracks(m_rightTracks);
        
        if(bestLeft == m_currentFocusNote || bestRight == m_currentFocusNote)
            return;

        bool found = false;
        
        
        // if there's no note on one of the tracks
        if (bestLeft == null && bestRight != null)
        {
            m_currentFocusNote = bestRight;
            found = true;
        }

        if (bestRight == null && bestLeft != null)
        {
            m_currentFocusNote = bestLeft;
            found = true;
        }

        if (!found)
        {
            if (bestLeft.Data.Time == bestRight.Data.Time)
            {
                m_currentFocusNote = bestLeft;
                bestRight.SetFocus();
            }
            else if (bestLeft.Data.Time < bestRight.Data.Time)
            {
                m_currentFocusNote = bestLeft;
            }
            else
            {
                m_currentFocusNote = bestRight;
            }
        }

        m_currentFocusNote.SetFocus();
    }

    public Track GetTrack(string id)
    {
        return m_tracks.FirstOrDefault(x => x.Id == id);
    }
}
