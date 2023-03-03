using UnityEngine;
using System.Collections;

public class DebugMixiRandomizer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var charamanager = DataManager.instance.CharacterManager;
        var profile = ProfileManager.instance.profile;
        
        for(int i = 0; i < 3; i++)
        {
            /*var newchara = charamanager.GenerateCharacter((Job)i);
            newchara.Id = "" + i;
            profile.Characters[i] = newchara;
            profile.CurrentTeam[i] = newchara.Id;*/
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
