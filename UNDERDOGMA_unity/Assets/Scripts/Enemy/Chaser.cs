using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class Chaser : Enemy
{
    public override void Start()
    {
        base.Start();
    }

    // 적의 행동을 정의하는 함수. 추격자의 경우 만약 캐릭터와 일직선상에 있고, 그 사이에 아무런 장애물이 없다면 돌진한 후 데미지를 입힌다.
    public override IEnumerator EnemyAction(int playerRow, int playerCol)
    {
        bool isObstacle = false;

        if (Row == playerRow && Col < playerCol)
        {
            for (int i = Col + 1; i < playerCol; i++)
            {
                // 추적자와 캐릭터 사이에 아무런 적, 벽이 없다면 돌진한다. 
                if (StageManager.Instance._stageData.TileDictionary[new Vector2Int(Row, i)][0] != -1
                    && StageManager.Instance._stageData.TileDictionary[new Vector2Int(Row, i)][0] != 1)
                {
                    isObstacle = true;

                }
            }
            // 적과 플레이어 사이에 장애물이 없다면 플레이어의 위치 바로 앞까지 돌진. 
            if (!isObstacle)
            {
                gameObject.transform.DOMove(new Vector3(Row, playerCol - 1, 0), 1.0f, false);
                yield break;
            }
        }
        else if (Row == playerRow && Col > playerCol)
        {
            for (int i = Col - 1; i > playerCol; i--)
            {

            }
        }
        else if (Col == playerCol)
        {
            for (int i = Col + 1; i < playerCol; i++)
            {
                //추적자와 캐릭터 사이에 아무런 적, 벽이 없다면 돌진한다.
                if (StageManager.Instance._stageData.TileDictionary[new Vector2Int(Row, i)][0] != -1
                    && StageManager.Instance._stageData.TileDictionary[new Vector2Int(Row, i)][0] != -1)
                {
                    yield break;
                }
            }
        }



        yield return null;
    }
}
