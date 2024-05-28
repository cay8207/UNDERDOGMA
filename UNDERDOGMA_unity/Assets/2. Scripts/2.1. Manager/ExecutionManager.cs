using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting.FullSerializer.Internal;

public class ExecutionManager : MonoBehaviour
{
    // 싱글톤 패턴. 인스턴스를 하나만 만들어서 사용한다. 
    private static ExecutionManager _instance;

    public static ExecutionManager Instance
    {
        get => _instance;
        set => _instance = value;
    }

    // 1. 처형 애니메이션과 관련된 변수들. 
    // 1.1. 관련 UI들을 담는 캔버스.
    [SerializeField] GameObject ExecutionCanvas;
    // 1.2. 처형 횟수를 표시해줄 프리팹.
    [SerializeField] GameObject ExecutionCountPrefab;
    // 1.3. 처형할 적들을 표시해줄 프리팹.
    [SerializeField] Image ExecutionBackGround;
    [SerializeField] TextMeshProUGUI ExecutionDescription;
    [SerializeField] TextMeshProUGUI ExecutionNum;
    [SerializeField] TextMeshProUGUI ExecutionText;
    [SerializeField] Image ControlDescription;
    [SerializeField] GameObject ExecutionTargetPrefab;
    [SerializeField] public Image ExecutionEffect;
    [SerializeField] public Image ExecutionWolf;
    [SerializeField] public GameObject ExecutionClaw;

    [SerializeField] public GameObject RemainEnemyPrefab;
    [SerializeField] public GameObject RemainEnemyText;
    [SerializeField] public GameObject RemainEnemyBackGround;

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

    private List<GameObject> _remainEnemyObjectList = new List<GameObject>();
    public List<GameObject> RemainEnemyObjectList
    {
        get => _remainEnemyObjectList;
        set => _remainEnemyObjectList = value;
    }

    private GameObject RemainEnemyBackgroundObject;
    private GameObject RemainEnemyTextObject;

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

    private int fadeCount = 0;
    private bool canFade = false;

    public StageData _stageData;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {

    }

    public void Init(int world, int stage)
    {
        ExecutionSetUp(world, stage);
        SetupRemainEnemyUI();
    }

    public void Update()
    {
        UpdateExecutionTargetUI();

        UpdateRemainEnemyUI();

        if (StageManager.Instance._character != null)
        {
            ExecutionNum.SetText((ExecutionCount - StageManager.Instance._character.GetComponent<Character>().MoveCount).ToString());
        }
    }

