using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class KickBoss : Enemy
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

    // 적의 행동을 정의하는 함수. 일반 적의 경우 만약 플레이어가 공격 범위에 있다면 공격한다.
    public int CheckCharacterDamaged(int playerRow, int playerCol)
    {
        Vector2Int targetPosition = new Vector2Int(Row, Col) + directionOffsetDictionary[EnemyAttackDirection];

        if (targetPosition == new Vector2Int(playerRow, playerCol))
        {
            StartCoroutine(EnemyAttackAnimation(targetPosition));

            Vector2Int KickedPosition = new Vector2Int(playerRow, playerCol) + directionOffsetDictionary[EnemyAttackDirection];

            while (true)
            {
                TileObject tileObject = StageManager.Instance.TempTileDictionary[KickedPosition];

                // 1. 이동하려는 칸에 벽이 있는 경우 멈춘다.
                if (tileObject.Type == TileType.Wall || (tileObject.Type == TileType.Enemy && tileObject.EnemyData.IsAlive == true)
                    || (tileObject.Type == TileType.Meat && tileObject.MeatData.IsExist == true && tileObject.Type == TileType.Ball))
                {
                    // 1.1. targetPosition에 벽이 있으므로, 이전 위치로 돌아가야 한다. 
                    KickedPosition -= directionOffsetDictionary[EnemyAttackDirection];
                    break;
                }

                // 2. 다음으로 이동할 칸을 계속 업데이트해줘야 한다. 
                KickedPosition += directionOffsetDictionary[EnemyAttackDirection];

                // 3. while문이 무한루프에 빠지는 것을 방지하기 위해 범위를 제한해준다.
                if (KickedPosition.x > 100 || KickedPosition.x < -100 || KickedPosition.y > 100 || KickedPosition.y < -100)
                {
                    Debug.Log("캐릭터 이동 관련해서 무한루프 발생! 맵 오브젝트(적, 벽, 고기 등)의 위치를 확인해주세요! (MoveState.cs)");
                    break;
                }
            }

            StageManager.Instance._character.GetComponent<Character>()
                        .EnqueueCoroutine(StageManager.Instance._character.GetComponent<Character>()
                            .CharacterMoveCoroutine(
                                new Vector2Int(StageManager.Instance._character.GetComponent<Character>().Row,
                                StageManager.Instance._character.GetComponent<Character>().Col),
                                    KickedPosition));

            StageManager.Instance._character.GetComponent<Character>().UpdatePosition(KickedPosition.x, KickedPosition.y);

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