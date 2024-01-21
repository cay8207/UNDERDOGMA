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
            Coroutine EnemyActionCoroutine = StartCoroutine(enemy.Value.GetComponent<NormalEnemy>().EnemyAction(PlayerPosition.x, PlayerPosition.y));
            EnemyActionCoroutineQueue.Enqueue(EnemyActionCoroutine);
        }
    }

    public void EnemyDeath(Vector2Int targetPosition)
    {
        Coroutine EnemyDeathCoroutine = StartCoroutine(EnemyDictionary[targetPosition].GetComponent<NormalEnemy>().EnemyDeath(targetPosition));
        EnemyDeathCoroutineQueue.Enqueue(EnemyDeathCoroutine);
    }
}
