using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    [SerializeField] private ScoreManager m_scoreManager;

    [SerializeField] private TextMeshProUGUI m_text;

    private void Awake()
    {
        m_scoreManager.OnScoreChangedEvent += OnScoreChanged;
    }

    private void OnDestroy()
    {
        m_scoreManager.OnScoreChangedEvent -= OnScoreChanged;
    }

    void OnScoreChanged(ScoreManager.ScoreEventData scoreEventData)
    {
        m_text.text = scoreEventData.TotalScore.ToString();
    }
}
