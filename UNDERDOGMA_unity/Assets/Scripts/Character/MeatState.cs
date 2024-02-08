using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MeatState : BaseState
{
    public Vector2Int _targetPosition;

    private bool _isCharacterEatMeatCoroutineRunning = false;

    public MeatState(Character character, Vector2Int targetPosition) : base(character)
    {
        _targetPosition = targetPosition;
    }

    public override void OnStateEnter()
    {
        StartCoroutine(CharacterEatMeat(_targetPosition));
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }

    public IEnumerator CharacterEatMeat(Vector2Int targetPosition)
    {
        // 1. 코루틴이 실행되는 중인지를 체크하는 변수를 설정한다. 
        _isCharacterEatMeatCoroutineRunning = true;

        // 2. 사운드를 재생한다. 
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Eat);

        // 3. 애니메이션을 재생한다.  
        _character.GetComponent<Animator>().SetBool("IsAttack", true);

        yield return new WaitForSeconds(0.9f);

        _character.GetComponent<Animator>().SetBool("IsAttack", false);

        // 4. 플레이어의 체력을 변화시키고, 고기를 사라지는 애니메이션을 실행한다. 

        Meat meat = StageManager.Instance.GameObjectDictionary[_targetPosition].GetComponent<Meat>();

        _character.HeartChange(meat.Amount);
        meat.EatMeat(_targetPosition);

        // 5. 코루틴이 끝났다는 의미로 변수의 값을 변경해준다. 
        _isCharacterEatMeatCoroutineRunning = false;
    }
}
