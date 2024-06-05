using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WorldSelectManager : MonoBehaviour
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static WorldSelectManager _instance;

    public static WorldSelectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (WorldSelectManager)FindObjectOfType(typeof(WorldSelectManager));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(WorldSelectManager)} (Singleton)");
                    _instance = singletonObject.AddComponent<WorldSelectManager>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }
    #endregion

    [SerializeField] Button ReturnButton;
    [SerializeField] Button TitleButton;
    [SerializeField] Button WorldEnterButton;
    [SerializeField] Button LeftButton;
    [SerializeField] Button RightButton;

    [SerializeField] List<GameObject> WorldImages;
    [SerializeField] List<Sprite> WorldIcons;


    public int _selectedIndex = 0;

    public float InitialLeftButtonX;
    public float InitialRightButtonX;


    public void Start()
    {
        ReturnButton.onClick.AddListener(ReturnToWorld);
        TitleButton.onClick.AddListener(ReturnToTitle);
        WorldEnterButton.onClick.AddListener(EnterWorld);

        LeftButton.onClick.AddListener(IndexMinus);
        RightButton.onClick.AddListener(IndexPlus);

        InitialLeftButtonX = LeftButton.transform.position.x;
        InitialRightButtonX = RightButton.transform.position.x;

        Debug.Log("InitialLeftButtonX: " + InitialLeftButtonX);
        Debug.Log("InitialRightButtonX: " + InitialRightButtonX);

        LeftButton.transform.DOMoveX(InitialLeftButtonX - 10, 0.5f).SetLoops(-1, LoopType.Yoyo);
        RightButton.transform.DOMoveX(InitialRightButtonX + 10, 0.5f).SetLoops(-1, LoopType.Yoyo);

        SetWorldImages();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            IndexMinus();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            IndexPlus();
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            EnterWorld();
        }
    }

    public void IndexMinus()
    {
        if (_selectedIndex > 0)
        {
            _selectedIndex--;
            SetWorldImages();
        }

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.UI_Toggle);
    }

    public void IndexPlus()
    {
        if (_selectedIndex < 4)
        {
            _selectedIndex++;
            SetWorldImages();
        }

        AudioManager.Instance.PlaySfx(AudioManager.Sfx.UI_Toggle);
    }

    public void SetWorldImages()
    {
        for (int i = 0; i < WorldImages.Count; i++)
        {
            WorldImages[i].transform.DOLocalMoveX((i - _selectedIndex) * Screen.width, 0.5f).SetEase(Ease.InOutCubic);
        }
    }

    public void ReturnToWorld()
    {
        SceneManager.LoadScene("World" + GameManager.Instance.World.ToString());
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void EnterWorld()
    {
        if (_selectedIndex == 0)
        {
            GameManager.Instance.World = 1;
            SceneManager.LoadScene("World1");
        }
        else if (_selectedIndex == 1)
        {
            GameManager.Instance.World = 2;
            SceneManager.LoadScene("World2");
        }
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.UI_Toggle);
    }
}
