using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CoroutineController : MonoBehaviour
{
    public Queue<IEnumerator> CharacterCoroutineQueue = new Queue<IEnumerator>();
    Character _character;
    IEnumerator _nowCoroutine;

    public void Awake()
    {
        _character = GetComponent<Character>();
    }

    // 코루틴을 큐에 넣어준다. 
    public void EnqueueCoroutine(IEnumerator coroutine)
    {
        CharacterCoroutineQueue.Enqueue(coroutine);
    }

    // 만약 코루틴이 실행중이라면 false를 반환하고, 그렇지 않다면 코루틴을 실행한다. 
    public void ExecuteCoroutine()
    {
        if (_character.IsCharacterCoroutineRunning == true)
        {
            return;
        }

        if (CharacterCoroutineQueue.Count != 0)
        {
            _nowCoroutine = CharacterCoroutineQueue.Dequeue();

            if (_nowCoroutine != null)
            {
                StartCoroutine(_nowCoroutine);
            }

            Debug.Log(_character.IsCharacterCoroutineRunning);
        }
    }
}
