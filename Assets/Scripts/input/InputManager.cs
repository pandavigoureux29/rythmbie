using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static InputManager _instance;

    public EventHandler<EventArgs> OnMousePressedDown;

	// Use this for initialization
	void Awake () {
        _instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (Utils.MouseJustPressed())
        {
            if (OnMousePressedDown != null)
                OnMousePressedDown.Invoke(this, new EventArgs());
        }
	}

    public static InputManager instance
    {
        get
        {
            return _instance;
        }
    }
}
