using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

// 게임 매니저에서는 전반적으로 게임 진행과 관련된 것들을 컨트롤한다.
// 최대한 핵심적인 부분만 담고, 나머지는 모두 다른 클래스에서 담당하도록 한다.
public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject StageManagerPrefab;
    [SerializeField] GameObject ExecutionManagerPrefab;
    [SerializeField] GameObject DialogueManagerPrefab;

    StageManager _stageManager;
    ExecutionManager _executionManager;
    DialogueManager _dialogueManager;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {

    }

    // 스테이지를 시작하면 stageManager, executionManager, dialogueManager를 생성한다.
    public void StageInit()
    {

    }
}
