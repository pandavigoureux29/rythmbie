using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DatabaseLoader : MonoBehaviour {
    
    // Use this for initialization
    virtual protected void Awake () {
        LoadDatabase();
	}
	    
    protected virtual void LoadDatabase()
    {     
    }

    protected JSONObject LoadDataJSON(string _fileName)
    {
        TextAsset json = Resources.Load("database/" + _fileName) as TextAsset;
        //PArse JSON
        JSONObject jsonData = new JSONObject(json.text);
        Resources.UnloadAsset(json);
        return jsonData;
    }
    
}
