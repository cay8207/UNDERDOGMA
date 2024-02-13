using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Execution : MonoBehaviour
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static Execution _instance;

    public static Execution Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Execution)FindObjectOfType(typeof(Execution));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(Execution)} (Singleton)");
                    _instance = singletonObject.AddComponent<Execution>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }
    #endregion

    [SerializeField] GameObject ExecutionPrefab;
    [SerializeField] GameObject ExecutionCanvas;
    [SerializeField] GameObject ExecutionHealthText;
    [SerializeField] GameObject ExecutionCountPrefab;

    [SerializeField] public Sprite CloseEye;
    [SerializeField] public Sprite OpenEye;

    public GameObject ExecutionObject;

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

    public int ExecutionCount => _executionCount;

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

        ExecutionObject = Instantiate(ExecutionPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

        for (int i = _executionCount - 1; i >= 0; i--)
        {
            GameObject ExecutionCountObject = Instantiate(ExecutionCountPrefab, new Vector3(37.5f * (_executionCount - 1) - 75.0f * i, 480.0f, 0.0f), Quaternion.identity);
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
        _executionCountObjectList[moveCount - 1].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(90.0f, 56.0f);

        if (moveCount >= _executionCount)
        {
            for (int i = 0; i < moveCount; i++)
            {
                _executionCountObjectList[i].GetComponent<Image>().sprite = CloseEye;
                _executionCountObjectList[i].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(69.0f, 17.0f);
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

    // 체력이 가장 높은 적 or 캐릭터가 처형당해야 한다.
    public GameObject ExecuteEnemies()
    {
        Debug.Log("execute Enemies!");

        // 1. 체력이 가장 높은 오브젝트를 저장하는 변수들을 선언한다.
        int _targetHeart = 0;
        int _targetRow = 0;
        int _targetCol = 0;
        EnemyType _enemyType = EnemyType.NormalEnemy;
        GameObject _executionTarget = null;

        // 2. 적들 중에서 체력이 가장 높은 적을 찾는다.
        foreach (var gameObject in StageManager.Instance.GameObjectDictionary)
        {
            int _row = gameObject.Key.x;
            int _col = gameObject.Key.y;

            var tile = StageManager.Instance.TempTileDictionary[new Vector2Int(_row, _col)];

            if (tile.Type == TileType.Enemy && tile.EnemyData.IsAlive == true)
            {
                if (tile.EnemyData.Heart > _targetHeart)
                {
                    _targetHeart = tile.EnemyData.Heart;
                    _targetRow = gameObject.Key.x;
                    _targetCol = gameObject.Key.y;
                    _enemyType = tile.EnemyData.EnemyType;
                    _executionTarget = gameObject.Value;
                }
            }
        }

        // 3. 플레이어의 생명력이 가장 높은거라면 플레이어를 처형.
        if (StageManager.Instance._character.GetComponent<Character>().Heart > _targetHeart)
        {
            _targetHeart = StageManager.Instance._character.GetComponent<Character>().Heart;
            _executionTarget = StageManager.Instance._character;
        }
        // 4. 그게 아니라면 적을 처형.
        else
        {
            EnemyManager.Instance.EnemyDeath(new Vector2Int(_targetRow, _targetCol));
        }

        return _executionTarget;
    }
}

