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
        // 1. 처형할 적을 찾는다.
        GameObject _executionTarget = Execution.Instance.ExecuteEnemies();

        // 2. 처형 애니메이션을 보여준다.
        _character.EnqueueCoroutine(_character.ExecutionEvent());

        // 3. 만약 죽어야 하는게 캐릭터라면 Death State로 넘어간다.
        if (_executionTarget == StageManager.Instance._character)
        {
            _character.ChangeState(Character.State.Death);
        }
        // 4. 만약 죽어야 하는게 적이라면 Idle State로 넘어간다.  
        else
        {
            _character.ChangeState(Character.State.Idle);
        }
    }

    public override void OnStateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Execution.Instance.ExecutionStop();
        }
    }

    public override void OnStateExit()
    {

    }
}
