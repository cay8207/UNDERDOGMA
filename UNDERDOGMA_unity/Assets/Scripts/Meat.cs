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

    public void EatMeat(Vector2Int targetPosition)
    {
        // 1. 고기 오브젝트를 파괴시켜준다.
        Destroy(StageManager.Instance.GameObjectDictionary[targetPosition]);

        // 2. 데이터 업데이트를 위해 TempTileDictionary의 값을 변경해준다. 
        StageManager.Instance.TempTileDictionary[targetPosition].MeatData.IsExist = false;

        // 3. StageManager의 GameObjectDictionary에서도 제거해준다. 
        StageManager.Instance.GameObjectDictionary.Remove(targetPosition);
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