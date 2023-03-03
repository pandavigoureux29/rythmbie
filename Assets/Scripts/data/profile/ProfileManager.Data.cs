using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class ProfileManager {
    
    [System.Serializable]
    public class CharacterData
    {
        public string Id;
        public string Name = "temp";
        public int Xp = 0;
        public Job Job;
        public int Tiers = 1;

        //public CharacterBuild Build; // skins

        //Equipment
        public List<EquipmentData> Equipments;
        //Appearance
        public List<LooksData> Looks;
        
        //Magics
        public List<SkillData> Skills;

        public string Talent = "0";

        public string ColorId = "0";

        public CharacterData(string _id)
        {
            Id = _id;
            Equipments = new List<EquipmentData>();
            Looks = new List<LooksData>();
            Skills = new List<SkillData>();
        }

        public string GetEquipmentId(EquipmentType _equipmentType)
        {
            var weapon = Equipments.Find(x => x.EquipmentType == _equipmentType);
            if (weapon == null)
                Debug.Log("no weapon of type "+ _equipmentType.ToString() +" found for " + Id);
            return weapon.Id;
        }

        public void AddEquipement(string _id, EquipmentType _type)
        {
            Equipments.Add(new EquipmentData(_id, _type));
        }
        
        public void AddLooks(string _id, LooksType _type)
        {
            Looks.Add(new LooksData(_id, _type));
        }

        public void AddSkills(string _id)
        {
            Skills.Add(new SkillData(_id));
        }

        [System.Serializable]
        public class EquipmentData
        {
            public string Id;
            public EquipmentType EquipmentType;

            public EquipmentData(string id, EquipmentType _type)
            {
                Id = id;
                EquipmentType = _type;
            }

        }

        [System.Serializable]
        public class LooksData
        {
            public string Id;
            public LooksType LooksType;

            public LooksData(string id, LooksType _type)
            {
                Id = id;
                LooksType = _type;
            }
        }

        [System.Serializable]
        public class SkillData
        {
            public string Id;
            public string Ap;
            public bool equipped = false;

            public SkillData(string _id)
            {
                Id = _id;
            }
        }
    }

    [System.Serializable]
    public class Map
    {
        public string Name = "";
        public List<Level> Levels = new List<Level>();

        public Map(string _name)
        {
            Name = _name;
        }

        [System.Serializable]
        public class Level
        {
            public string Id;
            public int Score = 0;
            public int WinCount = 0;

            public Level(string _id)
            {
                Id = _id;
            }
        }
    }

    [System.Serializable]
    public class Item
    {
        public string Id;
        public int Quantity;
    }

}
