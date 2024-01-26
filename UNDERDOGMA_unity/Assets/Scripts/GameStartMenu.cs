using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;

    [SerializeField] Image StartButtonText;
    [SerializeField] Image CreditsButtonText;
    [SerializeField] Image ExitButtonText;

    [SerializeField] GameObject ButtonSelectHighlight;

    private int _selectedIndex;

    void Awake()
    {
        Managers.Instance.Init();
        GameManager.Instance.Init();

        // 각 버튼에 클릭 리스너 등록
        startButton.onClick.AddListener(StartGame);
        creditsButton.onClick.AddListener(ShowCredits);
        exitButton.onClick.AddListener(ExitGame);

        AudioManager.Instance.Init();
        AudioManager.Instance.PlayBgm(true);

        _selectedIndex = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_selectedIndex > 0)
            {
                _selectedIndex--;
                ButtonSelectHighlight.GetComponent<Transform>().transform.position += new Vector3(0, 100, 0);
                HighlightButton();
            }
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_selectedIndex < 2)
            {
                _selectedIndex++;
                ButtonSelectHighlight.GetComponent<Transform>().transform.position -= new Vector3(0, 100, 0);
                HighlightButton();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (_selectedIndex == 0)
            {
                StartGame();
            }
            else if (_selectedIndex == 1)
            {
                ShowCredits();
            }
            else if (_selectedIndex == 2)
            {
                ExitGame();
            }
            Debug.Log("Audio Start!");
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.UI_Toggle);
        }
    }

    void HighlightButton()
    {
        if (_selectedIndex == 0)
        {
            StartButtonText.rectTransform.sizeDelta = new Vector2(330.0f, 40.0f);
            CreditsButtonText.rectTransform.sizeDelta = new Vector2(150.0f, 32.0f);
            ExitButtonText.rectTransform.sizeDelta = new Vector2(90.0f, 32.0f);
        }
        else if (_selectedIndex == 1)
        {
            StartButtonText.rectTransform.sizeDelta = new Vector2(260.0f, 32.0f);
            CreditsButtonText.rectTransform.sizeDelta = new Vector2(200.0f, 40.0f);
            ExitButtonText.rectTransform.sizeDelta = new Vector2(90.0f, 32.0f);
        }
        else if (_selectedIndex == 2)
        {
            StartButtonText.rectTransform.sizeDelta = new Vector2(260.0f, 32.0f);
            CreditsButtonText.rectTransform.sizeDelta = new Vector2(150.0f, 32.0f);
            ExitButtonText.rectTransform.sizeDelta = new Vector2(120.0f, 40.0f);
        }
    }

    // '게임 시작' 버튼 클릭 시 호출되는 함수
    void StartGame()
    {
        SceneManager.LoadScene("Tutorial");
        Debug.Log("Stage1 Scene Loaded");
    }

    // '크레딧' 버튼 클릭 시 호출되는 함수
    void ShowCredits()
    {
        // 원하는 Scene으로 이동
        SceneManager.LoadScene("Credit");
        AudioManager.Instance.PlayBgm(true);
    }

    // '게임 종료' 버튼 클릭 시 호출되는 함수
    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}