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
    [SerializeField] public int stage;
    [SerializeField] GameObject ResetAnimationUpSide;
    [SerializeField] GameObject ResetAnimationDownSide;


    // 위의 변수와 다르게 _character는 생성된 게임오브젝트를 저장하기 위한 변수. 
    public GameObject _character;

    public StageData _stageData;

    // 적에 대한 정보가 담긴 dictionary. 어떤 타일에 적이 있는지를 저장해두고, 플레이어가 적을 공격했을 때 이벤트를 보여주기 위해서. 
    private Dictionary<Vector2Int, GameObject> _enemyDictionary = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, GameObject> EnemyDictionary
    {
        get => _enemyDictionary;
        set => _enemyDictionary = value;
    }

    public Dictionary<Vector2Int, GameObject> _meatDictionary = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, GameObject> MeatDictionary
    {
        get => _meatDictionary;
        set => _meatDictionary = value;
    }

    // Start is called before the first frame update
    public void Awake()
    {
        string path = "Stage" + stage.ToString();
        _stageData = StageDataLoader.Instance.LoadStageData(path);

        // Dialogue dialogue = new Dialogue();
        // dialogue.Init();
        // DialogueManager.Instance.ShowDialogue(dialogue);

        TileInstantiate();

        AudioManager.Instance.Init();
        AudioManager.Instance.PlayBgm(true);
    }

    public void TileInstantiate()
    {
        // 타일들을 하나씩 만들어준다. 
        GameObject Tiles = new GameObject("Tiles");

        foreach (var tile in _stageData.TileDictionary)
        {
            Vector3 tilePosition = new Vector3(tile.Key.x, tile.Key.y, 0);

            // -1인 경우는 벽. 이외의 경우에만 타일 만들어주면 된다. 
            if (tile.Value[0] >= 0)
            {
                GameObject newTile = Instantiate(TilePrefab, tilePosition, Quaternion.identity, Tiles.transform);
            }

            // 1은 적을 의미함. 해당 타일에 적을 만들어주고, 공격력과 생명력을 설정해준다. 
            if (tile.Value[0] == 1)
            {
                GameObject newEnemy = null;

                // 적의 타입에 따라 다른 적을 생성해준다. 일단 급해서 이렇게 짜두기는 했는데,
                // 나중에 enemyManager쪽으로 넣어서 깔끔하게 리팩토링 할 예정. 
                if (tile.Value[1] == 0)
                {
                    newEnemy = Instantiate(NormalEnemyPrefab, tilePosition + new Vector3(-0.06f, 0.3f, 0.0f), Quaternion.identity);
                    newEnemy.GetComponent<NormalEnemy>()._attackDirection = tile.Value[5];
                    newEnemy.GetComponent<NormalEnemy>().Attack = tile.Value[3];
                    newEnemy.GetComponent<NormalEnemy>().Heart = tile.Value[4];
                    newEnemy.GetComponent<NormalEnemy>().Row = tile.Key.x;
                    newEnemy.GetComponent<NormalEnemy>().Col = tile.Key.y;
                }

                _enemyDictionary.Add(new Vector2Int(tile.Key.x, tile.Key.y), newEnemy);
            }

            if (tile.Value[0] == 2)
            {
                GameObject newMeat = Instantiate(MeatPrefab, tilePosition, Quaternion.identity);
                newMeat.GetComponent<Meat>().Heart = tile.Value[2];

                _meatDictionary.Add(new Vector2Int(tile.Key.x, tile.Key.y), newMeat);
            }
        }

        // 캐릭터 오브젝트를 생성하고 초기화해준다. 
        _character = Instantiate(CharacterPrefab, new Vector3(-0.07f, 0.35f, 0.0f), Quaternion.identity);
        _character.GetComponent<Character>().Init(_stageData.CharacterRow, _stageData.CharacterCol, _stageData.CharacterHeart);
        MainCamera.GetComponent<MainCamera>()._character = _character;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        ResetAnimation();

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Reset);

        // 맵에 존재하는 타일을 제외한 오브젝트들을 초기화시켜준다.
        // 1. 만약 처형이 진행중이라면 처형을 멈추고, 처형에 관한 변수들을 초기화시켜준다.
        ExecutionManager.Instance.ExecutionStop();

        for (int i = 0; i < ExecutionManager.Instance.ExecutionCount; i++)
        {
            ExecutionManager.Instance.ExecutionCountObjectList[i].GetComponent<Image>().sprite = ExecutionManager.Instance.CloseEye;
            ExecutionManager.Instance.ExecutionCountObjectList[i].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(69.0f, 17.0f);
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

        // enemy 게임오브젝트들을 파괴. 
        foreach (var enemyWithVector in _enemyDictionary)
        {
            enemyWithVector.Value.SetActive(false);
            Destroy(enemyWithVector.Value);
        }
        _enemyDictionary.Clear();

        // 3. 고기(체력 회복)들을 모두 초기화해준다. 
        foreach (var meatWithVector in _meatDictionary)
        {
            Destroy(meatWithVector.Value);
        }
        _meatDictionary.Clear();

        // 4. 캐릭터를 초기화해준다.
        Destroy(_character);

        // 5. 적과 고기들을 새롭게 생성해준다. 
        foreach (var tile in _stageData.TileDictionary)
        {
            Vector3 tilePosition = new Vector3(tile.Key.x, tile.Key.y, 0);

            // 리셋 코드이니 타일은 그대로 두고 적, 고기만 다시 생성해준다. 
            if (tile.Value[0] == 1)
            {
                GameObject newEnemy = null;

                // 적의 타입에 따라 다른 적을 생성해준다. 일단 급해서 이렇게 짜두기는 했는데,
                // 나중에 enemyManager쪽으로 넣어서 깔끔하게 리팩토링 할 예정. 
                if (tile.Value[1] == 0)
                {
                    newEnemy = Instantiate(NormalEnemyPrefab, tilePosition + new Vector3(-0.06f, 0.3f, 0.0f), Quaternion.identity);

                    newEnemy.GetComponent<NormalEnemy>()._attackDirection = tile.Value[5];
                    newEnemy.GetComponent<NormalEnemy>().Attack = tile.Value[3];
                    newEnemy.GetComponent<NormalEnemy>().Heart = tile.Value[4];
                    newEnemy.GetComponent<NormalEnemy>().Row = tile.Key.x;
                    newEnemy.GetComponent<NormalEnemy>().Col = tile.Key.y;
                }

                _stageData.TileDictionary[tile.Key][2] = 1;

                _enemyDictionary.Add(new Vector2Int(tile.Key.x, tile.Key.y), newEnemy);
            }

            if (tile.Value[0] == 2)
            {
                GameObject newMeat = Instantiate(MeatPrefab, tilePosition, Quaternion.identity);
                newMeat.GetComponent<Meat>().Heart = tile.Value[2];

                _stageData.TileDictionary[tile.Key][1] = 1;

                _meatDictionary.Add(new Vector2Int(tile.Key.x, tile.Key.y), newMeat);
            }
        }

        // 캐릭터 오브젝트를 생성하고 초기화해준다. 
        _character = Instantiate(CharacterPrefab, new Vector3(-0.07f, 0.35f, 0.0f), Quaternion.identity);
        _character.GetComponent<Character>().Init(_stageData.CharacterRow, _stageData.CharacterCol, _stageData.CharacterHeart);
        MainCamera.GetComponent<MainCamera>()._character = _character;
    }

    public void ResetAnimation()
    {
        Sequence ResetUpSideSequence = DOTween.Sequence();
        Sequence ResetDownSideSequence = DOTween.Sequence();

        ResetUpSideSequence
            .Append(
                ResetAnimationUpSide.GetComponent<RectTransform>()
                .DOLocalMove(new Vector3(0.0f, 333.0f, 0.0f), 0.5f, false)
                .SetEase(Ease.InQuart))
            .AppendInterval(1.0f)
            .Append(
                ResetAnimationUpSide.GetComponent<RectTransform>()
                .DOLocalMove(new Vector3(0.0f, 1100.0f, 0.0f), 1.0f, false));
        ResetDownSideSequence
            .Append(
                ResetAnimationDownSide.GetComponent<RectTransform>()
                .DOLocalMove(new Vector3(0.0f, -333.0f, 0.0f), 0.5f, false)
                .SetEase(Ease.InQuart))
            .AppendInterval(1.0f)
            .Append(
                ResetAnimationDownSide.GetComponent<RectTransform>()
                .DOLocalMove(new Vector3(0.0f, -1100.0f, 0.0f), 1.0f, false));
    }

    public void StageClearCheck()
    {
        Debug.Log("EnemyDictionary.Count: " + EnemyDictionary.Count);

        if (EnemyDictionary.Count == 0)
        {
            StartCoroutine(StageClear());
        }
    }

    public IEnumerator StageClear()
    {
        Debug.Log("StageClear");

        // 승리 애니메이션 추가되면 수정할 예정. 
        // _character.GetComponent<Animator>().SetBool("StageClear", true);

        // yield return new WaitForSeconds(2.0f);

        // _character.GetComponent<Animator>().SetBool("StageClear", false);

        yield return new WaitForSeconds(2.0f);

        if (stage == 3)
        {
            SceneManager.LoadScene("Ending");
        }
        else
        {
            SceneManager.LoadScene("Stage" + (stage + 1).ToString());
        }
    }
}
