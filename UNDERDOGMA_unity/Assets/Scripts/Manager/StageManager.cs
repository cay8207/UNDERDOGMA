using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
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

    // 1. SerializeField가 붙은것들은 프리팹을 저장하기 위한 변수들. 

    // 1.1. 피격 시 카메라 쉐이킹 등을 위함. 
    [SerializeField] public GameObject MainCamera;

    // 1.2. 오브젝트들을 그려주기 위한 프리팹 및 스프라이트들. 
    [SerializeField] GameObject TilePrefab;
    [SerializeField] GameObject CharacterPrefab;
    [SerializeField] GameObject MeatPrefab;
    [SerializeField] GameObject NormalEnemyPrefab;
    [SerializeField] GameObject ChaserEnemyPrefab;
    [SerializeField] GameObject MiniBossPrefab;
    [SerializeField] List<Sprite> TileSprites;


    // 1.4. 리셋 애니메이션을 보여주기 위한 오브젝트. 
    [SerializeField] public GameObject ResetAnimationUpSide;
    [SerializeField] public GameObject ResetAnimationDownSide;

    // 1.5. 툴팁 애니메이션을 위한 오브젝트.
    [SerializeField] GameObject toolTip;

    // 2. 위의 변수와 다르게 _character는 생성된 게임오브젝트를 저장하기 위한 변수. 
    public GameObject _character;

    public StageData _stageData;

    // 2.1. 일시정지 메뉴
    [SerializeField] private GameObject pauseCanvasPrefab;
    private GameObject pauseCanvas;
    private bool isPauseCanvasOpened;

    // 3. 스테이지의 데이터, 오브젝트 등을 저장하기 위한 변수들.

    // 3.1. 현재 스테이지를 저장하기 위한 변수.
    public int stage;

    // 3.2. 기본적인 타일 구조는 StageData의 TileDictionary에 저장되어있다.
    // 하지만 게임을 진행하면서 데이터가 계속해서 바뀌어야 하는데, 리셋할때에는 또 초기 정보가 필요하다.
    // 그래서 변경해도 상관없도록 복사한 Dictionary를 하나 만들어서 데이터를 그쪽에서 관리하고,
    // 기존의 TileDictionary는 리셋할때에만 사용한다. 
    private Dictionary<Vector2Int, TileObject> _tempTileDictionary = new Dictionary<Vector2Int, TileObject>();
    public Dictionary<Vector2Int, TileObject> TempTileDictionary
    {
        get => _tempTileDictionary;
        set => _tempTileDictionary = value;
    }

    // 3.3. 현재 게임에 존재하는 모든 오브젝트들을 관리하는 dictionary.
    private Dictionary<Vector2Int, GameObject> _gameObjectDictionary = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, GameObject> GameObjectDictionary
    {
        get => _gameObjectDictionary;
        set => _gameObjectDictionary = value;
    }

    // Start is called before the first frame update
    public void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name.Length == 6)
        {
            stage = int.Parse(scene.name.Substring(5, 1));
        }
        else if (scene.name.Length == 7)
        {
            stage = int.Parse(scene.name.Substring(5, 1)) * 10 + int.Parse(scene.name.Substring(6, 1));
        }

        string path = "Stage" + stage.ToString();
        _stageData = StageDataLoader.Instance.LoadStageData(path);

        SetCameraPosition();

        TileInstantiate();

        pauseCanvas = Instantiate(pauseCanvasPrefab);
        pauseCanvas.SetActive(false);
        isPauseCanvasOpened = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPauseCanvasOpened)
            {
                pauseCanvas.SetActive(false);
                isPauseCanvasOpened = false;
                Time.timeScale = 1f;
            }
            else
            {
                pauseCanvas.SetActive(true);
                isPauseCanvasOpened = true;
                Time.timeScale = 0;
            }
        }
    }

    // 타일들을 모두 탐색해서, 가장 맵의 끝에 있는 타일의 x, y값을 찾아서 그 중간값을 카메라의 위치로 설정해준다.
    public void SetCameraPosition()
    {
        int minX = 99999999;
        int maxX = 0;
        int minY = 99999999;
        int maxY = 0;

        foreach (var tile in _stageData.TileDictionary)
        {
            if (tile.Value.Type != TileType.Wall)
            {
                // 가장 작은 x값과 가장 큰 x값, 가장 작은 y값과 가장 큰 y값을 찾아서 그 중간값을 카메라의 위치로 설정해주면 될 것 같아.
                // 그럼 카메라가 맵의 중간에 위치하게 될거야.

                if (tile.Key.x < minX)
                {
                    minX = tile.Key.x;
                }
                else if (tile.Key.x > maxX)
                {
                    maxX = tile.Key.x;
                }
                if (tile.Key.y < minY)
                {
                    minY = tile.Key.y;
                }
                else if (tile.Key.y > maxY)
                {
                    maxY = tile.Key.y;
                }
            }
        }

        Vector2 CameraPosition = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
        MainCamera.transform.position = new Vector3(CameraPosition.x, CameraPosition.y, -10.0f);
    }

    public void TileInstantiate()
    {
        // 원래는 한번만 가져오면 될 것 같은데 지금 알 수 없는 이유로 기존의 데이터가 훼손되어서... 일단 반복해서 가져오는걸로.
        string path = "Stage" + stage.ToString();
        _stageData = StageDataLoader.Instance.LoadStageData(path);

        // 타일들을 하나씩 만들어준다. 
        GameObject Tiles = new GameObject("Tiles");

        Debug.Log("(StageManager.cs) TileInstantiate 함수 실행됨.");

        _tempTileDictionary.Clear();

        // 게임을 리셋할때에 변경된 데이터들을 모두 제대로 되돌려준다. 
        foreach (var entry in _stageData.TileDictionary)
        {
            TileObject clonedTileObject = new TileObject(entry.Value); // 복사 생성자 사용
            _tempTileDictionary.Add(entry.Key, clonedTileObject);
        }

        foreach (var tile in _tempTileDictionary)
        {
            Vector3 tilePosition = new Vector3(tile.Key.x, tile.Key.y, 0);

            // -1인 경우는 벽. 이외의 경우에만 타일 만들어주면 된다. 
            if (tile.Value.Type != TileType.Wall)
            {
                GameObject newTile = Instantiate(TilePrefab, tilePosition, Quaternion.identity, Tiles.transform);
                newTile.GetComponent<SpriteRenderer>().sprite = SetTileSprite(tile.Value);

                if (tile.Value.Round == 1 || tile.Value.Round == 2)
                {
                    if (tile.Value.TileDirection == TileDirection.Up)
                    {
                        newTile.transform.Rotate(0, 0, -90);
                    }
                    else if (tile.Value.TileDirection == TileDirection.Down)
                    {
                        newTile.transform.Rotate(0, 0, 90);
                    }
                    else if (tile.Value.TileDirection == TileDirection.Right)
                    {
                        newTile.transform.Rotate(0, 0, 180);
                    }
                }
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

    public void DestroyAllObjectsAndTileInstantiate()
    {
        // 1. 모든 코루틴 스탑. 
        foreach (var coroutine in EnemyManager.Instance.EnemyDeathCoroutineQueue)
        {
            if (coroutine != null)
            {
                EnemyManager.Instance.StopCoroutine(coroutine);
            }
        }

        // 2 enemy 게임오브젝트들을 파괴. 
        foreach (var gameObjectWithVector in _gameObjectDictionary)
        {
            gameObjectWithVector.Value.SetActive(false);
            Destroy(gameObjectWithVector.Value);
        }

        _gameObjectDictionary.Clear();

        // 3. 캐릭터를 초기화해준다.
        Destroy(_character);

        TileInstantiate();
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

    // 타일의 스프라이트를 반환하는 함수. 둥근 모서리의 수, 패턴, 방향에 따라 다르게 반환해줘야 한다. 
    private Sprite SetTileSprite(TileObject tile)
    {
        if (tile.Round == 0 && tile.Pattern == 0 && tile.TileDirection == TileDirection.None)
        {
            return TileSprites[0];
        }
        else if (tile.Round == 0 && tile.Pattern == 1 && tile.TileDirection == TileDirection.None)
        {
            return TileSprites[1];
        }
        else if (tile.Round == 0 && tile.Pattern == 2 && tile.TileDirection == TileDirection.None)
        {
            return TileSprites[2];
        }
        else if (tile.Round == 1 && tile.Pattern == 0)
        {
            return TileSprites[3];
        }
        else if (tile.Round == 1 && tile.Pattern == 1)
        {
            return TileSprites[4];
        }
        else if (tile.Round == 1 && tile.Pattern == 2)
        {
            return TileSprites[5];
        }
        else if (tile.Round == 2 && tile.Pattern == 0)
        {
            return TileSprites[6];
        }
        else if (tile.Round == 2 && tile.Pattern == 1)
        {
            return TileSprites[7];
        }
        else if (tile.Round == 2 && tile.Pattern == 2)
        {
            return TileSprites[8];
        }

        return null;
    }

    //툴팁 활성화 함수
    public void ActivateTooltip()
    {
        if(toolTip != null)
        {
            Sequence sequenceActivateTooltip = DOTween.Sequence()
    .Append(toolTip.GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f))
    .Append(toolTip.GetComponent<Image>().DOColor(new Color(1.0f, 0.486f, 0.0f), 0.25f))
    .AppendInterval(0.2f)
    .Append(toolTip.GetComponent<Image>().DOColor(new Color(1.0f, 1.0f, 1.0f), 0.25f))
    .AppendInterval(0.2f)
    .Append(toolTip.GetComponent<Image>().DOColor(new Color(1.0f, 0.486f, 0.0f), 0.25f))
    .AppendInterval(0.2f)
    .Append(toolTip.GetComponent<Image>().DOColor(new Color(1.0f, 1.0f, 1.0f), 0.25f))
    .AppendInterval(0.2f)
    .Append(toolTip.GetComponent<Image>().DOColor(new Color(1.0f, 0.486f, 0.0f), 0.25f))
    .AppendInterval(0.2f)
    .Append(toolTip.GetComponent<Image>().DOColor(new Color(1.0f, 1.0f, 1.0f), 0.25f));
        }
    }
}
