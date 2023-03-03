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

    public static GameObject CreateCharacterUIObject(string _id, float _scale, bool _draggable = false)
    {
        var charaData = ProfileManager.instance.GetCharacter(_id);
        return CreateCharacterUIObject(charaData, _scale, _draggable);
    }
    /// <summary>
    /// Create an gameobject of a character for UI purposes ( Image instead of Sprite )
    /// </summary>
    public static GameObject CreateCharacterUIObject(ProfileManager.CharacterData _charaData, float _scale, bool _draggable = false)
    {
        //Create character
        GameObject character = DataManager.instance.CreateCharacter(_charaData);
        //convert to ui
        Utils.SetLayerRecursively(character, LayerMask.NameToLayer("SpriteUI"));
        Utils.ConvertToUIImage(character);
        //Set Parent
        GameObject container = new GameObject("Char_" + _charaData.Id);
        Utils.SetLocalScaleXY(character.transform, _scale, _scale);
        var rect = container.AddComponent<RectTransform>();
        character.transform.SetParent(container.transform, false);
        //Set Draggable
        var uiItem = container.AddComponent<UIInventoryDraggableItem>();
        uiItem.IsDraggable = _draggable;
        uiItem.ItemParentTransform = character.transform;
        uiItem.CharId = _charaData.Id;
        return container;
    }

}
