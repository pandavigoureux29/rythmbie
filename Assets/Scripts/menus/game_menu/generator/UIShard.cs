using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class UIShard : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] Image m_imageRenderer;
    [SerializeField] Text m_quantityText;

    GameMenuMixiGenerator m_menu;
    
    string m_shardId;

	public void LoadShard(string _shardId, int _quantity, GameMenuMixiGenerator _menu)
    {
        m_shardId = _shardId;
        var shardColor = DataManager.instance.InventoryManager.GetShardColor(_shardId);
        m_imageRenderer.color = shardColor.Color;
        RefreshQuantity(_quantity);
        m_menu = _menu;
    }

    public void RefreshQuantity(int quantity)
    {
        m_quantityText.text = "" + quantity;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_menu.OnSelectShard(ShardId);
    }

    public string ShardId { get { return m_shardId; } }
}
