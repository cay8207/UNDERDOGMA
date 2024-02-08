using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class IdleState : BaseState
{
    public IdleState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {

    }

    public override void OnStateUpdate()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            StartCoroutine(CharacterMove(0));
            if (ExecutionManager.Instance.ExecutionCheck(_moveCount)) _moveCount = 0;
        }
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            StartCoroutine(CharacterMove(1));
            if (ExecutionManager.Instance.ExecutionCheck(_moveCount)) _moveCount = 0;
        }
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            StartCoroutine(CharacterMove(2));
            if (ExecutionManager.Instance.ExecutionCheck(_moveCount)) _moveCount = 0;
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            StartCoroutine(CharacterMove(3));
            if (ExecutionManager.Instance.ExecutionCheck(_moveCount)) _moveCount = 0;
        }
    }

    public override void OnStateExit()
    {

    }
}
