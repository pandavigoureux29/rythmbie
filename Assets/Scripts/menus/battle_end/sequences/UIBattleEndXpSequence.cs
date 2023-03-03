using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBattleEndXpSequence : UISequence {
        
    public override void Skip()
    {
        base.Skip();
    }

    protected override void LaunchStep()
    {
        switch (CurrentStep.Id)
        {
            case "xp_total":
                int total = ProfileManager.instance.BattleData.TotalXp ;
                var step = CurrentStep as UIStepTextNumberScroller;
                step.Launch(OnStepEnd, total);
                break;
            default: base.LaunchStep();
                break;
        }
    }

    public override void OnStepEnd(UIStep step)
    {
        base.OnStepEnd(step);
    }

}