    // 남은 적 UI를 세팅해주는 함수. 
    public void SetupRemainEnemyUI()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject RemainEnemyObject = Instantiate(RemainEnemyPrefab, new Vector3(-9999.0f, -9999.0f, 0.0f), Quaternion.identity);
            RemainEnemyObject.transform.SetParent(ExecutionCanvas.transform, false);
            RemainEnemyObject.transform.localScale = new Vector2(0.6f, 0.6f);
            _remainEnemyObjectList.Add(RemainEnemyObject);
        }
    }

    // 처형 대상을 표시하는 함수. 매 프레임마다 처형될 개체들을 찾고, 이들을 표시해준다. 
    public void UpdateExecutionTargetUI()
    {
        // 만약 캐릭터가 존재하고, 캐릭터가 리셋 중이 아니라면 처형 대상을 찾아서 표시해준다.
        if (StageManager.Instance._character != null)
        {
            if (StageManager.Instance._character.GetComponent<Character>().IsCharacterResetCoroutineRunning == false)
            {
                if (ExecutionCount > 0)
                {
                    // 처형 대상들을 찾아 딕셔너리에 저장한다. 
                    _executionTargetDictionary = FindExecutionTargets();

                    if (_executionTargetDictionary.Count > 0)
                    {
                        canFade = true;

                        int count = 0;

                        // 처형 대상들을 표시해준다. 빨간 원으로 적을 표시한다. 
                        foreach (var enemy in _executionTargetDictionary)
                        {
                            _executionTargetObjectList[count].transform.position = new Vector3(enemy.Key.x, enemy.Key.y + 0.2f, 0.0f);

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
            else
            {
                // 처형 UI가 보이면 안 되기 때문에 안 보이는 곳에 숨긴다. 
                for (int i = 0; i < 10; i++)
                {
                    _executionTargetObjectList[i].transform.position = new Vector3(-9999.0f, -9999.0f, 0.0f);
                }
            }
        }

        // 
        if (canFade)
        {
            for (int i = 0; i < 10; i++)
            {
                if (fadeCount % 500 < 250)
                {
                    _executionTargetObjectList[i].GetComponent<SpriteRenderer>()
                        .DOFade(0.8f - 0.0012f * (fadeCount % 500), 0.0f);
                }
                else
                {
                    _executionTargetObjectList[i].GetComponent<SpriteRenderer>()
                        .DOFade(0.5f + 0.0012f * (fadeCount % 500 - 250), 0.0f);
                }
            }

            fadeCount++;
        }
    }

    // 남은 적들을 표시하는 UI를 업데이트하는 함수.
    public void UpdateRemainEnemyUI()
    {
        int _row = 0;
        int _col = 0;
        int _enemyCount = 0;

        // 1. 살아있는 적들의 수를 세준다. 
        foreach (var gameObject in StageManager.Instance.GameObjectDictionary)
        {
            _row = gameObject.Key.x;
            _col = gameObject.Key.y;

            if (StageManager.Instance.TempTileDictionary[new Vector2Int(_row, _col)].Type == TileType.Enemy)
            {
                if (StageManager.Instance.TempTileDictionary[new Vector2Int(_row, _col)].EnemyData.IsAlive == true)
                {
                    _enemyCount++;
                }

            }
        }

        // 2. 처형 중이라면 UI를 숨겨준다.
        if (StageManager.Instance._character != null)
        {
            if (StageManager.Instance._character.GetComponent<Character>().IsCharacterExecutionCoroutineRunning == true)
            {
                ExecutionBackGround.enabled = false;
                ExecutionDescription.enabled = false;
                ExecutionNum.enabled = false;
                ExecutionText.enabled = false;
                ControlDescription.enabled = false;

                RemainEnemyBackgroundObject.transform.localPosition = new Vector3(-9999.0f, -9999.0f, 0.0f);
                RemainEnemyTextObject.transform.localPosition = new Vector3(-9999.0f, -9999.0f, 0.0f);

                for (int i = 0; i < _enemyCount; i++)
                {
                    _remainEnemyObjectList[i].transform.localPosition = new Vector3(-9999.0f, -9999.0f, 0.0f);
                }
                for (int i = _enemyCount; i < 10; i++)
                {
                    _remainEnemyObjectList[i].transform.localPosition = new Vector3(-9999.0f, -9999.0f, 0.0f);
                }
            }
            // 3. 처형 중이 아니라면 UI를 보여준다.
            else
            {
                ExecutionBackGround.enabled = true;
                ExecutionDescription.enabled = true;
                ExecutionNum.enabled = true;
                ExecutionText.enabled = true;
                ControlDescription.enabled = true;

                RemainEnemyBackgroundObject.transform.localPosition = new Vector3(-850.0f, 37.5f * (_enemyCount) + 75.0f, 0.0f);
                RemainEnemyTextObject.transform.localPosition = new Vector3(-850.0f, 37.5f * (_enemyCount) + 75.0f, 0.0f);

                for (int i = 0; i < _enemyCount; i++)
                {
                    _remainEnemyObjectList[i].transform.localPosition = new Vector3(-850.0f, 37.5f * (_enemyCount) - 75.0f * i, 0.0f);
                }
                for (int i = _enemyCount; i < 10; i++)
                {
                    _remainEnemyObjectList[i].transform.localPosition = new Vector3(-9999.0f, -9999.0f, 0.0f);
                }
            }
        }
    }

    public void ExecutionSetUp(int world, int stage)
    {
        // 1. 스테이지 데이터를 불러온다.
        string path = "Stage" + (world * 100 + stage).ToString();
        _stageData = StageDataLoader.Instance.LoadStageData(path);

        // 2. 처형 카운트 설정 및 표시해줄 UI를 생성한다.
        _executionCount = _stageData.ExecutionCount;

        // 3. 처형당할 적들을 표시해줄 스프라이트. 일단 10개를 만들어서 구석에 두고, 하나씩 표시해준다. 
        for (int i = 0; i < 10; i++)
        {
            GameObject ExecutionTargetObject = Instantiate(ExecutionTargetPrefab, new Vector3(-9999.0f, -9999.0f, 0.0f), Quaternion.identity);
            ExecutionTargetObject.transform.SetParent(ExecutionCanvas.transform, false);
            ExecutionTargetObject.transform.localScale = new Vector2(0.6f, 0.6f);
            _executionTargetObjectList.Add(ExecutionTargetObject);
        }

        // 4. 처형 대상들에게 나타날 스프라이트. 일단 10개를 만들어서 구석에 두고, 하나씩 표시해준다.
        for (int i = 0; i < 100; i++)
        {
            GameObject ExecutionClawObject = Instantiate(ExecutionClaw, new Vector3(-9999.0f, -9999.0f, 0.0f), Quaternion.identity);
            ExecutionClawObject.transform.SetParent(ExecutionCanvas.transform, false);
            ExecutionClawObject.GetComponent<UnityEngine.UI.Image>().DOFade(0.0f, 0.0f);
            _executionClawObjectList.Add(ExecutionClawObject);
        }

        RemainEnemyBackgroundObject = Instantiate(RemainEnemyBackGround, new Vector3(-850.0f, 0.0f, 0.0f), Quaternion.identity);
        RemainEnemyBackgroundObject.transform.SetParent(ExecutionCanvas.transform, false);
        RemainEnemyTextObject = Instantiate(RemainEnemyText, new Vector3(-850.0f, 0.0f, 0.0f), Quaternion.identity);
        RemainEnemyTextObject.transform.SetParent(ExecutionCanvas.transform, false);

        // 5. 처형 애니메이션에 관련된 변수. 미리 꺼둔다. 
        ExecutionEffect.DOFade(0.0f, 0.0f);
        ExecutionWolf.DOFade(0.0f, 0.0f);
    }


    // 체력이 가장 높은 적 or 캐릭터들이 처형당해야 한다.
    public Dictionary<Vector2Int, GameObject> FindExecutionTargets()
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
                if ((tile.EnemyData.EnemyType == EnemyType.NormalEnemy || tile.EnemyData.EnemyType == EnemyType.Chaser
                    || tile.EnemyData.EnemyType == EnemyType.StrongAttack || tile.EnemyData.EnemyType == EnemyType.AllDirectionsAttack
                    || tile.EnemyData.EnemyType == EnemyType.Angel)
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

