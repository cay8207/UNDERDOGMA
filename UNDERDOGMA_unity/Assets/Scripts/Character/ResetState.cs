using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ResetState : BaseState
{
    public ResetState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {

    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }

    public void ResetGame()
    {
        // 1. 짐승의 입이 잡아먹는듯한 애니메이션을 실행시켜준다.
        _character.EnqueueCoroutine(_character.ResetAnimation());

        // 2. 리셋 효과음을 실행시켜준다.
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Reset);

        // 3. 모든 오브젝트들을 파괴시켜준다.
        StageManager.Instance.DestroyAllObjects();

        // 4. 타일들을 새롭게 다시 그려준다. 
        StageManager.Instance.TileInstantiate();
    }
}