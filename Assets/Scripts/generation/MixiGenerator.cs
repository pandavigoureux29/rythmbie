using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MixiGenerator {
    
    DataCharManager CharManager { get { return DataManager.instance.CharacterManager; } }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public ProfileManager.CharacterData Generate( string _shardId, int _quantity = 1 )
    {
        var shard = DataManager.instance.InventoryManager.GetShard(_shardId);
        int r = Random.Range(0, shard.Compatibilities.Count -1);
        Job job = shard.Compatibilities[r];
        Debug.Log("Job " + job.ToString());

        //Decrement profile values
        ProfileManager.instance.RemoveShard(_shardId, _quantity);

        return Generate(shard.Tiers, job);
    }

    public ProfileManager.CharacterData Generate(int _tiers, Job _job)
    {
        string id = "Mixi" + ProfileManager.instance.profile.Characters.Count;
        ProfileManager.CharacterData chara = new ProfileManager.CharacterData(id);

        chara.Job = _job;
        chara.Tiers = _tiers;
        //Color
        var colors = CharManager.GetColors(_tiers);
        chara.ColorId = GameUtils.GetRandom(colors).Id;
        
        SetEquipement(chara, _job);
        SetLooks(chara, _job);
        SetSkills(chara, _job);

        return chara;
    }
    
    void SetEquipement(ProfileManager.CharacterData _chara, Job _job)
    {
        //For each type of equipement
        for (int i = 0; i < Utils.EnumCount(EquipmentType.ACCESSORY); i++)
        {
            EquipmentType type = (EquipmentType)i;
            var randomEqpmnt = GetRandomEquipment(_job, type,_chara.Tiers);
            _chara.AddEquipement(randomEqpmnt.Id, type);
        }
    }

    void SetLooks(ProfileManager.CharacterData _chara, Job _job)
    {
        //For each type of skills
        for (int i = 0; i < Utils.EnumCount(LooksType.EYEBROWS); i++)
        {
            LooksType type = (LooksType)i;
            var randomLooks = GetRandomLooks(_job, type, _chara.Tiers);
            _chara.AddLooks(randomLooks.Id, type);
        }
    }

    void SetSkills(ProfileManager.CharacterData _chara, Job _job)
    {
        var charManager = DataManager.instance.CharacterManager;
        //take two skills from the lowest tiers possible
        int minTiers = _chara.Tiers - 1 < 1 ? 1 : _chara.Tiers - 1;
        for(int i = minTiers; i <= _chara.Tiers; ++i)
        {
            var skills = charManager.GetSkills(_job, _chara.Tiers);
            if (skills.Count <= 0)
                continue;
            int r = Random.Range(0, skills.Count - 1);
            var skill = skills[r];
            _chara.AddSkills(skill.SkillId);
        }
        //Add a talent
        var talents = charManager.GetTalents(_job, _chara.Tiers);
        int r2 = Random.Range(0, talents.Count - 1);
        _chara.Talent = talents[r2].SkillId;
    }

    #region CHARACTER_GENERATION
    
    public DataCharManager.BuildData GetRandomLooks(Job _job, LooksType _type, int _tiers)
    {
        var listOfLooks = CharManager.GetLooks(_type, _job, _tiers);
        int r = Random.Range(0, listOfLooks.Count - 1);
        return listOfLooks[r];
    }

    public DataCharManager.BuildData GetRandomEquipment(Job _job, EquipmentType _type, int _tiers)
    {
        var listOfEquipements = CharManager.GetEquipements(_type, _job, _tiers);
        int r = Random.Range(0, listOfEquipements.Count - 1);
        return listOfEquipements[r];
    }

    #endregion
}
