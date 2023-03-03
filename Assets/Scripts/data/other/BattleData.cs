using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class BattleData {

    public int TotalXp = 0;

    public List<PlayerData> Characters = new List<PlayerData>();

    public List<string> Shards = new List<string>();

    public int NotesCount = 0;
    public int TotalScore = 0;
    public Dictionary<HitAccuracy, int> NotesCountByAccuracy;

    public void AddPlayerData(string _id,int _xpStart, int _xpGained)
    {
        Characters.Add(new PlayerData(_id, _xpStart,_xpGained) ) ;
    }	

    /// <summary>
    /// Returns the PlayerData of the character for the battle. It contains character info for this battle
    /// </summary>
    public PlayerData GetCharacter(string _id)
    {
        return Characters.FirstOrDefault(x => x.Id == _id);
    }

    [System.Serializable]
    public class PlayerData
    {
        public string Id;
        public int XpStart = 0;
        public int XpGained = 0;

        public PlayerData(string _id,int _xpStart, int _xpGained)
        {
            Id = _id;
            XpStart = _xpStart;
            XpGained = _xpGained;
        }
    }
}
