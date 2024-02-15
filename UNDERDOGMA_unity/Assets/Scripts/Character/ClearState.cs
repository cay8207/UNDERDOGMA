using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ClearState : BaseState
{
    public ClearState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {

    }

    public override void OnStateUpdate()
    {
        if (_character.IsCharacterCoroutineRunning == false)
        {
            if (StageClearCheck() == true)
            {
                _character.EnqueueCoroutine(_character.StageClear());
                _character.ChangeState(Character.State.Idle);
            }
            else
            {
                _character.ChangeState(Character.State.Idle);
            }
        }
    }

    public override void OnStateExit()
    {

    }

    public bool StageClearCheck()
    {
        Debug.Log("GameObject.Count: " + StageManager.Instance.GameObjectDictionary.Count);

        int _row = 0;
        int _col = 0;
        int _enemyCount = 0;

        foreach (var gameObject in StageManager.Instance.GameObjectDictionary)
        {
            _row = gameObject.Key.x;
            _col = gameObject.Key.y;

            if (StageManager.Instance.TempTileDictionary[new Vector2Int(_row, _col)].Type == TileType.Enemy)
            {
                if (StageManager.Instance.TempTileDictionary[new Vector2Int(_row, _col)].EnemyData.IsAlive == true)
                {
                    _enemyCount++;
                }

            }
        }

        if (_enemyCount == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
