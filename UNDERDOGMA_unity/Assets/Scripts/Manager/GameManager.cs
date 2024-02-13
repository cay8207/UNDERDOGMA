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
    [SerializeField] GameObject ExecutionManagerPrefab;
    [SerializeField] GameObject DialogueManagerPrefab;

    StageManager _stageManager;
    Execution _executionManager;
    DialogueManager _dialogueManager;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
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

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }
}
