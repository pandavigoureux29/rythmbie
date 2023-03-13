using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreDataAsset m_scoreData;
    public ScoreDataAsset ScoreData => m_scoreData;

    public Action<ScoreEventData> OnScoreChangedEvent;

    private int m_totalScore = 0;

    private void Awake()
    {
        LR.EventDispatcher.Instance.Subscribe<NoteHitEventData>(OnNoteHit);
    }

    void OnNoteHit(NoteHitEventData eventData)
    {
        var scoreData = m_scoreData.GetScoreData(eventData.Accuracy);
        m_totalScore += scoreData.Score;
        OnScoreChangedEvent?.Invoke(new ScoreEventData{ DeltaScore = scoreData.Score, TotalScore = m_totalScore});
    }

    public struct ScoreEventData
    {
        public int DeltaScore;
        public int TotalScore;
    }
}
