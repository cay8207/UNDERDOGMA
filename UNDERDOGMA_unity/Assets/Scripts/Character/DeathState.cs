using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class DeathState : BaseState
{
    public DeathState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {
        // 캐릭터가 죽었을 경우 죽는 애니메이션을 보여주고 게임을 재시작한다. 
        public IEnumerator CharacterDeath()
        {
            _heart = 0;
            _heartText.GetComponent<Text>().SetText(_heart);

            gameObject.GetComponent<SpriteRenderer>().DOFade(0, 1.0f);

            yield return new WaitForSeconds(1.0f);

            // 죽는 모션 추가되면 수정할 예정. 
            // gameObject.GetComponent<Animator>().SetBool("IsDied", true);

            // yield return new WaitForSeconds(1.0f);

            // gameObject.GetComponent<Animator>().SetBool("IsDied", false);

            StageManager.Instance.ResetGame();
        }
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }
}
