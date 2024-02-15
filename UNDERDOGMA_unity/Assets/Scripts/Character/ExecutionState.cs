using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class ExecutionState : BaseState
{
    public ExecutionState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {

    }

    public override void OnStateUpdate()
    {
        if (_character.IsCharacterCoroutineRunning == false)
        {
            ExecuteEnemies();
        }
    }

    public override void OnStateExit()
    {

    }

    public void ExecuteEnemies()
    {
        // 1. 처형할 적을 찾는다.
        Dictionary<Vector2Int, GameObject> _executionTarget = Execution.Instance.ExecuteEnemies();

        foreach (var enemy in _executionTarget)
        {
            Debug.Log("(ExecutionState.cs) executionTarget: " + enemy.Value.name);
        }

        // 2. 만약 큐에 character가 있다면 Death State로 넘어가야 한다. 
        if (_executionTarget.ContainsValue(StageManager.Instance._character))
        {
            // 2.1. 캐릭터를 dictionary에서 삭제한다. 
            foreach (var gameObject in _executionTarget)
            {
                if (gameObject.Value == StageManager.Instance._character)
                {
                    _executionTarget.Remove(gameObject.Key);
                    break;
                }
            }

            // 2.2. 그리고 모든 적을 죽인다.
            foreach (var enemy in _executionTarget)
            {
                EnemyManager.Instance.EnemyDeath(enemy.Key, true);
            }

            _character.EnqueueCoroutine(_character.ExecutionEvent(_executionTarget));

            _character.ChangeState(Character.State.Death);
        }
        // 3. 만약 큐에 character가 없다면 적을 모두 처형한다. 
        else
        {
            foreach (var enemy in _executionTarget)
            {
                EnemyManager.Instance.EnemyDeath(enemy.Key, true);
            }

            _character.EnqueueCoroutine(_character.ExecutionEvent(_executionTarget));

            _character.ChangeState(Character.State.Clear);
        }
    }
}
