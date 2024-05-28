using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class ExecutionState : BaseState
{
    public Character.State _nextState;

    public bool isCharacterExecuted = false;

    public ExecutionState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {
        isCharacterExecuted = false;

        ExecuteEnemies();
    }

    public override void OnStateUpdate()
    {
        // 1. 캐릭터의 코루틴이 끝났다면
        if (_character.IsCharacterCoroutineRunning == false)
        {
            // 1.1. 즉, 처형이 끝났다면 게임이 클리어되었는지를 확인한다. 
            if (StageManager.Instance.StageClearCheck() && isCharacterExecuted == false)
            {
                _nextState = Character.State.EndingDialogueState;
            }

            // 1.2. 다음 상태에 따라 state를 변경해준다. 
            if (_nextState == Character.State.Death)
            {
                _character.ChangeState(Character.State.Death);
            }
            else if (_nextState == Character.State.Idle)
            {
                _character.ChangeState(Character.State.Idle);
            }
            else if (_nextState == Character.State.EndingDialogueState)
            {
                _character.ChangeState(Character.State.EndingDialogueState);
            }
        }
    }

    public override void OnStateExit()
    {

    }

    public void ExecuteEnemies()
    {
        // 1. 처형할 적을 찾는다.
        Dictionary<Vector2Int, GameObject> _executionTarget = ExecutionManager.Instance.FindExecutionTargets();

        foreach (var enemy in _executionTarget)
        {
            Debug.Log("(ExecutionState.cs) executionTarget: " + enemy.Value.name);
        }

        // 2. 만약 큐에 character가 있다면 Death State로 넘어가야 한다. 
        if (_executionTarget.ContainsValue(StageManager.Instance._character))
        {
            // 2.2. 그리고 모든 적을 죽인다.
            foreach (var enemy in _executionTarget)
            {
                if (enemy.Value != StageManager.Instance._character)
                {
                    EnemyManager.Instance.EnemyDeath(enemy.Key, true);
                }
            }

            _character.EnqueueCoroutine(_character.ExecutionEvent(_executionTarget));

            _nextState = Character.State.Death;

            isCharacterExecuted = true;
        }
        // 3. 만약 큐에 character가 없다면 적을 모두 처형한다. 
        else
        {
            foreach (var enemy in _executionTarget)
            {
                EnemyManager.Instance.EnemyDeath(enemy.Key, true);
            }

            _character.EnqueueCoroutine(_character.ExecutionEvent(_executionTarget));
        }

        // 4. 캐릭터의 이동 카운트를 0으로 초기화한다. 
        _character.MoveCount = 0;
    }
}
