using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DataInventoryManager : DatabaseLoader
{
    ActionDataCollection m_attacks;
    ActionDataCollection m_magics;

    ShardDataCollection m_shards;
    DataManager.ColorDataCollection m_shardColors;

    protected override void LoadDatabase()
    {
        base.LoadDatabase();

        JSONObject tempJson;
        tempJson = LoadDataJSON("skills_database");
        m_attacks = JSONLoaderLR.LoadTable<ActionDataCollection>(tempJson["attack"]);
        m_magics = JSONLoaderLR.LoadTable<ActionDataCollection>(tempJson["magic"]);
        tempJson = LoadDataJSON("inventory_database");
        m_shards = JSONLoaderLR.LoadTable<ShardDataCollection>(tempJson["shards"]);
        m_shardColors = JSONLoaderLR.LoadTable< DataManager.ColorDataCollection>(tempJson["shard_colors"]);
    }
    
    public ActionData GetAttackActionData(string _id)
    {
        return m_attacks[_id];
    }

    public ActionData GetMagicActionData(string _id)
    {
        return m_magics[_id];
    }

    public ShardData GetShard(string _shardId)
    {
        return m_shards[_shardId];
    }

    public DataManager.ColorData GetShardColor(string _shardId)
    {
        var shard = GetShard(_shardId);
        return m_shardColors[shard.ColorId];
    }
    
    #region ACTIONDATA
    public class ActionData : JSONData
    {
        public string Name;
        public string Prefab;
        public int Power;
        public int MpCost;
        public bool Offense;
        public string Description;

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            Name = _json.GetField("name").str;
            Prefab = _json.GetField("prefab").str;
            Power = (int)_json.GetField("power").f;
            MpCost = (int)_json.GetField("mp").f;
            var desc = _json.GetField("description");
            if (desc != null)
                Description = desc.str;
            //type
            string strType = _json.GetField("type").str;
            Offense = strType == "offense" ? true : false;
        }
    }

    public class ActionDataCollection : IJSONDataDicoCollection<ActionData> { }

    #endregion

    #region ITEM_DATA
    public class ItemData : GameUtils.WeightableData
    {
        public string Name;
        public string Image;
        public string Description;

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            Name = _json.GetField("name").str;
            Image = _json.GetField("image").str;
            Description = _json.GetField("description").str;
        }
    }

    public class ItemDataCollection : IJSONDataDicoCollection<ItemData> { }

    public class ShardData : ItemData
    {
        public string ColorId;
        public List<Job> Compatibilities = new List<Job>();

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            ColorId = _json.GetField("color").str;
            //Compat
            Compatibilities.Add((Job)System.Enum.Parse(typeof(Job), _json.GetField("job").str.ToUpper()));
            var compat2 = _json.GetField("job2");
            if (compat2 != null)
            {
                Compatibilities.Add((Job)System.Enum.Parse(typeof(Job), compat2.str.ToUpper()));
            }
        }
                
    }

    public class ShardDataCollection : IJSONDataDicoCollection<ShardData> { }

    #endregion
}
