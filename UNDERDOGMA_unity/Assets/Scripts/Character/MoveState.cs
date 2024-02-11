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
    public MoveState(Character character, KeyCode key) : base(character)
    {
        _key = key;
    }

    public override void OnStateEnter()
    {
        CharacterMove(_key);
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }

    private void CharacterMove(KeyCode key)
    {
        // 캐릭터가 이동하지 않는 경우에는 적들에게 데미지를 받는 등의 이벤트가 발생하면 안된다. 
        Vector2Int targetPosition = CheckCharacterMove(key);

        // while문을 탈출한 후, 즉 이동이 끝난 후 상하좌우에 적이 있다면 데미지를 입어야 한다.
        if ((targetPosition.x != _character.Row) || (targetPosition.y != _character.Col))
        {
            _character.MoveCount++;

            Execution.Instance.ExecutionCheck(_character.MoveCount);
            _character.EnqueueCoroutine(_character.CharacterMoveCoroutine(targetPosition));

            _character.HeartChange(-1);

            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Move);

            // 적의 턴을 진행하는 코드. 
            EnemyManager.Instance.EnemyTurn();

            if (_character.Heart <= 0)
            {
                _character.ChangeState(Character.State.Death);
            }
        }
    }

    public Vector2Int CheckCharacterMove(KeyCode key)
    {
        int row = _character.Row;
        int col = _character.Col;

        // 다음으로 이동할 칸의 위치를 설정해준다. 
        Vector2Int targetPosition = new Vector2Int(row, col) + returnDirection(key);

        while (true)
        {
            Debug.Log("next row: " + targetPosition.x + " " + "next col: " + targetPosition.y);

            // 다음으로 이동할 위치를 지정해준다. 
            TileObject tileObject = StageManager.Instance.TempTileDictionary[targetPosition];

            // 1. 이동하려는 칸에 벽이 있는 경우 멈춘다.
            if (tileObject.Type == TileType.Wall)
            {
                // targetPosition에 벽이 있으므로, 이전 위치로 돌아가야 한다. 
                targetPosition -= returnDirection(key);
                _character.ChangeState(Character.State.Idle);
                break;
            }

            // 2. 적이 있고, 살아있는 경우 Damaged State로 이동한다.
            if (tileObject.Type == TileType.Enemy)
            {
                if (tileObject.EnemyData.IsAlive == true)
                {
                    // targetPosition에 벽이 있으므로, 이전 위치로 돌아가야 한다. 
                    targetPosition -= returnDirection(key);
                    _character.ChangeState(Character.State.Damaged);
                    break;
                }
            }

            // 3. 만약 해당 칸에 고기가 있다면 Meat State로 이동한다. 
            if (tileObject.Type == TileType.Meat)
            {
                if (tileObject.MeatData.IsExist == true)
                {
                    // 3.1. 고기의 경우 해당 칸까지 가야한다. targetPosition을 그대로 반환하면 됌. 
                    _character.ChangeState(Character.State.Meat);
                    break;
                }
            }

            // 4. 이외의 경우 해당 칸으로 한 칸 더 전진. 
            if (tileObject.Type == TileType.Empty)
            {
                // 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
                targetPosition += returnDirection(key);

                // while문이 무한루프에 빠지는 것을 방지하기 위해 범위를 제한해준다.
                if (row > 100 || row < -100 || col > 100 || col < -100)
                {
                    Debug.Log("캐릭터 이동 관련해서 무한루프 발생! 맵 오브젝트(적, 벽, 고기 등)의 위치를 확인해주세요! (MoveState.cs)");
                    break;
                }
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