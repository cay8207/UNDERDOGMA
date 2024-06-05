using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class WorldMapManager : MonoBehaviour
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static WorldMapManager _instance;

    public static WorldMapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (WorldMapManager)FindObjectOfType(typeof(WorldMapManager));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(WorldMapManager)} (Singleton)");
                    _instance = singletonObject.AddComponent<WorldMapManager>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }
    #endregion

    public GameObject WorldMapInfo;
    public List<GameObject> WorldMapInfos = new List<GameObject>();
    public List<WorldMapImage> WorldMapImages = new List<WorldMapImage>();
    public int SelectedWorld;

    [SerializeField]
    private int WorldMapInfoSpacing;

    private void Start()
    {
        SelectedWorld = 1;
        SetInfoPos(SelectedWorld);
        for (int i = 1; i <= 5; i++)
        {
            if (i == SelectedWorld)
            {
                WorldMapImages[i - 1].InitializeWorldImage(true);
            }
            else
            {
                WorldMapImages[i - 1].InitializeWorldImage(false);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectedWorld = Math.Max(1, SelectedWorld - 1);
            SetInfoPos(SelectedWorld);
            SetWorldMapImage(SelectedWorld);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectedWorld = Math.Min(5, SelectedWorld + 1);
            SetInfoPos(SelectedWorld);
            SetWorldMapImage(SelectedWorld);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadSelectedWorld();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(WorldMapImages[SelectedWorld - 1].transform.localScale);
        }
    }

    public void SetInfoPos(int world)
    {
        WorldMapInfo.transform.DOMoveY((world - 1) * WorldMapInfoSpacing * (-1), 0.5f, false).SetEase(Ease.OutCubic);
    }
    public void SetWorldMapImage(int world)
    {
        WorldMapImage CurrentWorldMapImage;
        for (int i = 1; i <= 5; i++)
        {
            CurrentWorldMapImage = WorldMapImages[i - 1];
            if (i == world)
            {
                CurrentWorldMapImage.SelectWorld(true);
            }
            else
            {
                CurrentWorldMapImage.SelectWorld(false);
            }
        }
    }
    public void LoadSelectedWorld()
    {
        if (WorldMapImages[SelectedWorld - 1].Unlocked)
        {
            SceneManager.LoadScene(WorldMapImages[SelectedWorld - 1].WorldScene);
        }
        else
        {
            Debug.Log("World locked!");
        }
    }
}