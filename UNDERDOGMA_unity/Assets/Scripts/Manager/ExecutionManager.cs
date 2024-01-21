using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] GameObject ExecutionCanvas;
    [SerializeField] GameObject ExecutionHealthObject;
    [SerializeField] GameObject ExecutionCountPrefab;

    [SerializeField] public Sprite CloseEye;
    [SerializeField] public Sprite OpenEye;

    private GameObject ExecutionObject;

    private List<GameObject> _executionCountObjectList = new List<GameObject>();
    public List<GameObject> ExecutionCountObjectList
    {
        get => _executionCountObjectList;
        set => _executionCountObjectList = value;
    }


    private bool _executionInProgress;
    public bool ExecutionInProgress
    {
        get => _executionInProgress;
        set => _executionInProgress = value;
    }

    private int _executionCount;

    public int ExecutionCount
    {
        get => _executionCount;
        set => _executionCount = value;
    }

    private int _executionHealth;

    public int ExecutionHealth
    {
        get => _executionHealth;
        set => _executionHealth = value;
    }

    private Coroutine _executionCoroutine;

    public void Start()
    {
        ExecutionSetUp();
    }

    public void Update()
    {
        Vector2 CameraPosition = StageManager.Instance.MainCamera.transform.position;
        ExecutionObject.transform.position = new Vector3(CameraPosition.x, CameraPosition.y, 0.0f);

        if (Input.GetKeyDown(KeyCode.F))
        {
            ExecutionStop();
        }
    }

    public void ExecutionSetUp()
    {
        _executionCount = StageManager.Instance._stageData.ExecutionCount;
        _executionHealth = StageManager.Instance._stageData.ExecutionHealth;

        ExecutionObject = Instantiate(ExecutionPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

        ExecutionHealthObject.GetComponentInChildren<TextMeshProUGUI>().text = _executionHealth.ToString();
        for (int i = _executionCount - 1; i >= 0; i--)
        {
            GameObject ExecutionCountObject = Instantiate(ExecutionCountPrefab, new Vector3(640.0f - 100.0f * i, 400.0f, 0.0f), Quaternion.identity);
            ExecutionCountObject.transform.SetParent(ExecutionCanvas.transform, false);
            _executionCountObjectList.Add(ExecutionCountObject);
        }

    }

    // 매개변수로 해당 스테이지와 현재 이동 수를 받아오면 처형 여부를 결정한다. 
    public bool ExecutionCheck(int moveCount)
    {
        // 이동 횟수가 처형 횟수보다 많거나 0 이하이면 로직이 잘못된 것. false를 반환. 
        if (moveCount > _executionCount || moveCount <= 0)
        {
            return false;
        }

        // 처형 카운트. 즉 상단에 있는 눈이 추가로 떠지도록 한다. 
        _executionCountObjectList[moveCount - 1].GetComponent<Image>().sprite = OpenEye;

        if (moveCount >= _executionCount)
        {
            _executionCoroutine = StartCoroutine(ExecutionEvent());
            for (int i = 0; i < moveCount; i++)
            {
                _executionCountObjectList[i].GetComponent<Image>().sprite = CloseEye;
            }
            return true;
        }
        return false;
    }

    public void ExecutionStop()
    {
        // 코루틴이 진행중이라면 중지시킨다. 
        if (_executionCoroutine != null)
            StopCoroutine(_executionCoroutine);
        ExecutionObject.GetComponent<Animator>().SetBool("InExecution", false);

        // 만약 처형이 진행되다가 멈춰서 처형중임을 나타내는 변수가 true라면 false로 바꿔준다. 캐릭터가 이동 가능하도록. 
        if (_executionInProgress == true)
        {
            // 처형 또한 진행되지 않았을 수 있기 때문에 처형을 진행해준다.
            ExecuteEnemies();
            _executionInProgress = false;
        }

    }

    public IEnumerator ExecutionEvent()
    {
        // 1. 처형이 이뤄지는 동안 플레이어는 이동할 수 없다. 
        _executionInProgress = true;

        // 2. 처형 애니메이션 이전에 적이 공격하는 애니메이션 등을 보여주기 위해 잠깐의 텀을 둔다. 
        yield return new WaitForSeconds(1.0f);

        ExecutionObject.GetComponent<Animator>().SetBool("InExecution", true);

        // 3. 처형 애니메이션 보여줌.
        yield return new WaitForSeconds(3.0f);

        // 4. 애니메이션 종료.
        ExecutionObject.GetComponent<Animator>().SetBool("InExecution", false);

        ExecutionObject.GetComponent<SpriteRenderer>().sprite = null;


        // 5. 특정 체력 이상의 적을 처형
        ExecuteEnemies();

        // 6. 처형 이벤트 종료. 캐릭터는 다시 움직일 수 있다. 
        _executionInProgress = false;
    }

    void ExecuteEnemies()
    {
        Debug.Log("execute Enemies!");
        // 플레이어의 생명력이 처형 수치 이상이라면 처형.
        if (StageManager.Instance._character.GetComponent<Character>().Heart >= _executionHealth)
        {
            StartCoroutine(StageManager.Instance._character.GetComponent<Character>().CharacterDeath());
            return;
        }

        foreach (GameObject enemy in StageManager.Instance.EnemyDictionary.Values)
        {
            Debug.Log("enemyHeart: " + enemy.GetComponent<NormalEnemy>().Heart);
            Debug.Log("executionHealth: " + _executionHealth);
            if (enemy.GetComponent<NormalEnemy>().Heart >= _executionHealth)
            {
                int row = enemy.GetComponent<NormalEnemy>().Row;
                int col = enemy.GetComponent<NormalEnemy>().Col;
                EnemyManager.Instance.EnemyDeath(new Vector2Int(row, col));
            }
        }
    }
}

