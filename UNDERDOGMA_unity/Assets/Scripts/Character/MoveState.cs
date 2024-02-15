using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveState : BaseState
{
    public KeyCode _key;
    public Character.State _nextState;
    public Vector2Int targetPosition;

    public MoveState(Character character, KeyCode key) : base(character)
    {
        _key = key;
    }

    public override void OnStateEnter()
    {
        targetPosition = CharacterMove(_key);
    }

    public override void OnStateUpdate()
    {
        if (_character.IsCharacterCoroutineRunning == false)
        {
            // 1. 고기가 있는 칸에 도달하는 경우 MeatState로 넘어간다.
            // 2. 그렇지 않은 경우 적에게 공격을 받는지 체크하는 Damaged State로 넘어간다.

            if (_nextState == Character.State.Meat)
            {
                _character.ChangeState(Character.State.Meat, targetPosition);
            }
            else if (_nextState == Character.State.Damaged)
            {
                _character.ChangeState(Character.State.Damaged);
            }
        }
    }

    public override void OnStateExit()
    {

    }

    private Vector2Int CharacterMove(KeyCode key)
    {
        // 1. 캐릭터가 이동하지 않는 경우에는 적들에게 데미지를 받는 등의 이벤트가 발생하면 안된다. 
        Vector2Int targetPosition = CheckCharacterMove(key);

        // 2. while문을 탈출한 후, 이동해야 한다면 캐릭터를 이동시킨다. 
        if ((targetPosition.x != _character.Row) || (targetPosition.y != _character.Col))
        {
            _character.MoveCount++;

            _character.EnqueueCoroutine(_character.CharacterMoveCoroutine(targetPosition));

            _character.HeartChange(-1);

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Move);
        }

        _character.UpdatePosition(targetPosition.x, targetPosition.y);

        return targetPosition;
    }

    public Vector2Int CheckCharacterMove(KeyCode key)
    {
        int row = _character.Row;
        int col = _character.Col;

        // 다음으로 이동할 칸의 위치를 설정해준다. 
        Vector2Int targetPosition = new Vector2Int(row, col) + returnDirection(key);

        while (true)
        {
            Debug.Log("(MoveState) next row: " + targetPosition.x + " " + "next col: " + targetPosition.y);
            Debug.Log("(MoveState) next type: " + StageManager.Instance.TempTileDictionary[targetPosition].Type);
            if (StageManager.Instance.TempTileDictionary[targetPosition].Type == TileType.Enemy)
            {
                Debug.Log("(MoveState) next enemy: " + StageManager.Instance.TempTileDictionary[targetPosition].EnemyData.IsAlive);
            }

            // 다음으로 이동할 위치를 지정해준다. 
            TileObject tileObject = StageManager.Instance.TempTileDictionary[targetPosition];

            // 1. 이동하려는 칸에 벽이 있는 경우 멈춘다.
            if (tileObject.Type == TileType.Wall || (tileObject.Type == TileType.Enemy && tileObject.EnemyData.IsAlive == true))
            {
                // 1.1. targetPosition에 벽이 있으므로, 이전 위치로 돌아가야 한다. 
                targetPosition -= returnDirection(key);
                _nextState = Character.State.Damaged;
                break;
            }

            // 3. 만약 해당 칸에 고기가 있다면 Meat State로 이동한다. 
            if (tileObject.Type == TileType.Meat)
            {
                if (tileObject.MeatData.IsExist == true)
                {
                    // 3.1. 고기의 경우 해당 칸까지 가야한다. targetPosition을 그대로 반환하면 됌. 
                    _nextState = Character.State.Meat;
                    break;
                }
            }

            // 4. 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
            targetPosition += returnDirection(key);

            // 5. while문이 무한루프에 빠지는 것을 방지하기 위해 범위를 제한해준다.
            if (targetPosition.x > 100 || targetPosition.x < -100 || targetPosition.y > 100 || targetPosition.y < -100)
            {
                Debug.Log("캐릭터 이동 관련해서 무한루프 발생! 맵 오브젝트(적, 벽, 고기 등)의 위치를 확인해주세요! (MoveState.cs)");
                break;
            }
        }

        return targetPosition;
    }

    private Vector2Int returnDirection(KeyCode key)
    {
        if (key == KeyCode.W)
        {
            return new Vector2Int(0, 1);
        }
        else if (key == KeyCode.S)
        {
            return new Vector2Int(0, -1);
        }
        else if (key == KeyCode.A)
        {
            return new Vector2Int(-1, 0);
        }
        else if (key == KeyCode.D)
        {
            return new Vector2Int(1, 0);
        }
        else
        {
            return new Vector2Int(0, 0);
        }
    }
}