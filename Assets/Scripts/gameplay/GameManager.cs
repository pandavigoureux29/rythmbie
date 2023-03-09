using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;
    
    [SerializeField] private SongDataSO m_songAsset;
    [SerializeField] private TracksManager m_tracks;
    [SerializeField] private NotesGenerator m_notesGenerator;
    [SerializeField] private ScoreManager m_scoreManager;
    [SerializeField] private GameParticleEffectsManager m_effectsManager;

    public SongDataSO SongData => m_songAsset;
    public ScoreManager ScoreManager => m_scoreManager;

    [SerializeField] private AudioSource m_audio;

    private bool m_paused = true;

    private void Awake()
    {
        m_instance = this;
        m_tracks.Initialize();
        m_notesGenerator.Initialize(m_songAsset);
        m_effectsManager.Initialize();
        m_audio.clip = m_songAsset.Clip;

        //call after everything is loaded
        StartGame();
    }

    private void StartGame()
    {
        //TODO remove this
        m_audio.mute = true;

        m_audio.Play();
        m_paused = false;
    }

    void Update()
    {
        if(m_paused)
            return;
        
        m_notesGenerator.ManualUpdate(m_audio.time);
        m_tracks.ManualUpdate(m_audio.time);
    }

    public void CheckInput(GameplayInputActionInfos inputActionInfos)
    {
        m_tracks.CheckTracks(inputActionInfos, m_audio.time);
    }
}