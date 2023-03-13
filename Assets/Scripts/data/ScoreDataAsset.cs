using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreDataAsset", menuName = "SongData/Scoring")]
public class ScoreDataAsset : ScriptableObject
{
    [SerializeField] private List<ScoreDataAccuracy> Accuracies;

    public ScoreAccuracy GetAccuracy(float deltaTime)
    {
        float best = 1000;
        ScoreAccuracy result = ScoreAccuracy.MISSED;
        foreach (var accData in Accuracies)
        {
            if (best > accData.DeltaTime && deltaTime <= accData.DeltaTime)
            {
                best = accData.DeltaTime;
                result = accData.Accuracy;
            }
        }

        return result;
    }

    public ScoreDataAccuracy GetScoreData(ScoreAccuracy scoreAccuracy)
    {
        return Accuracies.FirstOrDefault(x => x.Accuracy == scoreAccuracy);
    }
}

[Serializable]
public class ScoreDataAccuracy
{
    [SerializeField] public ScoreAccuracy Accuracy;
    [SerializeField] public float DeltaTime;
    [SerializeField] public int Score;
}
