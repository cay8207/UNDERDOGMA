using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Chaser : Enemy
{
    [SerializeField] GameObject AttackRange;

    public override void Start()
    {
        base.Start();

        Vector2Int targetPosition = new Vector2Int(Row, Col) + directionOffsetDictionary[EnemyAttackDirection];
        GameObject _attackRange = Instantiate(AttackRange, new Vector3(targetPosition.x, targetPosition.y, 0), Quaternion.identity);
        SetAttackRangePosition(_attackRange, EnemyAttackDirection);
        _attackRange.transform.parent = transform;
        AttackRange.GetComponent<SpriteRenderer>().enabled = false;
    }

    // 적의 행동을 정의하는 함수. 추격자의 경우 만약 캐릭터와 일직선상에 있고, 그 사이에 아무런 장애물이 없다면 돌진한 후 데미지를 입힌다.
    public int CheckCharacterDamaged(int playerRow, int playerCol)
    {
        bool isObstacle = false;

        Vector2Int EnemyPosition = new Vector2Int(Row, Col);
        Vector2Int targetPosition = EnemyPosition + directionOffsetDictionary[EnemyAttackDirection];

        // 1. 만약 캐릭터가 추격자의 공격범위 칸에 있다면 공격한다.
        if (targetPosition == new Vector2Int(playerRow, playerCol))
        {
            StartCoroutine(EnemyAttackAnimation(targetPosition));
            return 1;
        }

        Debug.Log("(ChaserEnemy.cs) playerRow: " + playerRow + ", playerCol: " + playerCol);

        // 2. 만약 캐릭터가 추적자의 공격범위 칸에 있지 않고, 일직선상에 있지 않다면 공격하지 않고 함수를 중단한다.
        if ((EnemyAttackDirection == SpriteDirection.Up || EnemyAttackDirection == SpriteDirection.Down) && Row != playerRow)
        {
            return 0;
        }
        else if ((EnemyAttackDirection == SpriteDirection.Left || EnemyAttackDirection == SpriteDirection.Right) && Col != playerCol)
        {
            return 0;
        }

        // 3. 만약 캐릭터가 추적자의 공격범위 칸에 있지는 않지만, 일직선상에 있다면 돌진한다. 이를 위해 일직선상에 장애물이 있는지 체크한다.
        while (true)
        {
            targetPosition += directionOffsetDictionary[EnemyAttackDirection];
            Debug.Log("(ChaserEnemy.cs) next position: " + targetPosition);
            TileType tileType = StageManager.Instance.TempTileDictionary[targetPosition].Type;
            if (tileType == TileType.Wall || tileType == TileType.Enemy || tileType == TileType.Meat)
            {
                isObstacle = true;
                break;
            }
            else if (targetPosition == new Vector2Int(playerRow, playerCol))
            {
                // 추적자가 실제로 도달할 칸은 플레이어 바로 전 칸이다. 따라서 플레이어 칸에서 공격 방향을 하나 뺀 만큼의 칸으로 이동한다.
                targetPosition -= directionOffsetDictionary[EnemyAttackDirection];
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

            StartCoroutine(EnemyAttackAnimation(targetPosition));

            // 현재 DamagedState에서 foreach문을 도는중이기 때문에 ChaserEnemy의 정보를 업데이트 할 수 없음.
            // 따라서 EnemyManager에 저장해두고, 한번에 업데이트한다. 

            EnemyManager.Instance.ChaserMoveDictionary.Add(new Vector2Int(Row, Col), targetPosition);

            // 4.3. 일단 행과 열이 바뀌었음을 저장해준다. 
            Row = targetPosition.x;
            Col = targetPosition.y;

            return 1;
        }

        return 0;
    }

    public override IEnumerator EnemyAttackAnimation(Vector2Int targetPosition)
    {
        StartCoroutine(base.EnemyAttackAnimation(targetPosition));

        yield return null;
    }

    public void SetAttackRangePosition(GameObject attackRange, SpriteDirection attackDirection)
    {
        switch (attackDirection)
        {
            case SpriteDirection.Up:
                attackRange.transform.Rotate(0, 0, -90);
                attackRange.transform.position += new Vector3(0.0f, -0.38f, 0.0f);
                break;
            case SpriteDirection.Down:
                attackRange.transform.Rotate(0, 0, 90);
                attackRange.transform.position += new Vector3(0.0f, 0.38f, 0.0f);
                break;
            case SpriteDirection.Left:
                attackRange.transform.Rotate(0, 0, 0);
                attackRange.transform.position += new Vector3(0.38f, 0.0f, 0.0f);
                break;
            case SpriteDirection.Right:
                attackRange.transform.Rotate(0, 0, 180);
                attackRange.transform.position += new Vector3(-0.38f, 0.0f, 0.0f);
                break;
        }
    }
}
