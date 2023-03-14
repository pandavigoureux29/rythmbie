using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeManager : MonoBehaviour
{
    [SerializeField] private int m_maxLife;

    private int m_currentLife;

    public Action<LifeChangedEventData> OnValueChangedEvent;

    public void Awake()
    {
        m_currentLife = m_maxLife;
        LR.EventDispatcher.Instance.Subscribe<NoteAttackEventData>(OnNoteAttack);
    }

    private void OnNoteAttack(NoteAttackEventData eventData)
    {
        m_currentLife -= eventData.Note.Damage;
        var lifeChangedEventData = new LifeChangedEventData { MaxLife = m_maxLife, CurrentLife = m_currentLife };
        OnValueChangedEvent?.Invoke(lifeChangedEventData);
        LR.EventDispatcher.Instance.Publish(lifeChangedEventData);
    }
}
