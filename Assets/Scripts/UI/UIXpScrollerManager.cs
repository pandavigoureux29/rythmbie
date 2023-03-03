using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Scrolls xp on a Text component and fill a gauge at the same time
/// </summary>
public class UIXpScrollerManager : MonoBehaviour {

    [SerializeField] UIFilledGauge m_gauge;
    [SerializeField] Text m_xpText;

    [SerializeField] Text m_levelText;

    [SerializeField] protected float m_FillUnitValue = 0.1f;

    /// <summary>
    /// Used to accelerate the fill according to the xp range
    /// </summary>
    protected float m_fillMultiplier = 1.0f;

    protected bool m_scrolling = false;

    protected float m_currentXp = 0;
    protected int m_totalXp = 0;

    protected float m_timeByUnit = 1;
    protected float m_time = 0;
    protected int m_direction = 1;

    DataCharManager.LevelUpData m_nextLevelData;
    protected int m_previousLevelXpNeeded = 0;
    Job m_job;
    
    public delegate void onScrollFinished(UIXpScrollerManager _scroller);
    protected onScrollFinished m_scrollFinishedDelegate = null;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_scrolling)
        {
            m_time += Time.deltaTime;
            if (m_time >= m_timeByUnit)
            {
                m_time = 0;
                //increase value with computed speeds
                m_currentXp += m_FillUnitValue * m_fillMultiplier;
                                
                //check if a level is reached
                if ( m_currentXp >= m_nextLevelData.XpNeeded)
                {
                    SetNextLevel();
                }

                //check if the total xp has been processed
                if (m_currentXp >= m_totalXp)
                {
                    m_currentXp = m_totalXp;
                    m_scrolling = false;
                    TargetReached();
                }
                SetValues();
            }
        }
    }

    void SetValues()
    {
        m_xpText.text = Mathf.RoundToInt (m_nextLevelData.XpNeeded - m_currentXp).ToString();
        SetGaugeValue();
    }

    void SetGaugeValue()
    {
        if(m_gauge != null)
        {            
            int length = Mathf.Abs( m_nextLevelData.XpNeeded - m_previousLevelXpNeeded );
            float prog = 0;
            
            prog = Mathf.Abs(m_currentXp - m_previousLevelXpNeeded) / (float)length;

            if(m_gauge != null)
                m_gauge.SetValue(prog);
        }
    }

    void SetNextLevel()
    {
        m_previousLevelXpNeeded = m_nextLevelData.XpNeeded;
        m_levelText.text = m_nextLevelData.Stats.Level.ToString();
        m_levelText.GetComponent<Animation>().Play();
        //get next level data
        m_nextLevelData = DataManager.instance.CharacterManager.GetLevel(m_job, m_nextLevelData .Stats.Level+ 1);
        SetFillMultiplier();
    }
    
    protected virtual void TargetReached()
    {
        m_scrolling = false;
        if( m_scrollFinishedDelegate != null)
        {
            m_scrollFinishedDelegate(this);
        }
    }

    public virtual void Scroll(int _xpStart, int _xpEnd, Job _job, float _duration, onScrollFinished _delegate = null)
    {
        m_job = _job;

        //current level
        DataCharManager.LevelUpData levelUpData = DataManager.instance.CharacterManager.GetLevelByXp(_job, _xpStart);
        //xp for previous level
        m_previousLevelXpNeeded = levelUpData != null ? levelUpData.XpNeeded : 0;
        m_nextLevelData = DataManager.instance.CharacterManager.GetLevel(m_job, levelUpData.Stats.Level + 1);

        m_currentXp = _xpStart - m_previousLevelXpNeeded;
        m_totalXp = _xpEnd;
        
        //Get direction of the scroll
        float delta = Mathf.Abs(_xpEnd - _xpStart);
        //compute the speed
        m_timeByUnit = _duration * m_FillUnitValue / Mathf.Abs(delta);

        m_scrollFinishedDelegate = _delegate;
        m_scrolling = true;
        SetFillMultiplier();
    }

    public void Skip()
    {
        m_nextLevelData = DataManager.instance.CharacterManager.GetLevelByXp(m_job, m_totalXp);
        m_previousLevelXpNeeded = DataManager.instance.CharacterManager.GetLevel(m_job, m_nextLevelData.Stats.Level - 1).XpNeeded;
        m_currentXp = m_totalXp;
        SetValues();
        TargetReached();
    }

    void SetFillMultiplier()
    {
        m_fillMultiplier = 1.0f + (m_nextLevelData.XpNeeded - m_previousLevelXpNeeded) *0.06f;
    }

    public bool Scrolling
    {
        get
        {
            return m_scrolling;
        }
    }
}
