using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class EnemyManager : Singleton<EnemyManager>
{
    public Dictionary<Vector2Int, GameObject> GameObjectDictionary => StageManager.Instance.GameObjectDictionary;

    public Queue<Coroutine> EnemyActionCoroutineQueue = new Queue<Coroutine>();
    public Queue<Coroutine> EnemyDeathCoroutineQueue = new Queue<Coroutine>();

    public void EnemyTurn()
    {
        Vector2Int PlayerPosition = new Vector2Int(StageManager.Instance._character.GetComponent<Character>().Row,
                                                    StageManager.Instance._character.GetComponent<Character>().Col);
        // 적의 턴이 시작되면 적들의 action을 모두 실행. 
        foreach (var gameObject in GameObjectDictionary)
        {
            int enemyRow = gameObject.Key.x;
            int enemyCol = gameObject.Key.y;
            Coroutine EnemyActionCoroutine = null;
            // 0번 타입 적인경우
            if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.NormalEnemy)
            {
                EnemyActionCoroutine = StartCoroutine(gameObject.Value.GetComponent<NormalEnemy>().EnemyAction(PlayerPosition.x, PlayerPosition.y));
            }
            // 1번 타입 적인 경우
            else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.ChaserEnemy)
            {
                EnemyActionCoroutine = StartCoroutine(gameObject.Value.GetComponent<ChaserEnemy>().EnemyAction(PlayerPosition.x, PlayerPosition.y));
            }
            else if (StageManager.Instance.TempTileDictionary[new Vector2Int(enemyRow, enemyCol)].EnemyData.EnemyType == EnemyType.MiniBoss)
            {
                EnemyActionCoroutine = StartCoroutine(gameObject.Value.GetComponent<MiniBoss>().EnemyAction(PlayerPosition.x, PlayerPosition.y));
            }

            EnemyActionCoroutineQueue.Enqueue(EnemyActionCoroutine);
        }
    }

    public void EnemyDeath(Vector2Int targetPosition)
    {
        Coroutine EnemyDeathCoroutine = null;
        if (StageManager.Instance.TempTileDictionary[targetPosition].EnemyData.EnemyType == EnemyType.NormalEnemy)
        {
            EnemyDeathCoroutine = StartCoroutine(GameObjectDictionary[targetPosition].GetComponent<NormalEnemy>().EnemyDeath(targetPosition));
        }
        else if (StageManager.Instance.TempTileDictionary[targetPosition].EnemyData.EnemyType == EnemyType.ChaserEnemy)
        {
            EnemyDeathCoroutine = StartCoroutine(GameObjectDictionary[targetPosition].GetComponent<ChaserEnemy>().EnemyDeath(targetPosition));
        }
        else if (StageManager.Instance.TempTileDictionary[targetPosition].EnemyData.EnemyType == EnemyType.MiniBoss)
        {
            EnemyDeathCoroutine = StartCoroutine(GameObjectDictionary[targetPosition].GetComponent<MiniBoss>().EnemyDeath(targetPosition));
        }

        EnemyDeathCoroutineQueue.Enqueue(EnemyDeathCoroutine);
    }
}
