using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ResetState : BaseState
{
    public ResetState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {
        // 1. 짐승의 입이 잡아먹는듯한 애니메이션을 실행시켜준다.
        _character.EnqueueCoroutine(_character.ResetAnimation());
    }

    public override void OnStateUpdate()
    {
        // 1. 재생되는 애니메이션. 즉, 공격하는거나 죽는거나 등등 모두 끝나고 리셋해야 한다. 
        if (_character.IsCharacterCoroutineRunning == false)
        {
            Debug.Log("Game Reset!");
            ResetGame();
        }
    }

    public override void OnStateExit()
    {

    }

    public void ResetGame()
    {
        // 1. 스테이지를 다시 로드한다. 
        StageManager.Instance.Init(GameManager.Instance.World, GameManager.Instance.Stage);
    }
}