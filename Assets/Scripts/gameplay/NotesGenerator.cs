using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotesGenerator : MonoBehaviour
{
    [SerializeField] private NotesPoolManager m_pool;
    [SerializeField] private TracksManager m_tracks;
    
    private SongDataSO m_songAsset;
    private Dictionary<Track, int> m_currentIndexNoteByTrack;
    private Dictionary<Track, float> m_deltaTimeByTrack;

    public void Initialize(SongDataSO songData)
    {
        m_songAsset = songData;
        m_pool.Initialize();
        
        //index of the next note to check the time with
        m_currentIndexNoteByTrack = new Dictionary<Track, int>();
        //delta time before which we need to create a note
        m_deltaTimeByTrack = new Dictionary<Track, float>();
        
        foreach (var segment in m_songAsset.Segments)
        {
            var track = m_tracks.GetTrack(segment.Id);
            m_currentIndexNoteByTrack[track] = 0;
            m_deltaTimeByTrack[track] = track.Distance / m_songAsset.Speed;
        }
    }
    
    public void ManualUpdate(float time)
    {
        foreach (var track in m_currentIndexNoteByTrack.Keys.ToList())
        {
            var segment = m_songAsset.GetSegment(track.Id);

            //skip finished segments
            if (m_currentIndexNoteByTrack[track] >= segment.Notes.Count)
            {
                return;
            }
            
            var nextNoteToCheck = segment.Notes[ m_currentIndexNoteByTrack[track] ];
            float timeToCreate = time + m_deltaTimeByTrack[track];
            float diff = nextNoteToCheck.Time - timeToCreate;

            if (diff <= float.Epsilon)
            {
                if(diff < -0.5f)
                    Debug.LogError($"[GENERATION] Create note {nextNoteToCheck.Time}. Diff {diff}. First note should begin after {m_deltaTimeByTrack[track]}, or increase speed ");
                CreateNote(nextNoteToCheck,track, time);
                m_currentIndexNoteByTrack[track]++;
            }
        }
    }

    void CreateNote(NoteData noteData, Track track, float timeCreated)
    {
        var newNote = m_pool.Take();
        newNote.gameObject.SetActive(true);
        newNote.Initialize(noteData,track, timeCreated);
        track.AddNote(newNote);
    }
}
