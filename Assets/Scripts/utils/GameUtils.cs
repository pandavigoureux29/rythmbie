using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUtils {

    public class WeightableData : JSONData
    {
        public int Tiers;
        public int Weight;

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            if (_json.GetField("tiers") != null)
                Tiers = (int)_json.GetField("tiers").f;
            if (_json.GetField("weight") != null)
                Weight = (int)_json.GetField("weight").f;
        }
    }

    /// <summary>
    /// Returns a list of items that have the same tiers. Set _tiersRange if you want more inferior tiers
    /// </summary>
    public static List<T> SearchByTiers<T>(IEnumerable<T> pool, int _tiers, int _tiersRange = 0) where T : WeightableData
    {
        List<T> list = new List<T>();
        foreach (var item in pool)
        {
            bool isTiersOk = item.Tiers == _tiers;
            if (_tiersRange > 0) {
                isTiersOk &= item.Tiers >= _tiers - _tiersRange;
            }
            if ( isTiersOk )
                list.Add(item);
        }
        return list;
    }

    /// <summary>
    /// Gets a random item from the pool using the Russian Roulette algo
    /// </summary>
    public static T GetRandom<T>(List<T> pool) where T : WeightableData
    {
        //compute total weight
        int totalWeight = 0;
        foreach (var i in pool)
        {
            totalWeight += i.Weight;
        }
        int random = Random.Range(1,totalWeight);
        int stackedOffset = 0;

        WeightableData result = null;
        //for each item 
        foreach (var i in pool)
        {
            WeightableData w = (WeightableData)i;
            //if it's valid & the weight is drawn
            if (w.Weight > 0 && stackedOffset + w.Weight >= random )
            {
                result = w;
                break;
            }
            stackedOffset += w.Weight;
        }
        return result as T;
    }

}
