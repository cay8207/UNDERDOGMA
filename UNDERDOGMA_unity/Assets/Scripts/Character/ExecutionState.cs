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
        Coroutine _executionCoroutine = StartCoroutine(ExecutionManager.Instance.ExecutionEvent());
    }

    public override void OnStateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ExecutionManager.Instance.ExecutionStop();
        }
    }

    public override void OnStateExit()
    {

    }
}
