using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// 일반 적은 하나의 방향으로만 공격한다.
public class NormalEnemy : Enemy
{
    [SerializeField] GameObject AttackRange;

    public override void Start()
    {
        base.Start();

        // 일반 적의 경우 공격 방향이 정해져있고, 해당 방향에 UI를 넣어줘야 함. 
        Vector2Int targetPosition = new Vector2Int(Row, Col) + directionOffsetDictionary[EnemyAttackDirection];
        GameObject _attackRange = Instantiate(AttackRange, new Vector3(targetPosition.x, targetPosition.y, 0), Quaternion.identity);
        SetAttackRangePosition(_attackRange, EnemyAttackDirection);
        _attackRange.transform.parent = transform;
        AttackRange.GetComponent<SpriteRenderer>().enabled = false;
    }

    // 적을 공격할 수 있는지 체크하는 함수. 
    public int CheckCharacterDamaged(int playerRow, int playerCol)
    {
        Vector2Int targetPosition = new Vector2Int(Row, Col) + directionOffsetDictionary[EnemyAttackDirection];

        if (targetPosition == new Vector2Int(playerRow, playerCol))
        {
            StartCoroutine(EnemyAttackAnimation(targetPosition));
            return 1;
        }

        return 0;
    }

    // 적을 공격하는 함수. 
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
