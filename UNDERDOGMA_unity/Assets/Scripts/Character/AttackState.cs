using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AttackState : BaseState
{
    Vector2Int _targetPosition;
    bool _isCharacterAttackCoroutineRunning = false;
    public AttackState(Character character, Vector2Int targetPosition) : base(character)
    {
        _targetPosition = targetPosition;
    }

    // 해당 state에서는 공격을 하는 부분만 있다. 공격이 가능한지 여부는 character 의 코드에서 판단해서 넘겨준다.
    public override void OnStateEnter()
    {
        // 1. 캐릭터가 공격하는 애니메이션을 보여주고, 캐릭터의 체력을 변화시킨다. 
        _character.EnqueueCoroutine(_character.CharacterAttack(_targetPosition));
        _character.HeartChange(TempTileDictionary[_targetPosition].EnemyData.Heart);

        // 2. 행동이 끝났으니 행동 카운트를 증가시켜준다. 
        _character.MoveCount++;

        // 3. 먄약 공격하는 코루틴이 끝났고
        if (_isCharacterAttackCoroutineRunning == false)
        {
            // 3.1. 행동 횟수가 처형 카운트를 넘었다면 처형을 진행한다. ExecutionState로 이동. 
            if (Execution.Instance.ExecutionCheck(_character.MoveCount))
            {
                _character.MoveCount = 0;
                _character.ChangeState(Character.State.Execution);
            }
            // 3.2. 아니라면 게임이 클리어되었는지 체크하는 Clear State로 넘어간다. 
            else
            {
                _character.ChangeState(Character.State.Clear);
            }
        }
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }
}
