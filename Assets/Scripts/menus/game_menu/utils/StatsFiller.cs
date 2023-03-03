using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsFiller : MonoBehaviour {
                              
    [SerializeField] Text m_hpText;
    [SerializeField] Text m_mpText;
    [SerializeField] Text m_attackText;
    [SerializeField] Text m_defenseText;
    [SerializeField] Text m_magicText;
    [SerializeField] Text m_speedText;

    // Use this for initialization
    void Start () {
	
	}

    public void Load(Stats _stats)
    {
        FillStat( m_hpText , _stats.HP);
        FillStat( m_mpText , _stats.MP);
        FillStat(m_attackText, _stats.Attack);
        FillStat(m_defenseText, _stats.Defense);
        FillStat(m_magicText, _stats.Magic);
        FillStat(m_speedText, _stats.Speed);
    }

    public void Load(Stats _stats, Stats _comparingStats)
    {
        FillStat(m_hpText, _stats.HP, GetColor( _stats.HP- _comparingStats.HP));
        FillStat(m_mpText, _stats.MP, GetColor( _stats.MP - _comparingStats.MP));
        FillStat(m_attackText, _stats.Attack, GetColor( _stats.Attack - _comparingStats.Attack));
        FillStat(m_defenseText, _stats.Defense, GetColor( _stats.Defense - _comparingStats.Defense));
        FillStat(m_magicText, _stats.Magic, GetColor( _stats.Magic - _comparingStats.Magic));
        FillStat(m_speedText, _stats.Speed, GetColor( _stats.Speed - _comparingStats.Speed));
    }

    public void Empty()
    {
        FillStat(m_hpText, "-", GetColor(0));
        FillStat(m_mpText, "-", GetColor(0));
        FillStat(m_attackText, "-", GetColor(0));
        FillStat(m_defenseText, "-", GetColor(0));
        FillStat(m_magicText, "-", GetColor(0));
        FillStat(m_speedText, "-", GetColor(0));
    }

    void FillStat( Text _text, object _value)
    {
        if (_text != null)
        {
            _text.text = _value.ToString();
        }
    }

    void FillStat(Text _text, object _value, Color _color)
    {
        if (_text != null)
        {
            _text.text = _value.ToString();
            _text.color = _color;
        }
    }

    Color GetColor(int _value)
    {
        if (_value == 0)
            return Color.black;
        return _value > 0 ? Color.green : Color.red;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
