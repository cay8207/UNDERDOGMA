using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : Singleton<LoadingManager>
{
    // 1. 프리팹들을 할당하는 변수들. 
    [SerializeField] GameObject StageManagerPrefab;
    [SerializeField] GameObject ExecutionPrefab;
    [SerializeField] GameObject DialogueManagerPrefab;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // 스테이지가 아닌 씬에서 스테이지 씬으로 이동하는 함수. 
    public void LoadStage(int world, int stage)
    {
        GameManager.Instance.Stage = stage;

        // Stage 씬으로 이동하고,
        StartCoroutine(LoadSceneAsync("Stage", world, stage));
    }

    // 스테이지에서 다른 스테이지로 이동하는 함수.
    // 이 함수는 StageManager에서 호출된다. 기존의 StageManager와 Execution, DialogueManager는 삭제된다. 그리고 다시 생성한다.
    public void LoadNextStage(int nextWorld, int nextStage)
    {
        GameManager.Instance.Stage = nextStage;

        // StageManager, Execution, DialogueManager를 삭제한다.
        Destroy(GameObject.Find("StageManager"));
        Destroy(GameObject.Find("Execution"));
        Destroy(GameObject.Find("DialogueManager"));

        CreateManagers(nextWorld, nextStage);
    }

    private IEnumerator LoadSceneAsync(string sceneName, int world, int stage)
    {
        // 비동기적으로 Scene을 로드합니다.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Scene 로드가 완료될 때까지 대기합니다.
        while (!asyncLoad.isDone)
        {
            // 로드 진행률을 확인하거나 다른 작업을 수행할 수 있습니다.
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // progress 값은 0.0에서 0.9 사이입니다.

            // 다음 프레임까지 대기합니다.
            yield return null;
        }

        CreateManagers(world, stage);
    }

    private void CreateManagers(int world, int stage)
    {
        GameObject stageManager = Instantiate(StageManagerPrefab);
        stageManager.GetComponent<StageManager>().Init(world, stage);

        GameObject executionManager = Instantiate(ExecutionPrefab);
        executionManager.GetComponent<ExecutionManager>().Init(world, stage);

        GameObject dialogueManager = Instantiate(DialogueManagerPrefab);
        dialogueManager.GetComponent<DialogueManager>().Init(world, stage);
    }
}