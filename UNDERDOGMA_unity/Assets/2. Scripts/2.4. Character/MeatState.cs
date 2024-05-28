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
    }

    public override void OnStateUpdate()
    {
        // 고기를 먹은 후 주변에 적이 있다면 데미지를 받아야 한다. 
        if (_character.IsCharacterCoroutineRunning == false)
        {
            _character.ChangeState(Character.State.Damaged);
        }
    }

    public override void OnStateExit()
    {

    }
}
