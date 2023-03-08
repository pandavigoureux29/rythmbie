using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TracksManager : MonoBehaviour
{
    [SerializeField] private List<Track> m_leftTracks;
    [SerializeField] private List<Track> m_rightTracks;
    private List<Track> m_tracks;
    
    public void Initialize()
    {
        m_tracks = m_leftTracks.ToList();
        m_tracks.InsertRange(0, m_rightTracks);
        
        m_tracks.ForEach(x=>x.Initialize(this));
    }

    public void ManualUpdate(float time)
    {
        foreach (var track in m_tracks)
        {
            track.ManualUpdate(time);
        }
    }

    public void CheckTracks(GameplayInputActionInfos inputActionInfos, float currentTime)
    {
        var tracks = inputActionInfos.inputRegion == InputRegion.LEFT ? m_leftTracks : m_rightTracks;
        NoteComponent bestNote = GetCurrentClosestNoteInTracks(tracks);
        
        var hitResult = bestNote != null ? bestNote.CheckInput(inputActionInfos, currentTime) : NoteHitResult.NONE;

        switch (hitResult)
        {
            case NoteHitResult.HIT :
                bestNote.Hit();
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
            tempNote = track.CurrentNote;
            if (tempNote != null && tempNote.Data.Time < bestTime)
            {
                bestNote = tempNote;
                bestTime = bestNote.Data.Time;
            }
        }

        return bestNote;
    }

    public Track GetTrack(string id)
    {
        return m_tracks.FirstOrDefault(x => x.Id == id);
    }
}
