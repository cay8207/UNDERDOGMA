using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EndingDialogueState : BaseState
{
    public EndingDialogueState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {
    }

    public override void OnStateUpdate()
    {
        if (_character.IsCharacterCoroutineRunning == false)
        {
            if (DialogueManager.Instance._isDialogueRunning == false)
            {
                _character.EnqueueCoroutine(_character.StageClear());
                _character.ChangeState(Character.State.Idle);
            }
        }
    }

    public override void OnStateExit()
    {

    }
}
