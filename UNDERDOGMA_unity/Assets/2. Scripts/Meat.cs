using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Meat : MonoBehaviour
{
    [SerializeField] private GameObject _amountText;

    private int _amount;

    public int Amount
    {
        get => _amount;
        set => _amount = value;
    }

    void Start()
    {
        _amountText.GetComponent<Text>().SetText(_amount);
    }

    public IEnumerator MeatEat(Vector2Int targetPosition)
    {
        // gameObject.GetComponent<SpriteRenderer>().sprite = _deadDog;
        StageManager.Instance.TempTileDictionary[targetPosition].MeatData.IsExist = false;

        gameObject.GetComponentInChildren<SpriteRenderer>().DOFade(0, 0.5f);

        yield return new WaitForSeconds(0.5f);

        Destroy(StageManager.Instance.GameObjectDictionary[targetPosition]);
        StageManager.Instance.GameObjectDictionary.Remove(targetPosition);

        yield return null;
    }
}