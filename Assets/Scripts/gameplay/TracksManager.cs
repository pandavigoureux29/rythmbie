using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TracksManager : MonoBehaviour
{
    [SerializeField] private List<Track> m_tracks;
    
    public void Initialize()
    {
        m_tracks.ForEach(x=>x.Initialize(this));
    }

    public void ManualUpdate(float time)
    {
        foreach (var track in m_tracks)
        {
            track.ManualUpdate(time);
        }
    }

    public Track GetTrack(string id)
    {
        return m_tracks.FirstOrDefault(x => x.Id == id);
    }
}
