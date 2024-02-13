using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AttackState : BaseState
{
    Vector2Int _targetPosition;
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

        // 3. 캐릭터가 이동했으니 상단의 눈이 하나 떠지도록 하는 코드.
        Execution.Instance.ExecutionCheck(_character.MoveCount);

        _character.ChangeState(Character.State.Clear);
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }
}
