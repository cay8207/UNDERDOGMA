using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ExecutionManager : MonoBehaviour
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static ExecutionManager _instance;

    public static ExecutionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (ExecutionManager)FindObjectOfType(typeof(ExecutionManager));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(ExecutionManager)} (Singleton)");
                    _instance = singletonObject.AddComponent<ExecutionManager>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }
    #endregion

    [SerializeField] GameObject ExecutionPrefab;

    private GameObject ExecutionObject;

    private bool _executionInProgress;
    public bool ExecutionInProgress
    {
        get => _executionInProgress;
        set => _executionInProgress = value;
    }

    private int _executionCount;

    private int _executionHealth;

    public void Start()
    {
        ExecutionObject = Instantiate(ExecutionPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

        _executionCount = 3;
        _executionHealth = 6;
    }

    public void Update()
    {
        Vector2 CameraPosition = StageManager.Instance.MainCamera.transform.position;
        ExecutionObject.transform.position = new Vector3(CameraPosition.x, CameraPosition.y, 0.0f);
    }

    // 매개변수로 해당 스테이지와 현재 이동 수를 받아오면 처형 여부를 결정한다. 
    public bool ExecutionCheck(int moveCount)
    {
        if (moveCount >= _executionCount)
        {
            StartCoroutine(ExecutionEvent(_executionHealth));
            return true;
        }
        return false;
    }

    public void ExecutionStop()
    {
        // 코루틴이 진행중이라면 중지시킨다. 
        if (ExecutionEvent(_executionHealth) != null)
            StopCoroutine(ExecutionEvent(_executionHealth));
        ExecutionObject.GetComponent<Animator>().SetBool("InExecution", false);
    }

    public IEnumerator ExecutionEvent(int executionHealth)
    {
        yield return new WaitForSeconds(1.0f);

        // 처형이 이뤄지는 동안 플레이어는 이동할 수 없다. 
        _executionInProgress = true;

        ExecutionObject.GetComponent<Animator>().SetBool("InExecution", true);

        // 일정 시간 동안 플레이어 조작 멈춤
        yield return new WaitForSeconds(3.0f);

        ExecutionObject.GetComponent<Animator>().SetBool("InExecution", false);

        ExecutionObject.GetComponent<SpriteRenderer>().sprite = null;

        // 특정 체력 이상의 적을 처형
        ExecuteEnemies(executionHealth);

        // 처형 이벤트 종료
        _executionInProgress = false;
    }

    void ExecuteEnemies(int executionHealth)
    {
        // 플레이어의 생명력이 처형 수치 이상이라면 처형.
        if (StageManager.Instance._character.GetComponent<Character>().Heart >= executionHealth)
        {
            StartCoroutine(StageManager.Instance._character.GetComponent<Character>().CharacterDeath());
        }

        foreach (GameObject enemy in StageManager.Instance.EnemyDictionary.Values)
        {
            Debug.Log("enemyHeart: " + enemy.GetComponent<Enemy>().Heart);
            Debug.Log("executionHealth: " + executionHealth);
            if (enemy.GetComponent<Enemy>().Heart >= executionHealth)
            {
                int row = enemy.GetComponent<Enemy>().Row;
                int col = enemy.GetComponent<Enemy>().Col;
                StartCoroutine(enemy.GetComponent<Enemy>().EnemyDeath(new Vector2Int(row, col)));
            }
        }
    }
}

