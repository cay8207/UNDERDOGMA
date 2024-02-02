using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class EnemyManager : Singleton<EnemyManager>
{
    public Dictionary<Vector2Int, GameObject> EnemyDictionary => StageManager.Instance.EnemyDictionary;

    public Queue<Coroutine> EnemyActionCoroutineQueue = new Queue<Coroutine>();
    public Queue<Coroutine> EnemyDeathCoroutineQueue = new Queue<Coroutine>();

    public void EnemyTurn()
    {
        Vector2Int PlayerPosition = new Vector2Int(StageManager.Instance._character.GetComponent<Character>().Row,
                                                    StageManager.Instance._character.GetComponent<Character>().Col);
        // 적의 턴이 시작되면 적들의 action을 모두 실행. 
        foreach (var enemy in EnemyDictionary)
        {
            int enemyRow = enemy.Key.x;
            int enemyCol = enemy.Key.y;
            Coroutine EnemyActionCoroutine = null;
            // 0번 타입 적인경우
            if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)][1] == 0)
            {
                EnemyActionCoroutine = StartCoroutine(enemy.Value.GetComponent<NormalEnemy>().EnemyAction(PlayerPosition.x, PlayerPosition.y));
            }
            // 1번 타입 적인 경우
            else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)][1] == 1)
            {
                EnemyActionCoroutine = StartCoroutine(enemy.Value.GetComponent<ChaserEnemy>().EnemyAction(PlayerPosition.x, PlayerPosition.y));
            }
            else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)][1] == 2)
            {
                EnemyActionCoroutine = StartCoroutine(enemy.Value.GetComponent<MiniBoss>().EnemyAction(PlayerPosition.x, PlayerPosition.y));
            }

            EnemyActionCoroutineQueue.Enqueue(EnemyActionCoroutine);
        }
    }

    public void EnemyDeath(Vector2Int targetPosition)
    {
        Coroutine EnemyDeathCoroutine = null;
        if (StageManager.Instance.TempTileDictionary[targetPosition][1] == 0)
        {
            EnemyDeathCoroutine = StartCoroutine(EnemyDictionary[targetPosition].GetComponent<NormalEnemy>().EnemyDeath(targetPosition));
        }
        else if (StageManager.Instance.TempTileDictionary[targetPosition][1] == 1)
        {
            EnemyDeathCoroutine = StartCoroutine(EnemyDictionary[targetPosition].GetComponent<ChaserEnemy>().EnemyDeath(targetPosition));
        }
        else if (StageManager.Instance.TempTileDictionary[targetPosition][1] == 2)
        {
            EnemyDeathCoroutine = StartCoroutine(EnemyDictionary[targetPosition].GetComponent<MiniBoss>().EnemyDeath(targetPosition));
        }

        EnemyDeathCoroutineQueue.Enqueue(EnemyDeathCoroutine);
    }
}
