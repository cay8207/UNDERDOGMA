using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static StageManager _instance;

    public static StageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (StageManager)FindObjectOfType(typeof(StageManager));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(StageManager)} (Singleton)");
                    _instance = singletonObject.AddComponent<StageManager>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }
    #endregion

    // SerializeField가 붙은것들은 프리팹을 저장하기 위한 변수들. 
    [SerializeField] public GameObject MainCamera;
    [SerializeField] GameObject TilePrefab;
    [SerializeField] GameObject CharacterPrefab;
    [SerializeField] GameObject MeatPrefab;
    [SerializeField] GameObject NormalEnemyPrefab;
    [SerializeField] GameObject ChaserEnemyPrefab;
    [SerializeField] GameObject MiniBossPrefab;
    [SerializeField] public int stage;
    [SerializeField] public GameObject ResetAnimationUpSide;
    [SerializeField] public GameObject ResetAnimationDownSide;


    // 위의 변수와 다르게 _character는 생성된 게임오브젝트를 저장하기 위한 변수. 
    public GameObject _character;

    public StageData _stageData;

    // 기본적인 타일 구조는 StageData의 TileDictionary에 저장되어있다.
    // 하지만 게임을 진행하면서 데이터가 계속해서 바뀌어야 하는데, 리셋할때에는 또 초기 정보가 필요하다.
    // 그래서 변경해도 상관없도록 복사한 Dictionary를 하나 만들어서 데이터를 그쪽에서 관리하고,
    // 기존의 TileDictionary는 리셋할때에만 사용한다. 
    private Dictionary<Vector2Int, TileObject> _tempTileDictionary = new Dictionary<Vector2Int, TileObject>();
    public Dictionary<Vector2Int, TileObject> TempTileDictionary
    {
        get => _tempTileDictionary;
        set => _tempTileDictionary = value;
    }

    // 현재 게임에 존재하는 모든 오브젝트들을 관리하는 dictionary.
    private Dictionary<Vector2Int, GameObject> _gameObjectDictionary = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, GameObject> GameObjectDictionary
    {
        get => _gameObjectDictionary;
        set => _gameObjectDictionary = value;
    }

    // Start is called before the first frame update
    public void Awake()
    {

        // Dialogue dialogue = new Dialogue();
        // dialogue.Init();
        // DialogueManager.Instance.ShowDialogue(dialogue);

        string path = "Stage" + stage.ToString();
        _stageData = StageDataLoader.Instance.LoadStageData(path);

        _tempTileDictionary = _stageData.TileDictionary;

        TileInstantiate();

        AudioManager.Instance.Init();
        AudioManager.Instance.PlayBgm(true);
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void TileInstantiate()
    {
        // 타일들을 하나씩 만들어준다. 
        GameObject Tiles = new GameObject("Tiles");

        foreach (var tile in _stageData.TileDictionary)
        {
            Vector3 tilePosition = new Vector3(tile.Key.x, tile.Key.y, 0);

            // -1인 경우는 벽. 이외의 경우에만 타일 만들어주면 된다. 
            if (tile.Value.Type != TileType.Wall)
            {
                GameObject newTile = Instantiate(TilePrefab, tilePosition, Quaternion.identity, Tiles.transform);
            }

            if (tile.Value.Type == TileType.Enemy)
            {
                GameObject newEnemy = InstantiateEnemy(tile.Value.EnemyData.EnemyType, tilePosition);
                if (newEnemy != null)
                {
                    SetEnemyAttributes(newEnemy, tile.Value.EnemyData);
                    SetEnemyPosition(newEnemy, tile.Key.x, tile.Key.y);
                    _gameObjectDictionary.Add(new Vector2Int(tile.Key.x, tile.Key.y), newEnemy);
                }
            }

            if (tile.Value.Type == TileType.Meat)
            {
                GameObject newMeat = Instantiate(MeatPrefab, tilePosition, Quaternion.identity);
                newMeat.GetComponent<Meat>().Amount = tile.Value.MeatData.Amount;

                _gameObjectDictionary.Add(new Vector2Int(tile.Key.x, tile.Key.y), newMeat);
            }
        }

        // 캐릭터 오브젝트를 생성하고 초기화해준다. 
        _character = Instantiate(CharacterPrefab, new Vector3(_stageData.CharacterRow - 0.07f, _stageData.CharacterCol + 0.35f, 0.0f), Quaternion.identity);
        _character.GetComponent<Character>().Init(_stageData.CharacterRow, _stageData.CharacterCol, _stageData.CharacterHeart);
        MainCamera.GetComponent<MainCamera>()._character = _character;
    }

    public void DestroyAllObjects()
    {
        // 일단 변경된 데이터들을 모두 제대로 되돌려준다. 
        _tempTileDictionary = _stageData.TileDictionary;

        // 맵에 존재하는 타일을 제외한 오브젝트들을 초기화시켜준다.
        // 1. 만약 처형이 진행중이라면 처형을 멈추고, 처형에 관한 변수들을 초기화시켜준다.
        Execution.Instance.ExecutionStop();

        for (int i = 0; i < Execution.Instance.ExecutionCount; i++)
        {
            Execution.Instance.ExecutionCountObjectList[i].GetComponent<Image>().sprite = Execution.Instance.CloseEye;
            Execution.Instance.ExecutionCountObjectList[i].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(69.0f, 17.0f);
        }

        // 2. 적들을 모두 초기화해준다. 
        // 적의 코루틴들을 모두 스탑시켜준다. 그렇지 않으면 진행되고 있던 코루틴이 리셋 이후 버그를 일으킬 수 있음. 
        foreach (var coroutine in EnemyManager.Instance.EnemyActionCoroutineQueue)
        {
            if (coroutine != null)
            {
                EnemyManager.Instance.StopCoroutine(coroutine);
            }
        }

        foreach (var coroutine in EnemyManager.Instance.EnemyDeathCoroutineQueue)
        {
            if (coroutine != null)
            {
                EnemyManager.Instance.StopCoroutine(coroutine);

            }
        }

        // 3. enemy 게임오브젝트들을 파괴. 
        foreach (var gameObjectWithVector in _gameObjectDictionary)
        {
            gameObjectWithVector.Value.SetActive(false);
            Destroy(gameObjectWithVector.Value);
        }
        _gameObjectDictionary.Clear();

        // 4. 캐릭터를 초기화해준다.
        Destroy(_character);
    }

    private GameObject InstantiateEnemy(EnemyType enemyType, Vector3 position)
    {
        GameObject prefab = GetEnemyPrefab(enemyType);
        if (prefab != null)
        {
            return Instantiate(prefab, position + new Vector3(-0.06f, 0.3f, 0.0f), Quaternion.identity);
        }

        return null;
    }

    private void SetEnemyAttributes(GameObject enemy, EnemyData enemyData)
    {
        if (enemy == null)
        {
            return;
        }

        if (enemy.TryGetComponent(out IEnemyAttributesSetter attributesSetter))
        {
            attributesSetter.SetAttributes(enemyData);
        }
    }

    private void SetEnemyPosition(GameObject enemy, int row, int col)
    {
        if (enemy == null)
        {
            return;
        }

        if (enemy.TryGetComponent(out IEnemyPositionSetter positionSetter))
        {
            positionSetter.Row = row;
            positionSetter.Col = col;
        }
    }

    private GameObject GetEnemyPrefab(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.NormalEnemy:
                return NormalEnemyPrefab;
            case EnemyType.ChaserEnemy:
                return ChaserEnemyPrefab;
            case EnemyType.MiniBoss:
                return MiniBossPrefab;
            default:
                return null;
        }
    }
}
