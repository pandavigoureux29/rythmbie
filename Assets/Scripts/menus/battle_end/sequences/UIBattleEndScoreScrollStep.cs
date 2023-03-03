using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIBattleEndScoreScrollStep : UIStep {

    [SerializeField] List<UIInfo> m_accuraciesScrollers;

    int m_count = 0;

    public void Launch(OnStepEndDelegate _del, Dictionary<HitAccuracy, int> _accuracies)
    {
        base.Launch(_del);
        foreach( var accData in _accuracies)
        {
            m_count++;
            var scroller = m_accuraciesScrollers.FirstOrDefault(x => x.accuracy == accData.Key).Scroller;
            scroller.ScrollTo(accData.Value, 2.0f,OnTargetReached);
        }
    }

    public void OnTargetReached(UITextNumberScroller scroller)
    {
        m_count--;
        if( m_count <= 0)
            Stop();
    }

    public override void Skip()
    {
        foreach(var acc in m_accuraciesScrollers)
        {
            acc.Scroller.Skip();
        }
    }

    [System.Serializable]
    public class UIInfo
    {
        public UITextNumberScroller Scroller;
        public HitAccuracy accuracy;
    }

}
