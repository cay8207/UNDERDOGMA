using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Meat : MonoBehaviour
{
    [SerializeField] private GameObject _heartText;

    private Dictionary<Vector2Int, GameObject> MeatDictionary => StageManager.Instance.MeatDictionary;
    private int _heart;

    public int Heart
    {
        get => _heart;
        set => _heart = value;
    }

    void Start()
    {
        _heartText.GetComponent<TextMeshPro>().text = _heart.ToString();
    }

    public void EatMeat(Vector2Int targetPosition)
    {
        Destroy(MeatDictionary[targetPosition]);
        StageManager.Instance._stageData.TileDictionary[targetPosition][1] = 0;
        MeatDictionary.Remove(targetPosition);
    }
}