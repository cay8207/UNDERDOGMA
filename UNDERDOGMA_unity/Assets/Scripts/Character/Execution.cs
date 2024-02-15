using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    // 1. 처형 애니메이션과 관련된 변수들. 
    // 1.1. 관련 UI들을 담는 캔버스.
    [SerializeField] GameObject ExecutionCanvas;
    // 1.2. 처형 횟수를 표시해줄 프리팹.
    [SerializeField] GameObject ExecutionCountPrefab;
    // 1.3. 처형할 적들을 표시해줄 프리팹.
    [SerializeField] GameObject ExecutionTargetPrefab;
    [SerializeField] public Image ExecutionEffect;
    [SerializeField] public Image ExecutionWolf;
    [SerializeField] public GameObject ExecutionClaw;

    [SerializeField] public Sprite CloseEye;
    [SerializeField] public Sprite OpenEye;

    private List<GameObject> _executionCountObjectList = new List<GameObject>();
    public List<GameObject> ExecutionCountObjectList
    {
        get => _executionCountObjectList;
        set => _executionCountObjectList = value;
    }

    private List<GameObject> _executionTargetObjectList = new List<GameObject>();
    public List<GameObject> ExecutionTargetObjectList
    {
        get => _executionTargetObjectList;
        set => _executionTargetObjectList = value;
    }

    private List<GameObject> _executionClawObjectList = new List<GameObject>();
    public List<GameObject> ExecutionClawObjectList
    {
        get => _executionClawObjectList;
        set => _executionClawObjectList = value;
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

    private Dictionary<Vector2Int, GameObject> _executionTargetDictionary = new Dictionary<Vector2Int, GameObject>();

    public void Start()
    {
        ExecutionSetUp();
    }

    public void Update()
    {
        Vector2 CameraPosition = StageManager.Instance.MainCamera.transform.position;

        if (ExecutionCount > 0)
        {
            _executionTargetDictionary = ExecuteEnemies();

            if (_executionTargetDictionary.Count > 0)
            {
                int count = 0;

                foreach (var enemy in _executionTargetDictionary)
                {
                    _executionTargetObjectList[count].transform.position = new Vector3(enemy.Key.x, enemy.Key.y, 0.0f);

                    count++;
                }

                // 나머지 오브젝트는 숨겨준다. 
                for (int i = count; i < 10; i++)
                {
                    _executionTargetObjectList[i].transform.position = new Vector3(-9999.0f, -9999.0f, 0.0f);
                }
            }
        }
    }

    public void ExecutionSetUp()
    {
        // 1. 처형 카운트 설정 및 표시해줄 UI를 생성한다.
        _executionCount = StageManager.Instance._stageData.ExecutionCount;

        for (int i = _executionCount - 1; i >= 0; i--)
        {
            GameObject ExecutionCountObject = Instantiate(ExecutionCountPrefab, new Vector3(37.5f * (_executionCount - 1) - 75.0f * i, 480.0f, 0.0f), Quaternion.identity);
            ExecutionCountObject.transform.SetParent(ExecutionCanvas.transform, false);
            _executionCountObjectList.Add(ExecutionCountObject);
        }

        // 2. 처형당할 적들을 표시해줄 스프라이트. 일단 10개를 만들어서 구석에 두고, 하나씩 표시해준다. 
        for (int i = 0; i < 10; i++)
        {
            GameObject ExecutionTargetObject = Instantiate(ExecutionTargetPrefab, new Vector3(-9999.0f, -9999.0f, 0.0f), Quaternion.identity);
            ExecutionTargetObject.transform.SetParent(ExecutionCanvas.transform, false);
            ExecutionTargetObject.transform.localScale = new Vector2(0.6f, 0.6f);
            _executionTargetObjectList.Add(ExecutionTargetObject);
        }

        // 3. 처형 대상들에게 나타날 스프라이트. 일단 10개를 만들어서 구석에 두고, 하나씩 표시해준다.
        for (int i = 0; i < 10; i++)
        {
            GameObject ExecutionClawObject = Instantiate(ExecutionClaw, new Vector3(-9999.0f, -9999.0f, 0.0f), Quaternion.identity);
            ExecutionClawObject.transform.SetParent(ExecutionCanvas.transform, false);
            ExecutionClawObject.GetComponent<UnityEngine.UI.Image>().DOFade(0.0f, 0.0f);
            _executionClawObjectList.Add(ExecutionClawObject);
        }

        // 3. 처형 애니메이션에 관련된 변수. 미리 꺼둔다. 
        ExecutionEffect.DOFade(0.0f, 0.0f);
        ExecutionWolf.DOFade(0.0f, 0.0f);
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

    // 체력이 가장 높은 적 or 캐릭터들이 처형당해야 한다.
    public Dictionary<Vector2Int, GameObject> ExecuteEnemies()
    {
        // 1. 체력이 가장 높은 오브젝트를 저장하는 변수들을 선언한다.
        int _targetHeart = 0;
        int _targetRow = 0;
        int _targetCol = 0;
        EnemyType _enemyType = EnemyType.NormalEnemy;
        Dictionary<Vector2Int, GameObject> _executionTarget = new Dictionary<Vector2Int, GameObject>();

        // 2. 적들 중에서 체력이 가장 높은 적을 찾는다.
        foreach (var gameObject in StageManager.Instance.GameObjectDictionary)
        {
            int _row = gameObject.Key.x;
            int _col = gameObject.Key.y;

            var tile = StageManager.Instance.TempTileDictionary[new Vector2Int(_row, _col)];

            if (tile.Type == TileType.Enemy)
            {
                if ((tile.EnemyData.EnemyType == EnemyType.NormalEnemy || tile.EnemyData.EnemyType == EnemyType.ChaserEnemy)
                    && tile.EnemyData.IsAlive == true)
                {
                    // 2.1. 만약 해당 적이 체력이 가장 높다면 기존 큐를 비우고 새롭게 추가한다. 
                    if (tile.EnemyData.Heart > _targetHeart)
                    {
                        _targetHeart = tile.EnemyData.Heart;
                        _targetRow = gameObject.Key.x;
                        _targetCol = gameObject.Key.y;
                        _enemyType = tile.EnemyData.EnemyType;

                        _executionTarget.Clear();
                        _executionTarget.Add(new Vector2Int(_targetRow, _targetCol), gameObject.Value);
                    }
                    // 2.2. 만약 체력이 가장 높은 적과 같다면 큐에 넣어준다. 
                    else if (tile.EnemyData.Heart == _targetHeart)
                    {
                        _targetHeart = tile.EnemyData.Heart;
                        _targetRow = gameObject.Key.x;
                        _targetCol = gameObject.Key.y;
                        _enemyType = tile.EnemyData.EnemyType;

                        _executionTarget.Add(new Vector2Int(_targetRow, _targetCol), gameObject.Value);
                    }
                }
            }
        }

        // 3. 플레이어의 생명력이 가장 높은거라면 기존 큐를 비워주고 처형해야 하는 오브젝트의 큐에 넣어준다. 
        if (StageManager.Instance._character.GetComponent<Character>().Heart > _targetHeart)
        {
            _targetHeart = StageManager.Instance._character.GetComponent<Character>().Heart;
            _targetRow = StageManager.Instance._character.GetComponent<Character>().Row;
            _targetCol = StageManager.Instance._character.GetComponent<Character>().Col;

            _executionTarget.Clear();
            _executionTarget.Add(new Vector2Int(_targetRow, _targetCol), StageManager.Instance._character);
        }
        // 4. 가장 높은 적과 같다면 함께 큐에 넣어준다. 
        else if (StageManager.Instance._character.GetComponent<Character>().Heart == _targetHeart)
        {
            _targetHeart = StageManager.Instance._character.GetComponent<Character>().Heart;
            _targetRow = StageManager.Instance._character.GetComponent<Character>().Row;
            _targetCol = StageManager.Instance._character.GetComponent<Character>().Col;

            _executionTarget.Add(new Vector2Int(_targetRow, _targetCol), StageManager.Instance._character);
        }

        return _executionTarget;
    }
}

