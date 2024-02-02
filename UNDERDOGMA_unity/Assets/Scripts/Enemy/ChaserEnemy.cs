using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class ChaserEnemy : Enemy
{
    [SerializeField] GameObject AttackRange;

    public int _attackDirection;

    public override void Start()
    {
        base.Start();

        Vector2Int targetPosition = new Vector2Int(Row, Col) + directionOffsets[_attackDirection];
        GameObject _attackRange = Instantiate(AttackRange, new Vector3(targetPosition.x, targetPosition.y, 0), Quaternion.identity);
        switch (_attackDirection)
        {
            case 0:
                _attackRange.transform.Rotate(0, 0, -90);
                _attackRange.transform.position += new Vector3(0.0f, -0.38f, 0.0f);
                break;
            case 1:
                _attackRange.transform.Rotate(0, 0, 90);
                _attackRange.transform.position += new Vector3(0.0f, 0.38f, 0.0f);
                break;
            case 2:
                _attackRange.transform.Rotate(0, 0, 0);
                _attackRange.transform.position += new Vector3(0.38f, 0.0f, 0.0f);
                break;
            case 3:
                _attackRange.transform.Rotate(0, 0, 180);
                _attackRange.transform.position += new Vector3(-0.38f, 0.0f, 0.0f);
                break;
        }
        _attackRange.transform.parent = transform;
        AttackRange.GetComponent<SpriteRenderer>().enabled = false;
    }

    // 적의 행동을 정의하는 함수. 추격자의 경우 만약 캐릭터와 일직선상에 있고, 그 사이에 아무런 장애물이 없다면 돌진한 후 데미지를 입힌다.
    public override IEnumerator EnemyAction(int playerRow, int playerCol)
    {
        bool isObstacle = false;

        yield return null;

        // 0, 1, 2, 3은 각각 상, 하, 좌, 우를 의미한다. 
        switch (_attackDirection)
        {
            case 0:
                // 몬스터의 행이 캐릭터와 같고, 열이 캐릭터보다 작다면 사이에 무엇이 있는지 검사. 
                if (Row == playerRow && Col < playerCol - 1)
                {
                    for (int i = Col + 1; i < playerCol; i++)
                    {
                        // 추적자와 캐릭터 사이에 아무런 적, 벽이 없다면 돌진한다. 
                        if (StageManager.Instance.TempTileDictionary[new Vector2Int(Row, i)][0] == -1
                            || StageManager.Instance.TempTileDictionary[new Vector2Int(Row, i)][0] == 1
                            || StageManager.Instance.TempTileDictionary[new Vector2Int(Row, i)][0] == 2)
                        {
                            isObstacle = true;
                        }
                    }
                    // 적과 플레이어 사이에 장애물이 없다면 플레이어의 위치 바로 앞까지 돌진. 
                    if (!isObstacle)
                    {
                        gameObject.transform.DOMove(new Vector3(Row - 0.06f, playerCol - 1.0f + 0.3f, 0), 1.0f, false);

                        StartCoroutine(base.EnemyAction(playerRow, playerCol));
                        StageManager.Instance._character.GetComponent<Character>().Heart -= Attack;
                        StageManager.Instance._character.GetComponent<Character>()._heartText.GetComponent<Text>().SetText(
                            StageManager.Instance._character.GetComponent<Character>().Heart
                        );

                        // 1. 원래 위치와 TileDictionary 정보를 바꾸어준다.
                        // 원래 위치는 타일만 있는것으로, 새로운 위치는 적의 정보를 넣어준다.
                        List<int> enemyInfo = StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)];
                        StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)] = new List<int> { 0 };
                        StageManager.Instance.TempTileDictionary[new Vector2Int(playerRow, playerCol - 1)] = enemyInfo;

                        // 2. EnemyDictionary 정보를 수정해준다. 
                        // 적의 과거 위치인 Row, Col을 기반으로 해당 Dictionary의 Value를 가져오고, 
                        // 이를 새로운 위치인 playerRow-1, playerCol에 넣어준 후 기존의 위치에 있는 Dictionary 값은 삭제한다.
                        StageManager.Instance.EnemyDictionary.Add(new Vector2Int(playerRow, playerCol - 1), StageManager.Instance.EnemyDictionary[new Vector2Int(Row, Col)]);
                        StageManager.Instance.EnemyDictionary.Remove(new Vector2Int(Row, Col));

                        // 3. 일단 행과 열이 바뀌었음을 저장해준다. 
                        Row = playerRow;
                        Col = playerCol - 1;
                    }
                }
                else if (Row == playerRow && Col == playerCol - 1)
                {
                    StartCoroutine(base.EnemyAction(playerRow, playerCol));
                    StageManager.Instance._character.GetComponent<Character>().Heart -= Attack;
                    StageManager.Instance._character.GetComponent<Character>()._heartText.GetComponent<Text>().SetText(
                        StageManager.Instance._character.GetComponent<Character>().Heart
                    );
                }
                break;
            case 1:
                // 몬스터의 행이 캐릭터와 같고, 열이 캐릭터보다 위인 경우. 
                if (Row == playerRow && Col > playerCol + 1)
                {
                    for (int i = Col - 1; i > playerCol; i--)
                    {
                        if (StageManager.Instance.TempTileDictionary[new Vector2Int(Row, i)][0] == -1
                            || StageManager.Instance.TempTileDictionary[new Vector2Int(Row, i)][0] == 1
                            || StageManager.Instance.TempTileDictionary[new Vector2Int(Row, i)][0] == 2)
                        {
                            isObstacle = true;
                        }
                    }
                    if (!isObstacle)
                    {
                        gameObject.transform.DOMove(new Vector3(Row - 0.06f, playerCol + 1.0f + 0.3f, 0), 1.0f, false);

                        StartCoroutine(base.EnemyAction(playerRow, playerCol));
                        StageManager.Instance._character.GetComponent<Character>().Heart -= Attack;
                        StageManager.Instance._character.GetComponent<Character>()._heartText.GetComponent<Text>().SetText(
                            StageManager.Instance._character.GetComponent<Character>().Heart
                        );

                        // 1. 원래 위치와 TileDictionary 정보를 바꾸어준다.
                        // 원래 위치는 타일만 있는것으로, 새로운 위치는 적의 정보를 넣어준다.
                        List<int> enemyInfo = StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)];
                        StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)] = new List<int> { 0 };
                        StageManager.Instance.TempTileDictionary[new Vector2Int(playerRow, playerCol + 1)] = enemyInfo;

                        // 2. EnemyDictionary 정보를 수정해준다. 
                        // 적의 과거 위치인 Row, Col을 기반으로 해당 Dictionary의 Value를 가져오고, 
                        // 이를 새로운 위치인 playerRow-1, playerCol에 넣어준 후 기존의 위치에 있는 Dictionary 값은 삭제한다.
                        StageManager.Instance.EnemyDictionary.Add(new Vector2Int(playerRow, playerCol + 1), StageManager.Instance.EnemyDictionary[new Vector2Int(Row, Col)]);
                        StageManager.Instance.EnemyDictionary.Remove(new Vector2Int(Row, Col));

                        // 3. 일단 행과 열이 바뀌었음을 저장해준다. 
                        Row = playerRow;
                        Col = playerCol + 1;
                    }
                }
                else if (Row == playerRow && Col == playerCol + 1)
                {
                    StartCoroutine(base.EnemyAction(playerRow, playerCol));
                    StageManager.Instance._character.GetComponent<Character>().Heart -= Attack;
                    StageManager.Instance._character.GetComponent<Character>()._heartText.GetComponent<Text>().SetText(
                        StageManager.Instance._character.GetComponent<Character>().Heart
                    );
                }
                break;
            case 2:
                // 몬스터의 열이 캐릭터와 같고, 행이 플레이어보다 크다면. 즉, 오른쪽에 있는 경우 체크. 
                if (Col == playerCol && Row > playerRow + 1)
                {
                    for (int i = playerRow + 1; i < Row; i++)
                    {
                        if (StageManager.Instance.TempTileDictionary[new Vector2Int(i, Col)][0] == -1
                            || StageManager.Instance.TempTileDictionary[new Vector2Int(i, Col)][0] == 1
                            || StageManager.Instance.TempTileDictionary[new Vector2Int(i, Col)][0] == 2)
                        {
                            isObstacle = true;
                        }
                    }
                    if (!isObstacle)
                    {
                        gameObject.transform.DOMove(new Vector3(playerRow + 1.0f - 0.06f, Col + 0.3f, 0), 1.0f, false);

                        StartCoroutine(base.EnemyAction(playerRow, playerCol));
                        StageManager.Instance._character.GetComponent<Character>().Heart -= Attack;
                        StageManager.Instance._character.GetComponent<Character>()._heartText.GetComponent<Text>().SetText(
                            StageManager.Instance._character.GetComponent<Character>().Heart
                        );

                        // 1. 원래 위치와 TileDictionary 정보를 바꾸어준다.
                        // 원래 위치는 타일만 있는것으로, 새로운 위치는 적의 정보를 넣어준다.
                        List<int> enemyInfo = StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)];
                        StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)] = new List<int> { 0 };
                        StageManager.Instance.TempTileDictionary[new Vector2Int(playerRow + 1, playerCol)] = enemyInfo;

                        // 2. EnemyDictionary 정보를 수정해준다. 
                        // 적의 과거 위치인 Row, Col을 기반으로 해당 Dictionary의 Value를 가져오고, 
                        // 이를 새로운 위치인 playerRow-1, playerCol에 넣어준 후 기존의 위치에 있는 Dictionary 값은 삭제한다.
                        StageManager.Instance.EnemyDictionary.Add(new Vector2Int(playerRow + 1, playerCol), StageManager.Instance.EnemyDictionary[new Vector2Int(Row, Col)]);
                        StageManager.Instance.EnemyDictionary.Remove(new Vector2Int(Row, Col));

                        // 3. 일단 행과 열이 바뀌었음을 저장해준다. 
                        Row = playerRow + 1;
                        Col = playerCol;
                    }
                }
                else if (Col == playerCol && Row == playerRow + 1)
                {
                    StartCoroutine(base.EnemyAction(playerRow, playerCol));
                    StageManager.Instance._character.GetComponent<Character>().Heart -= Attack;
                    StageManager.Instance._character.GetComponent<Character>()._heartText.GetComponent<Text>().SetText(
                        StageManager.Instance._character.GetComponent<Character>().Heart
                    );
                }
                break;
            case 3:
                // 몬스터의 열이 플레이어와 같고, 행이 플레이어보다 작다면. 즉, 왼쪽에 있는 경우 체크.
                if (Col == playerCol && Row < playerRow - 1)
                {
                    for (int i = Row + 1; i < playerRow; i++)
                    {
                        Debug.Log("Row: " + i + " Col: " + Col +
                            " What's here?: " + StageManager.Instance.TempTileDictionary[new Vector2Int(i, Col)][0]);
                        if (StageManager.Instance.TempTileDictionary[new Vector2Int(i, Col)][0] == -1
                            || StageManager.Instance.TempTileDictionary[new Vector2Int(i, Col)][0] == 1
                            || StageManager.Instance.TempTileDictionary[new Vector2Int(i, Col)][0] == 2)
                        {
                            isObstacle = true;
                        }
                    }
                    if (!isObstacle)
                    {
                        gameObject.transform.DOMove(new Vector3(playerRow - 1.0f - 0.06f, Col + 0.3f, 0), 1.0f, false);

                        StartCoroutine(base.EnemyAction(playerRow, playerCol));
                        StageManager.Instance._character.GetComponent<Character>().Heart -= Attack;
                        StageManager.Instance._character.GetComponent<Character>()._heartText.GetComponent<Text>().SetText(
                            StageManager.Instance._character.GetComponent<Character>().Heart
                        );

                        // 1. 원래 위치와 TileDictionary 정보를 바꾸어준다.
                        // 원래 위치는 타일만 있는것으로, 새로운 위치는 적의 정보를 넣어준다.
                        List<int> enemyInfo = StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)];
                        StageManager.Instance.TempTileDictionary[new Vector2Int(Row, Col)] = new List<int> { 0 };
                        StageManager.Instance.TempTileDictionary[new Vector2Int(playerRow - 1, playerCol)] = enemyInfo;

                        // 2. EnemyDictionary 정보를 수정해준다. 
                        // 적의 과거 위치인 Row, Col을 기반으로 해당 Dictionary의 Value를 가져오고, 
                        // 이를 새로운 위치인 playerRow-1, playerCol에 넣어준 후 기존의 위치에 있는 Dictionary 값은 삭제한다.
                        StageManager.Instance.EnemyDictionary.Add(new Vector2Int(playerRow - 1, playerCol), StageManager.Instance.EnemyDictionary[new Vector2Int(Row, Col)]);
                        StageManager.Instance.EnemyDictionary.Remove(new Vector2Int(Row, Col));

                        // 3. 일단 행과 열이 바뀌었음을 저장해준다. 
                        Row = playerRow - 1;
                        Col = playerCol;
                    }
                }
                else if (Col == playerCol && Row == playerRow - 1)
                {
                    StartCoroutine(base.EnemyAction(playerRow, playerCol));
                    StageManager.Instance._character.GetComponent<Character>().Heart -= Attack;
                    StageManager.Instance._character.GetComponent<Character>()._heartText.GetComponent<Text>().SetText(
                        StageManager.Instance._character.GetComponent<Character>().Heart
                    );
                }
                break;
        }

        yield return null;
    }
}
