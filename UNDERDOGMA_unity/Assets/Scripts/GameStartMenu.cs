using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;
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
        }
    }

    void HighlightButton()
    {
        if (_selectedIndex == 0)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 40;
            creditsButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 32;
            exitButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 32;
        }
        else if (_selectedIndex == 1)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 32;
            creditsButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 40;
            exitButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 32;
        }
        else if (_selectedIndex == 2)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 32;
            creditsButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 32;
            exitButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 40;
        }
    }

    // '게임 시작' 버튼 클릭 시 호출되는 함수
    void StartGame()
    {
        // 원하는 Scene으로 이동
        SceneManager.LoadScene("Stage1");
        Debug.Log("Stage1 Scene Loaded");
    }

    // '크레딧' 버튼 클릭 시 호출되는 함수
    void ShowCredits()
    {
        // 원하는 Scene으로 이동
        SceneManager.LoadScene("Credit");
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