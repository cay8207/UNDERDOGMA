using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    protected Character _character;

    // 모든 상태에서는 기본적인 연산을 위해 타일의 정보가 필요하다. 
    protected Dictionary<Vector2Int, TileObject> TempTileDictionary => StageManager.Instance.TempTileDictionary;

    protected BaseState(Character character)
    {
        _character = character;
    }

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
