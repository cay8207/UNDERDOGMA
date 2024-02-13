using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class EnemyManager : Singleton<EnemyManager>
{
    public Queue<Coroutine> EnemyDeathCoroutineQueue = new Queue<Coroutine>();
    public Dictionary<Vector2Int, GameObject> GameObjectDictionary => StageManager.Instance.GameObjectDictionary;

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
