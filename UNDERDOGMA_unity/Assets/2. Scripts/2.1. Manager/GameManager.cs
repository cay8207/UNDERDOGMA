using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

// 게임 매니저에서는 전반적으로 게임 진행과 관련된 것들을 컨트롤한다.
// 최대한 핵심적인 부분만 담고, 나머지는 모두 다른 클래스에서 담당하도록 한다.
public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject StageManagerPrefab;
    [SerializeField] GameObject ExecutionPrefab;
    [SerializeField] GameObject DialogueManagerPrefab;

    private int world = 1;
    public int World
    {
        get => world;
        set => world = value;
    }

    private int stage = 1;
    public int Stage
    {
        get => stage;
        set => stage = value;
    }

    private Language _language;
    public Language Language
    {
        get => _language;
        set => _language = value;
    }

    private DialogueDataTable _dialogueDataTable;
    public DialogueDataTable DialogueDataTable
    {
        get => _dialogueDataTable;
        set => _dialogueDataTable = value;
    }

    public bool FromStageSelector = false;
    public bool FromMapEditor = false;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // 기본 언어는 한국어로 설정해준다.
        _language = Language.Korean;

        // 다이얼로그 테이블을 불러온다.
        _dialogueDataTable = new DialogueDataTable("DialogueDataTable");
        _dialogueDataTable.LoadCsv("언더독 다이얼로그 및 번역 테이블 - 다이얼로그 시트.csv");
    }
    // Start is called before the first frame update
    void Start()
    {
        SetResolution();
    }

    public void Init()
    {

    }

    // 스테이지를 시작하면 stageManager, executionManager, dialogueManager를 생성한다.
    public void StageInit()
    {

    }

    /* 해상도 설정하는 함수 */
    public void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        // 목표 해상도 비율을 계산합니다.
        float targetAspect = (float)setWidth / setHeight;
        // 현재 기기의 해상도 비율을 계산합니다.
        float deviceAspect = (float)Screen.width / Screen.height;

        // 목표 해상도에 맞추어 스크린 해상도를 설정합니다.
        Screen.SetResolution(setWidth, setHeight, true);

        Debug.Log("ScreenWidth: " + Screen.width + " " + "ScreenHeight: " + Screen.height);
        Debug.Log("targetAspect: " + targetAspect + " " + "deviceAspect: " + deviceAspect);

        // 카메라 뷰포트를 조정하여 올바른 종횡비를 유지합니다.
        if (targetAspect > deviceAspect)
        {
            float scaleHeight = targetAspect / deviceAspect;
            Camera.main.rect = new Rect(0f, (1f - scaleHeight) / 2f, 1f, scaleHeight);
        }
        else
        {
            float scaleWidth = deviceAspect / targetAspect;
            Camera.main.rect = new Rect((1f - scaleWidth) / 2f, 0f, scaleWidth, 1f);
        }
    }
}
