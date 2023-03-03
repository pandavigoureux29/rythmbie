using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIXpText : UITextNumberScroller {

    private List<int> m_beginNumbers;
    private List<int> m_targetNumbers;

    private int m_currentIndex = 0;

    /// <summary>
    /// Make multiple scrollings from the array in parameter. Each couple in this array represents the begin value and the end value
    /// ie : [ 50, 0, 55, 9 ] will scroll once from 50 to 0, then from 55 to 9
    /// </summary>
    public virtual void ScrollTo(int[] _targetNumbers, float _duration)
    {
        m_targetNumbers = new List<int>();
        m_beginNumbers = new List<int>();
        int totalDelta = 0;
        for(int i=0; i < _targetNumbers.Length; i++)
        {
            if( i%2 == 0)
            {
                m_beginNumbers.Add(_targetNumbers[i]);
                if( i > 0 )
                    totalDelta +=  _targetNumbers[i - 1] - _targetNumbers[i];
            }
            else
            {
                m_targetNumbers.Add(_targetNumbers[i]);
            }
        }
        //Set starting value
        m_text.text = "" + m_beginNumbers[0];
        m_targetNumber = m_targetNumbers[0];

        m_scrolling = true;
        
        //Get direction of the scroll
        m_direction = totalDelta < 0 ? -1 : 1;
        //compute the speed
        m_stepTime = _duration / Mathf.Abs(totalDelta);
        m_time = 0;
    }
    
    protected override void TargetReached()
    {
        m_currentIndex++;
        if( m_currentIndex >= m_targetNumbers.Count)
        {
            m_scrolling = false;
        }
        else
        {
            m_text.text = "" + m_beginNumbers[m_currentIndex];
            m_targetNumber = m_targetNumbers[m_currentIndex];
        }
    }
}
