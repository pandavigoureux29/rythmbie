using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class DataCharManager : DatabaseLoader
{
    Dictionary<Job, LevelUpDataCollection> m_levelupsDB = new Dictionary<Job, LevelUpDataCollection>();
    Dictionary<EquipmentType, BuildDataCollection> m_equipmentsDB = new Dictionary<EquipmentType, BuildDataCollection>();
    Dictionary<LooksType, BuildDataCollection> m_looksDB = new Dictionary<LooksType, BuildDataCollection>();
    DataManager.ColorDataCollection m_colorsDB;

    JobEquipCompatibilitiesCollection jobs;
    Dictionary<Job, SkillGenerationDataCollection> skillsGenerations = new Dictionary<Job, SkillGenerationDataCollection>();
    Dictionary<Job, SkillGenerationDataCollection> talentsGenerations = new Dictionary<Job, SkillGenerationDataCollection>();

    protected override void LoadDatabase()
    {
        base.LoadDatabase();
        ReadLevelUp();
        ReadEquipement();
        ReadGeneration();        
    }

    #region DATABASE_READING
    void ReadLevelUp()
    {
        var database = LoadDataJSON("characters/characters_levelup_database");
        //foreach enum value
        for (int i = 0; i < Utils.EnumCount(Job.THIEF); i++)
        {
            Job job = (Job)i;
            string jobStr = job.ToString().ToLower();
            //Convert json into classes
            var coll = JSONLoaderLR.LoadTable<LevelUpDataCollection>(database[jobStr]);
            m_levelupsDB[job] = coll;
        }
    }

    void ReadEquipement()
    {
        var database = LoadDataJSON("characters/equipement_database");

        jobs = JSONLoaderLR.LoadTable<JobEquipCompatibilitiesCollection>(database["jobs"]);

        //equipement
        for (int i = 0; i < Utils.EnumCount(EquipmentType.ACCESSORY); i++)
        {
            EquipmentType equEnum = (EquipmentType)i;
            string equStr = equEnum.ToString().ToLower();
            var coll = JSONLoaderLR.LoadTable<BuildDataCollection>(database[equStr]);
            m_equipmentsDB[equEnum] = coll;
        }
        //looks
        for (int i = 0; i < Utils.EnumCount(LooksType.EYEBROWS); i++)
        {
            LooksType lookEnum = (LooksType)i;
            string lookStr = lookEnum.ToString().ToLower();
            var coll = JSONLoaderLR.LoadTable<BuildDataCollection>(database[lookStr]);
            m_looksDB[lookEnum] = coll;
        }
        var ld = database["body_colors"];
        m_colorsDB = JSONLoaderLR.LoadTable<DataManager.ColorDataCollection>(database["body_colors"]);
    }

    void ReadGeneration()
    {
        var database = LoadDataJSON("characters/mixi_generation_database");
        //foreach enum value
        for (int i = 0; i < Utils.EnumCount(Job.THIEF); i++)
        {
            Job job = (Job)i;
            string jobStr = job.ToString().ToLower();
            //skills
            var skillTable = database[jobStr + "_skills"];
            if (skillTable != null)
            {
                var skills = JSONLoaderLR.LoadTable<SkillGenerationDataCollection>(skillTable);
                skillsGenerations[job] = skills;
            }
            //talents
            var talentsTable = database[jobStr+"_talents"];
            if (talentsTable != null)
            {
                var talents = JSONLoaderLR.LoadTable<SkillGenerationDataCollection>(talentsTable);
                talentsGenerations[job] = talents;
            }
        }
    }
    #endregion

    public Stats ComputeStats(ProfileManager.CharacterData _charData)
    {
        Stats stats = new Stats();
        //Get Class Stats
        var levelupStats = GetLevelByXp(_charData.Job, _charData.Xp).Stats;
        stats.Add(levelupStats);
        return stats;
    }

    #region LEVEL

    public LevelUpData GetLevelByXp(Job _job, int _xp)
    {
        var lvls = m_levelupsDB[_job];
        if( lvls != null)
        {
            return lvls.GetByXp(_xp);
        }
        return null;
    }

    public LevelUpData GetLevel(Job _job, int _level)
    {
        var lvls = m_levelupsDB[_job];
        if (lvls != null)
        {
            return lvls[_level];
        }
        return null;
    }

    /// <summary>
    /// Returns the level data of the next level
    /// </summary>
    public LevelUpData GetNextLevelByXp(Job _job, int _xp)
    {
        var lvls = m_levelupsDB[_job];
        if (lvls != null)
        {
            return lvls.GetNextLevelByXp(_xp);
        }
        return null;
    }

    /// <summary>
    /// Returns the level data of the next level
    /// </summary>
    public LevelUpData GetNextLevel(Job _job, int _level)
    {
        var lvls = m_levelupsDB[_job];
        if (lvls != null)
        {
            return lvls[_level+1];
        }
        return null;
    }
    #endregion

    #region EQUIPEMENT
    
    public List<BuildData> GetEquipements(EquipmentType _type, Job _job, int _tiers = -1) 
    {
        var equipments = m_equipmentsDB[_type];
        var compats = jobs[_job.ToString().ToLower()].Compatibilities;
        return equipments.GetCompatibleBuilds(compats, _tiers);
    }
        
    public BuildData GetEquipement( EquipmentType _type, string _id)
    {
        var equipments = m_equipmentsDB[_type];
        return equipments[_id];
    }

    public BuildData GetEquipement(string _type, string _id)
    {
        EquipmentType enumType = (EquipmentType) System.Enum.Parse(typeof(EquipmentType), _type);
        return GetEquipement(enumType, _id);
    }

    public List<BuildData> GetLooks(LooksType _type, Job _job, int _tiers = -1)
    {
        var looks = m_looksDB[_type];
        var compats = jobs[_job.ToString().ToLower()].Compatibilities;
        return looks.GetCompatibleBuilds(compats, _tiers);
    }

    public BuildData GetLook( LooksType _type, string _id)
    {
        var looks = m_looksDB[_type];
        return looks[_id];
    }

    public BuildData GetLook(string _type, string _id)
    {
        LooksType lookType = (LooksType)System.Enum.Parse(typeof(LooksType), _type);
        return GetLook(lookType, _id);
    }

    public GameObject LoadAttackPrefab(string _weaponId)
    {
        BuildDataCollection weaponDB = m_equipmentsDB[EquipmentType.WEAPON];
        BuildData weapon = weaponDB[_weaponId];
        var prefab = Resources.Load( "prefabs/battle/attack/" + weapon.AttackPrefab );
        if (prefab == null)
            Debug.LogError("Couldn't Load " + "prefabs/battle/attack/" + weapon.AttackPrefab);
        GameObject go = Instantiate(prefab) as GameObject;
        return go;
    }

    #endregion

    #region COLOR

    public Color GetColor(string _colorId)
    {
        return m_colorsDB[_colorId].Color;
    }
    
    public List<DataManager.ColorData> GetColors(int _tiers)
    {
        return GameUtils.SearchByTiers(m_colorsDB.ToList(), _tiers,1);
    }

    #endregion

    #region SKILLS

    /// <summary>
    /// Get Skills of this tiers only
    /// </summary>
    public List<SkillGenerationData> GetSkills(Job _job, int _tiers)
    {
        var skillsColl = skillsGenerations[_job];
        return GameUtils.SearchByTiers(skillsColl.ToList(), _tiers);
    }

    public List<SkillGenerationData> GetTalents(Job _job, int _tiers)
    {
        var talents = talentsGenerations[_job];
        return GameUtils.SearchByTiers(talents.ToList(), _tiers);
    }
    
    #endregion   
    
    /// <summary>
    /// Used to hold data with job compatibilities ( compat and/or compat2 ). Also a WeightableData.
    /// </summary>
    public class EquipCompatibilityData : GameUtils.WeightableData
    {
        public List<EquipCompatibility> Compatibilities = new List<EquipCompatibility>();

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            //Compat
            Compatibilities.Add((EquipCompatibility)System.Enum.Parse(typeof(EquipCompatibility), _json.GetField("compat").str.ToUpper()));
            var compat2 = _json.GetField("compat2");
            if (compat2 != null)
            {
                Compatibilities.Add((EquipCompatibility)System.Enum.Parse(typeof(EquipCompatibility), compat2.str.ToUpper()));
            }
        }

        public bool IsCompatible(EquipCompatibility _type)
        {
            if (_type == EquipCompatibility.ALL)
                return true;
            foreach (var comp in Compatibilities)
                if (comp == EquipCompatibility.ALL || comp == _type)
                    return true;
            return false;
        }

        public bool IsCompatible(List<EquipCompatibility> _types)
        {
            foreach(var t in _types)
            {
                if (IsCompatible(t))
                    return true;
            }
            return false;
        }
    }     

    #region LEVEL_DATA
    /// <summary>
    /// Data used to stored levels 
    /// </summary>
    public class LevelUpData
    {
        public int XpNeeded = 0;
        public Stats Stats = new Stats();

        public LevelUpData(JSONObject _json)
        {
            XpNeeded = (int)_json.GetField("xp").f;
            Stats = new Stats(_json);
        }
    }

    public class LevelUpDataCollection : IJSONDataCollection
    {
        Dictionary<int, LevelUpData> levelups = new Dictionary<int, LevelUpData>();

        public void AddElement(JSONObject _element)
        {
            LevelUpData data = new LevelUpData(_element);
            levelups.Add(data.Stats.Level, data);
        }

        public IEnumerator<LevelUpData> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public LevelUpData GetByXp(int _xp)
        {
            LevelUpData level = null;
            foreach(var lvl in levelups)
            {
                if (lvl.Value.XpNeeded > _xp)
                    return level;
                level = lvl.Value;
            }
            return level;
        }

        public LevelUpData GetNextLevelByXp(int _xp)
        {
            foreach (var lvl in levelups)
            {
                if (lvl.Value.XpNeeded > _xp)
                    return lvl.Value;
            }
            return null;
        }

        public LevelUpData this[int i]
        {
            get
            {
                if (levelups.ContainsKey(i))
                    return levelups[i];
                return null;
            }
        }
    }

    #endregion

    #region BUILD_DATA

    public class BuildData : EquipCompatibilityData
    {
        public string Name = "NoName_Equipment";
        public string Prefab;
        public string AttackPrefab;

        public Stats Stats = new Stats();

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            Name = _json.GetField("name").str;
            Prefab = _json.GetField("prefab").str;
            if(_json.GetField("attackprefab"))
                AttackPrefab = _json.GetField("attackprefab").str;

            Stats = new Stats(_json);
        }
    }
       

    public class BuildDataCollection : IJSONDataDicoCollection<BuildData>
    {        
        public List<BuildData> GetCompatibleBuilds(List<EquipCompatibility> _compat,int _tiers)
        {
            var list = new List<BuildData>();
            foreach (var b in items.Values)
            {
                // we want to take tiers that are equals or one tiers down ( ex : for _tiers = 3, we can allow tiers 2 and 3 )
                bool isTiersCompatible = (b.Tiers <= _tiers && b.Tiers >= _tiers -1 ) || _tiers < 0 ;
                if (b.IsCompatible(_compat) && isTiersCompatible)
                {
                    list.Add(b);
                }
            }
            return list;
        }
    }

    #endregion
    
    #region SKILLS_GENERATION    

    public class SkillGenerationData : GameUtils.WeightableData
    {
        public Job Job;
        public string SkillId;

        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);
            SkillId = _json.GetField("skillid").str;
        }
    }

    public class SkillGenerationDataCollection : IJSONDataDicoCollection<SkillGenerationData> { }

    #endregion

    public class JobEquipCompatibilityData : EquipCompatibilityData
    {
        public override void BuildJSONData(JSONObject _json)
        {
            base.BuildJSONData(_json);            
        }        
    }

    public class JobEquipCompatibilitiesCollection : IJSONDataDicoCollection<JobEquipCompatibilityData> { }
}
