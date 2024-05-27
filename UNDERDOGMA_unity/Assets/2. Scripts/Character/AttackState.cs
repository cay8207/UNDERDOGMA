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
    }

    public override void OnStateUpdate()
    {
        if (_character.IsCharacterCoroutineRunning == false)
        {
            // 1. 행동이 끝났으니 행동 카운트를 증가시켜준다. 
            _character.MoveCount++;

            // 2. 만약 남은 적이 없다면 EndingDialogue로 이동한다. 
            if (StageManager.Instance.StageClearCheck())
            {
                DialogueManager.Instance.Init(DialogueEvent.Ending, GameManager.Instance.Language, GameManager.Instance.World, GameManager.Instance.Stage);
                _character._keyDownQueue.Clear();
                _character.ChangeState(Character.State.EndingDialogueState);
            }
            // 3. 만약 남은 적이 있다면 캐릭터의 모든 큐를 제거하고 DamagedState로 이동한다.
            else
            {
                _character._keyDownQueue.Clear();
                _character.ChangeState(Character.State.Damaged);
                return;
            }
        }
    }

    public override void OnStateExit()
    {

    }
}
