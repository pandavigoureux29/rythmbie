using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEndCharInfoUI : MonoBehaviour {
    
    public GameObject CharacterObject;
    public Text LevelText;
    public Text XpText;
    public UIFilledGauge Gauge;
    public UIXpScrollerManager XpScroller;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetGaugeValue(float _value)
    {
        Gauge.SetValue(_value);
    }
}
