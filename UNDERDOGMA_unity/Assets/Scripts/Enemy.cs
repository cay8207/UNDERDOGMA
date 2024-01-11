using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject _attackText;
    [SerializeField] GameObject _heartText;

    private int _attack;
    public int Attack
    {
        get => _attack;
        set => _attack = value;
    }

    private int _heart;

    public int Heart
    {
        get => _heart;
        set => _heart = value;
    }

    public Enemy(int attack, int heart)
    {
        this._attack = attack;
        this._heart = heart;
    }

    // Start is called before the first frame update
    void Start()
    {
        _attackText.GetComponent<TextMeshPro>().text = _attack.ToString();
        _heartText.GetComponent<TextMeshPro>().text = _heart.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
