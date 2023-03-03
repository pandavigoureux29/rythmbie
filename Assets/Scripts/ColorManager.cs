using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorManager : MonoBehaviour
{
    private static ColorManager _instance;

    [SerializeField] List<ColorStruct> colors;
    [System.Serializable]
    public struct ColorStruct
    {
        public string name;
        public Color color;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Color GetColor(string _name)
    {
        for(int i=0; i < colors.Count; i++)
        {
            if( colors[i].name == _name)
            {
                return colors[i].color;
            }
        }
        return Color.white;
    }

    public static ColorManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject newGO = Resources.Load("prefabs/other/ColorManager") as GameObject;
                _instance = newGO.GetComponent<ColorManager>();
            }
            return _instance;
        }
    }
}
