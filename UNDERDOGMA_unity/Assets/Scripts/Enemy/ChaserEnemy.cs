using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ChaserEnemy : Enemy
{
    [SerializeField] GameObject AttackRange;

    public AttackDirection _attackDirection;

    public void SetAttributes(EnemyData enemyData)
    {
        // NormalEnemy에 특화된 SetAttributes 로직을 구현
    }

    public override void Start()
    {
        base.Start();

        Vector2Int targetPosition = new Vector2Int(Row, Col) + directionOffsetDictionary[_attackDirection];
        GameObject _attackRange = Instantiate(AttackRange, new Vector3(targetPosition.x, targetPosition.y, 0), Quaternion.identity);
        SetAttackRangePosition(_attackRange, _attackDirection);
        _attackRange.transform.parent = transform;
        AttackRange.GetComponent<SpriteRenderer>().enabled = false;
    }

    // 적의 행동을 정의하는 함수. 추격자의 경우 만약 캐릭터와 일직선상에 있고, 그 사이에 아무런 장애물이 없다면 돌진한 후 데미지를 입힌다.
    public override IEnumerator EnemyAction(int playerRow, int playerCol)
    {
        bool isObstacle = false;

        yield return null;

        Vector2Int EnemyPosition = new Vector2Int(Row, Col);
        Vector2Int targetPosition = EnemyPosition + directionOffsetDictionary[_attackDirection];

        // 1. 만약 캐릭터가 추격자의 공격범위 칸에 있다면 공격한다.
        if (targetPosition == new Vector2Int(playerRow, playerCol))
        {
            StartCoroutine(base.EnemyAction(playerRow, playerCol));
            StageManager.Instance._character.GetComponent<Character>().HeartChange(-Attack);
        }

        // 2. 만약 캐릭터가 추적자의 공격범위 칸에 있지 않고, 일직선상에 있지 않다면 공격하지 않고 함수를 중단한다.
        if ((_attackDirection == AttackDirection.Up || _attackDirection == AttackDirection.Down) && Col != playerCol)
        {
            yield break;
        }
        else if ((_attackDirection == AttackDirection.Left || _attackDirection == AttackDirection.Right) && Row != playerRow)
        {
            yield break;
        }

        // 3. 만약 캐릭터가 추적자의 공격범위 칸에 있지는 않지만, 일직선상에 있다면 돌진한다. 이를 위해 일직선상에 장애물이 있는지 체크한다.
        while (true)
        {
            targetPosition += directionOffsetDictionary[_attackDirection];
            TileType tileType = StageManager.Instance.TempTileDictionary[targetPosition].Type;
            if (tileType == TileType.Wall || tileType == TileType.Enemy || tileType == TileType.Meat)
            {
                isObstacle = true;
                break;
            }
            else if (targetPosition == new Vector2Int(playerRow, playerCol))
            {
                // 추적자가 실제로 도달할 칸은 플레이어 바로 전 칸이다. 따라서 플레이어 칸에서 공격 방향을 하나 뺀 만큼의 칸으로 이동한다.
                targetPosition -= directionOffsetDictionary[_attackDirection];
                break;
            }

            // 무한루프를 방지하는 코드. 
            if (targetPosition.x > 100 || targetPosition.x < -100 || targetPosition.y > 100 || targetPosition.y < -100)
            {
                Debug.Log("ChaserEnemy에서 무한루프 발생! 문제를 확인해주세요!");
                break;
            }
        }

        // 4. 캐릭터와 추적자가 일직선상에 있다는 것(즉, 장애물이 없다는 것)을 확인했다면 돌진한다.
        if (!isObstacle)
        {
            gameObject.transform.DOMove(new Vector3(targetPosition.x, targetPosition.y, 0) + new Vector3(-0.06f, 0.3f, 0), 1.0f, false);

            StartCoroutine(base.EnemyAction(playerRow, playerCol));
            StageManager.Instance._character.GetComponent<Character>().HeartChange(-Attack);

            // 4.1. 원래 위치와 TileDictionary 정보를 바꾸어준다.
            // 원래 위치는 타일만 있는것으로, 새로운 위치는 적의 정보를 넣어준다.
            TileObject enemyInfo = StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)];
            StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)] = new TileObject(TileType.Empty);
            StageManager.Instance.TempTileDictionary[targetPosition] = enemyInfo;

            // 4.2. EnemyDictionary 정보를 수정해준다. 
            // 적의 과거 위치인 Row, Col을 기반으로 해당 Dictionary의 Value를 가져오고, 
            // 이를 새로운 위치인 playerRow-1, playerCol에 넣어준 후 기존의 위치에 있는 Dictionary 값은 삭제한다.
            StageManager.Instance.EnemyDictionary.Add(targetPosition, StageManager.Instance.EnemyDictionary[new Vector2Int(Row, Col)]);
            StageManager.Instance.EnemyDictionary.Remove(new Vector2Int(Row, Col));

            // 4.3. 일단 행과 열이 바뀌었음을 저장해준다. 
            Row = targetPosition.x;
            Col = targetPosition.y;
        }
    }

    public void SetAttackRangePosition(GameObject attackRange, AttackDirection attackDirection)
    {
        switch (attackDirection)
        {
            case AttackDirection.Up:
                attackRange.transform.Rotate(0, 0, -90);
                attackRange.transform.position += new Vector3(0.0f, -0.38f, 0.0f);
                break;
            case AttackDirection.Down:
                attackRange.transform.Rotate(0, 0, 90);
                attackRange.transform.position += new Vector3(0.0f, 0.38f, 0.0f);
                break;
            case AttackDirection.Left:
                attackRange.transform.Rotate(0, 0, 0);
                attackRange.transform.position += new Vector3(0.38f, 0.0f, 0.0f);
                break;
            case AttackDirection.Right:
                attackRange.transform.Rotate(0, 0, 180);
                attackRange.transform.position += new Vector3(-0.38f, 0.0f, 0.0f);
                break;
        }
    }
}
