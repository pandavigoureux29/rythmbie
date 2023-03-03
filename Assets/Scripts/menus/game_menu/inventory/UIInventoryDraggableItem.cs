using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIInventoryDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{    
    [SerializeField] private bool m_draggable = true;
    Vector3 m_initialPosition;

    GameMenuMixisInventory m_menu;

    Transform m_itemParentTransform;

    private string m_charId;
    
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData _eventData)
    {
        m_menu.SelectCharacter(this);
    }

    public void OnBeginDrag(PointerEventData _eventData)
    {
        m_initialPosition = transform.localPosition;
    }

    public void OnDrag(PointerEventData _eventData)
    {
        if (IsDraggable)
        {
            transform.position = _eventData.position;
            m_menu.OnInventoryItemDrag(this);
        }
    }

    public void OnEndDrag(PointerEventData _eventData)
    {
        if (IsDraggable == false)
            return;
        var dropped = m_menu.OnInventoryItemDrop(this);
        transform.localPosition = m_initialPosition;
        if (!dropped)
            m_menu.SelectCharacter(this);
    }

    public bool IsDraggable
    {
        get { return m_draggable; }
        set { m_draggable = value; }
    }

    public GameMenuMixisInventory Menu
    {
        get { return m_menu; }
        set { m_menu = value; }
    }

    public Transform ItemParentTransform
    {
        get { return m_itemParentTransform; }
        set { m_itemParentTransform = value; }
    }

    public string CharId
    {
        get { return m_charId; }
        set
        {
            m_charId = value;
            ItemParentTransform.name = m_charId;
            transform.name = "Char_" + m_charId;
        }
    }
}

