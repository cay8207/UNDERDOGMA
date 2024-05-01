using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Angel : Enemy
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
        Vector2Int targetPosition = new Vector2Int(Row, Col) + directionOffsetDictionary[EnemyAttackDirection];

        if (targetPosition == new Vector2Int(playerRow, playerCol))
        {
            StartCoroutine(EnemyAttackAnimation(targetPosition));
            return 1;
        }

        return 0;
    }

    public override IEnumerator EnemyAttackAnimation(Vector2Int targetPosition)
    {
        StartCoroutine(base.EnemyAttackAnimation(targetPosition));

        yield return null;
    }

    public override IEnumerator EnemyDeath(Vector2Int targetPosition, bool deathByExecution)
    {
        if (deathByExecution == true)
        {
            yield return new WaitForSeconds(2.5f);
        }

        // DamagedState에서 foreach문을 도는 상황이니 바로 업데이트는 불가. EnemyManager를 통해 나중에 업데이트한다.
        EnemyManager.Instance.DeathAngelPositionList.Add(targetPosition);

        gameObject.GetComponent<Animator>().SetBool("IsDied", true);

        gameObject.GetComponent<SpriteRenderer>().DOFade(0, 0.5f);

        yield return new WaitForSeconds(0.5f);

        gameObject.GetComponent<Animator>().SetBool("IsDied", false);

        Debug.Log("(Enemy.cs) 적이 죽었습니다. 데이터 변경하기!");

        Destroy(StageManager.Instance.GameObjectDictionary[targetPosition]);
        StageManager.Instance.GameObjectDictionary.Remove(targetPosition);

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
