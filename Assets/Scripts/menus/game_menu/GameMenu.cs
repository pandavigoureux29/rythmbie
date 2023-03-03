using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    protected bool m_alreadyActivated = false;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ActivateMenu()
    {
        Activate();
        m_alreadyActivated = true;
    }

    protected virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        Deactivate();
    }

    protected virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
