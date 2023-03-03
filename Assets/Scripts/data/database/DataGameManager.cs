using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataGameManager : DatabaseLoader
{
    AccuracyScoringData m_accuracyScoring;

    protected override void LoadDatabase()
    {
        base.LoadDatabase();
        var tempDatabase = LoadDataJSON("game_database");
        var accJSON = tempDatabase["accuracy_scoring"];
        m_accuracyScoring = new AccuracyScoringData(accJSON[0]);
    }

    public class AccuracyScoringData : JSONData
    {
        public Dictionary<HitAccuracy, int> scores = new Dictionary<HitAccuracy, int>();

        public AccuracyScoringData(JSONObject _json) : base(_json) { }

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            for(int i=0; i < Utils.EnumCount(HitAccuracy.GOOD) -1; ++i)
            {
                HitAccuracy hit = ((HitAccuracy)i);
                int acc = (int)_json.GetField(hit.ToString().ToLower()).f;
                scores[hit] = acc;
            }
        }
    }

    public AccuracyScoringData AccuracyScoring { get { return m_accuracyScoring; } }
}
