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
        StartCoroutine(CharacterAttack(_targetPosition));
        _character.HeartChange(TempTileDictionary[_targetPosition].EnemyData.Heart);

        // 2. 행동이 끝났으니 행동 카운트를 증가시켜준다. 
        _character.MoveCount++;

        // 3. 먄약 공격하는 코루틴이 끝났고
        if (_isCharacterAttackCoroutineRunning == false)
        {
            // 3.1. 처형되어야 하는 상태라면 처형한다. ExecutionState로 이동. 
            if (ExecutionManager.Instance.ExecutionCheck(_character.MoveCount))
            {
                _character.MoveCount = 0;
                _character.ChangeState(Character.State.Execution);
            }
            // 3.2. 아니라면 다시 Idle 상태로 바꿔준다. 
            else
            {
                _character.ChangeState(Character.State.Idle);
            }
        }
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }

    // 캐릭터가 공격하는 애니메이션을 보여준다. 
    public IEnumerator CharacterAttack(Vector2Int targetPosition)
    {
        _isCharacterAttackCoroutineRunning = true;

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Eat);

        // 캐릭터가 공격하는 애니메이션 재생. 
        _character.GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.9f);

        _character.GetComponent<Animator>().SetBool("IsAttack", false);

        // 공격한 적을 죽인다. 
        EnemyManager.Instance.EnemyDeath(targetPosition);

        _isCharacterAttackCoroutineRunning = false;
    }
}
