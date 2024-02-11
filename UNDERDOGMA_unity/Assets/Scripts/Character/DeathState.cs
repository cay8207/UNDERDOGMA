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
        _character.ChangeState(Character.State.Reset);
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }


}
