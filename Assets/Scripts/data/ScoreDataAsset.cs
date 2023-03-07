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
        float best = Single.PositiveInfinity;
        ScoreAccuracy result = ScoreAccuracy.BAD;
        foreach (var accData in Accuracies)
        {
            if (deltaTime <= accData.DeltaTime && best < deltaTime)
            {
                best = deltaTime;
                result = accData.Accuracy;
            }
        }

        return result;
    }
}

[Serializable]
public class ScoreDataAccuracy
{
    [SerializeField] public ScoreAccuracy Accuracy;
    [SerializeField] public float DeltaTime;
}
