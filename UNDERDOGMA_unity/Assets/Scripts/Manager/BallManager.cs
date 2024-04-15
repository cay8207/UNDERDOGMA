using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class BallManager : Singleton<BallManager>
{
    public Queue<Coroutine> KickCoroutineQueue = new Queue<Coroutine>();
    public Dictionary<Vector2Int, GameObject> GameObjectDictionary => StageManager.Instance.GameObjectDictionary;

    public void Kick(Vector2Int targetPosition, KeyCode key)
    {
        Coroutine KickCoroutine = null;

        KickCoroutine = StartCoroutine(GameObjectDictionary[targetPosition].GetComponent<Ball>().Kick(targetPosition, key));

        KickCoroutineQueue.Enqueue(KickCoroutine);
    }

}
