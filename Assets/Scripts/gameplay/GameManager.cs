using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance => m_instance;
    
    [SerializeField] private SongDataSO m_songAsset;
    [SerializeField] private TracksManager m_tracks;
    [SerializeField] private NotesGenerator m_notesGenerator;
    [SerializeField] private ScoreManager m_scoreManager;
    [SerializeField] private GameParticleEffectsManager m_effectsManager;

    [SerializeField] private List<CharacterVisual> m_characters;

    public SongDataSO SongData => m_songAsset;
    public ScoreManager ScoreManager => m_scoreManager;

    [SerializeField] private AudioSource m_audio;

    private bool m_paused = true;

    public Action<float> OnUpdateEvent;
    
    private void Awake()
    {
        m_instance = this;
        m_tracks.Initialize();
        m_notesGenerator.Initialize(m_songAsset);
        m_effectsManager.Initialize();
        m_characters.ForEach(x=>x.Initialize());
        m_audio.clip = m_songAsset.Clip;

        //call after everything is loaded
        StartGame();
    }

    private void StartGame()
    {
        m_audio.Play();
        m_paused = false;
    }

    void Update()
    {
        if(m_paused)
            return;
        
        m_notesGenerator.ManualUpdate(m_audio.time);
        m_tracks.ManualUpdate(m_audio.time);
        
        OnUpdateEvent?.Invoke(m_audio.time);
    }

    public void CheckInput(GameplayInputActionInfos inputActionInfos)
    {
        m_tracks.CheckTracks(inputActionInfos, m_audio.time);
    }

    public void QuitToBoot()
    {
        SceneManager.LoadScene("Boot");
    }
}