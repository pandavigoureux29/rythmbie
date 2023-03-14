using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILifeBar : MonoBehaviour
{
    [SerializeField] private PlayerLifeManager m_lifeManager;

    [SerializeField] private Image m_barImage;
        
    private void Awake()
    {
        m_lifeManager.OnValueChangedEvent += OnLifeChanged;
    }

    private void OnDestroy()
    {
        m_lifeManager.OnValueChangedEvent -= OnLifeChanged;
    }

    void OnLifeChanged(LifeChangedEventData eventData)
    {
        m_barImage.fillAmount = (float) eventData.CurrentLife / eventData.MaxLife;
    }
}
