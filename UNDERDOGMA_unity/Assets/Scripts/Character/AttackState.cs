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
        StartCoroutine(CharacterAttack(_targetPosition));
        _character.HeartChange(TempTileDictionary[_targetPosition].EnemyData.Attack);

        // 먄약 공격하는 코루틴이 끝났고
        if (_isCharacterAttackCoroutineRunning == false)
        {
            // 처형되어야 하는 상태라면 처형한다.
            if (ExecutionManager.Instance.ExecutionCheck(_character.MoveCount))
            {
                _character.MoveCount = 0;
                _character.ChangeState(Character.State.Execution);
            }
            // 아니라면 다시 Idle 상태로 바꿔준다. 
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
