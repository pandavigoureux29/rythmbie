using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreDataAsset m_scoreData;
    public ScoreDataAsset ScoreData => m_scoreData;
}
