using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DeathState : BaseState
{
    public DeathState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {
        _character.EnqueueCoroutine(_character.CharacterDeath());
    }

    public override void OnStateUpdate()
    {
        if (_character.IsCharacterCoroutineRunning == false)
        {
            _character.ChangeState(Character.State.Reset);
        }
    }

    public override void OnStateExit()
    {

    }


}
