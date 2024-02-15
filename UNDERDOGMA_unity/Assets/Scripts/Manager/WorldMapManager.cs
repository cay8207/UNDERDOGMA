using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class WorldMapManager : Singleton<WorldMapManager>
{
    public GameObject WorldMapInfo;
    public List<GameObject> WorldMapInfos = new List<GameObject>();
    public List<WorldMapImage> WorldMapImages = new List<WorldMapImage>();
    public int SelectedWorld;

    [SerializeField]
    private int WorldMapInfoSpacing;
    private Vector3 infoPos;

    private void Start()
    {
        //WorldMapInfo�� �ڵ����� �޾ƿ��� ����� �����Ƽ� �� ����
        SelectedWorld = 2;
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
    }

    public void SetInfoPos(int world)
    {
        WorldMapInfo.transform.DOMoveY((world - 1) * WorldMapInfoSpacing, 0.5f, false).SetEase(Ease.OutCubic);
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
        Debug.Log(SelectedWorld);
    }
}
