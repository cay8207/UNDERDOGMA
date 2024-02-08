using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class MoveState : BaseState
{
    public MoveState(Character character) : base(character)
    {

    }

    public override void OnStateEnter()
    {
        IEnumerator CharacterMove(int direction)
        {
            // 다음으로 이동할 칸의 위치를 설정해준다. 
            Vector2Int targetPosition = new Vector2Int(_row, _col) + directionOffsets[direction];

            // 캐릭터가 이동하지 않는 경우에는 적들에게 데미지를 받는 등의 이벤트가 발생하면 안된다. 
            bool isMove = false;

            while (true)
            {
                Debug.Log("next row: " + targetPosition.x + " " + "next col: " + targetPosition.y);

                // 다음으로 이동할 위치를 지정해준다. 
                TileObject tileObject = StageManager.Instance.TempTileDictionary[targetPosition];

                // 1. 이동하려는 칸에 벽이 있는 경우
                if (tileObject.Type == TileType.Wall)
                {
                    break;
                }

                // 2. 이동하려는 칸에 적이 있는 경우.
                if (tileObject.Type == TileType.Enemy)
                {
                    // 캐릭터의 체력이 적보다 높다면 AttackState로 전환한다.
                    if (tileObject.EnemyData.IsAlive == true && _heart > tileObject.EnemyData.Heart)
                    {
                        _character.ChangeState(Character.State.Attack);
                        break;
                    }
                }

                // 3. 만약 해당 칸에 하트가 있다면 하트 오브젝트를 파괴하고 그만큼 체력을 회복한다.
                if (tileObject.Type == TileType.Meat)
                {
                    if (tileObject.MeatData.IsExist == true)
                    {
                        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Meat);

                        _character.HeartChange(tileObject.MeatData.Amount);
                        MeatDictionary[targetPosition].GetComponent<Meat>().EatMeat(targetPosition);
                    }
                }

                // 적이 있고, 살아있는 경우 멈춘다. 
                if (tileObject.Type == TileType.Enemy)
                {
                    if (tileObject.EnemyData.IsAlive == true)
                    {
                        break;
                    }
                    else
                    {
                        isMove = true;
                        // transform.position += new Vector3(directionOffsets[direction].x, directionOffsets[direction].y, 0);
                        _row = targetPosition.x;
                        _col = targetPosition.y;

                        // 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
                        targetPosition += directionOffsets[direction];
                    }
                }

                // 이외의 경우 해당 칸으로 한 칸 더 전진. 
                else
                {
                    isMove = true;
                    // transform.position += new Vector3(directionOffsets[direction].x, directionOffsets[direction].y, 0);
                    _row = targetPosition.x;
                    _col = targetPosition.y;

                    // 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
                    targetPosition += directionOffsets[direction];

                    // while문이 무한루프에 빠지는 것을 방지하기 위해 범위를 제한해준다.
                    if (_row > 100 || _row < -100 || _col > 100 || _col < -100)
                    {
                        Debug.Log("캐릭터 이동 관련해서 무한루프 발생! 맵 오브젝트(적, 벽, 고기 등)의 위치를 확인해주세요! (MoveState.cs)");
                        break;
                    }
                }
            }

            transform.DOMove(new Vector3(_row - 0.07f, _col + 0.35f, 0), 1.0f, false).SetEase(Ease.OutCirc);

            // while문을 탈출한 후, 즉 이동이 끝난 후 상하좌우에 적이 있다면 데미지를 입어야 한다.
            if (isMove)
            {
                _character.HeartChange(-1);

                AudioManager.Instance.PlaySfx(AudioManager.Sfx.Move);

                _moveCount++;

                // 적의 턴을 진행하는 코드. 
                EnemyManager.Instance.EnemyTurn();

                yield return new WaitForSeconds(0.1f);

                if (_heart <= 0)
                {
                    StartCoroutine(CharacterDeath());
                }
            }
        }
    }

    public override void OnStateUpdate()
    {

    }

    public override void OnStateExit()
    {

    }
}