using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MeatState : BaseState
{
    public Vector2Int _targetPosition;

    public MeatState(Character character, Vector2Int targetPosition) : base(character)
    {
        _targetPosition = targetPosition;
    }

    public override void OnStateEnter()
    {
        _character.EnqueueCoroutine(_character.CharacterEatMeat(_targetPosition));
        _character.ChangeState(Character.State.Idle);
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }
}
