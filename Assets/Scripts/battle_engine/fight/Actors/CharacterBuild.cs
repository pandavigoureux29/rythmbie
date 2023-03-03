using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CharacterBuild : MonoBehaviour {
    
    [SerializeField] SpriteRenderer m_body;
    [SerializeField] SpriteRenderer m_eyebrows;
    [SerializeField] SpriteRenderer m_arm;
    [SerializeField] SpriteRenderer m_eyes;

    [SerializeField] Transform m_equipmentsGO;
    Dictionary<EquipmentType, GameObject> m_equipments = new Dictionary<EquipmentType, GameObject>();

    public string Id { get; set; }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    #region RENDERING

    public void SetColor(string _colorId)
    {
        var color = DataManager.instance.CharacterManager.GetColor(_colorId);
        m_body.color = color;
        m_arm.color = color;
    }

    public void Load(string _characterId)
    {
        var chara = ProfileManager.instance.GetCharacter(_characterId);
        LoadEquipment(chara);
        LoadAppearance(chara);
    }

    public void Load(ProfileManager.CharacterData _chara)
    {
        Id = _chara.Id;
        LoadEquipment(_chara);
        LoadAppearance(_chara);
    }

    void LoadEquipment(ProfileManager.CharacterData _chara)
    {        
        foreach( var equ in _chara.Equipments)
        {
            if (equ == null)
                continue;
            var eqData = DataManager.instance.CharacterManager.GetEquipement(equ.EquipmentType, equ.Id);
            if( eqData != null)
            {
                string pathToPrefab = "prefabs/equipments/" + equ.EquipmentType.ToString().ToLower();
                pathToPrefab += "/" + eqData.Prefab;
                //Load prefab
                GameObject go = Instantiate(Resources.Load(pathToPrefab)) as GameObject;
                if( go != null)
                {
                    go.transform.SetParent(m_equipmentsGO,false) ;
                }
            }
        }
    }

    void LoadAppearance(ProfileManager.CharacterData _chara)
    {
        foreach( var look in _chara.Looks )
        {
			if (string.IsNullOrEmpty(look.Id))
                continue;
            var lookData = DataManager.instance.CharacterManager.GetLook(look.LooksType, look.Id);
            if( lookData != null)
            {
                string pathToPrefab = "prefabs/looks/" + look.LooksType.ToString().ToLower();
                pathToPrefab += "/" + lookData.Prefab;
                var prefab = Resources.Load(pathToPrefab);
                if( prefab == null)
                {
                    Debug.LogError("Prefab not found at : " + pathToPrefab);
                    continue;
                }
                var go = Instantiate(prefab) as GameObject;
                switch(look.LooksType)
                {
                    case LooksType.EYES:
                        Destroy(m_eyes.gameObject);
                        go.transform.SetParent(this.transform,false);
                        m_eyes = go.GetComponent<SpriteRenderer>();
                        break;
                    case LooksType.EYEBROWS:
                        Destroy(m_eyebrows.gameObject);
                        go.transform.SetParent(this.transform,false);
                        m_eyebrows = go.GetComponent<SpriteRenderer>();
                        break;
                    case LooksType.FACE:
                        go.transform.SetParent(this.transform,false);
                        break;
                }
            }
        }
        //color
        SetColor(_chara.ColorId);
    }

    #endregion

    #region PROPERTIES

    public SpriteRenderer Arm { get { return m_arm; } }
    public SpriteRenderer Eyebrows { get { return m_eyebrows; } }
    public SpriteRenderer Body { get { return m_body; } }
    public SpriteRenderer Eyes { get { return m_eyes; } }

    #endregion
}
