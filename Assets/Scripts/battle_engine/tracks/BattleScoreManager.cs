using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleScoreManager : MonoBehaviour {

    static BattleScoreManager _instance = null;
	//Accuracy variables
	[SerializeField] private float m_accuPerfect = 80f;
	[SerializeField] private float m_accuGreat = 50f;

	//COUNT
	int m_notesCount = 0;
    /// <summary>
    /// Notes sorted by accuracy
    /// </summary>
	Dictionary<HitAccuracy,int> m_notesCountByAcc;

	//SCORE
	int m_totalScore = 0;
	Dictionary<HitAccuracy, int>  m_baseScoreByAcc;

    void Awake()
    {
        InitScoresData();
        _instance = this;
    }

	// Use this for initialization
	void Start () {
        m_notesCountByAcc = new Dictionary<HitAccuracy, int>();

        for (int i = 0; i < Utils.EnumCount(HitAccuracy.GOOD); i++) {
            HitAccuracy acc = (HitAccuracy)i;
			m_notesCountByAcc.Add(acc, 0 );
			if( !m_baseScoreByAcc.ContainsKey(acc) )
            	m_baseScoreByAcc[acc] = 0;
        }        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/** Adds Note to the scoreManager and returns accuracy (HitAccuracy) */
	public HitAccuracy AddNote ( float _accuracyValue ){
        //increase total
		m_notesCount ++;
        //Compute Accuracy
        HitAccuracy acc = GetAccuracyByValue(_accuracyValue);   
        //keep total of accuracies     
        m_notesCountByAcc[acc]++;
        //Increment score
        m_totalScore += m_baseScoreByAcc[acc];
		return acc;
	}

    public HitAccuracy GetAccuracyByValue(float _accuracyValue)
    {
        HitAccuracy acc;
        if (_accuracyValue > m_accuPerfect)
        {
            acc = HitAccuracy.PERFECT;
        }
        else if (_accuracyValue > m_accuGreat)
        {
            acc = HitAccuracy.GREAT;
        }
        else if (_accuracyValue > 0)
        {
            acc = HitAccuracy.GOOD;
        }
        else
        {
            acc = HitAccuracy.MISS;
        }
        return acc;
    }

    void InitScoresData()
    {
        var scoring = DataManager.instance.GameDataManager.AccuracyScoring;
		m_baseScoreByAcc = new Dictionary<HitAccuracy, int>();
        foreach(var score in scoring.scores)
        {
            try
            {
                m_baseScoreByAcc[score.Key] = score.Value;
            }catch(System.Exception e)
            {
                e.ToString();
            }
        }
    }

    public int TotalScore
    {
        get { return m_totalScore; }
    }

    public int NotesCount {
        get { return m_notesCount; }
    }

    public Dictionary<HitAccuracy,int> NotesCountByAccuracy
    {
        get { return m_notesCountByAcc; }
    }

    public static BattleScoreManager instance
    {
        get
        {
            return _instance;
        }
    }
}
