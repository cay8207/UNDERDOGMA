using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button exitButton;

    //[SerializeField] Image StartButtonText;
    //[SerializeField] Image CreditsButtonText;
    //[SerializeField] Image ExitButtonText;

    [SerializeField] GameObject ButtonSelectHighlight;

    private int _selectedIndex;

    void Awake()
    {
        Managers.Instance.Init();
        GameManager.Instance.Init();

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
                HighlightButton();
            }
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_selectedIndex < 2)
            {
                _selectedIndex++;
                //ButtonSelectHighlight.transform.DOMove(ButtonSelectHighlight.transform.position - new Vector3(0, 100, 0), 0.2f);
                HighlightButton();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (_selectedIndex == 0)
            {
                startButton.GetComponent<Buttons>().GoToTutorial();
            }
            else if (_selectedIndex == 1)
            {
                creditsButton.GetComponent<Buttons>().GoToCredit();
            }
            else if (_selectedIndex == 2)
            {
                exitButton.GetComponent<Buttons>().ExitGame();
            }
            Debug.Log("Audio Start!");
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.UI_Toggle);
        }
    }

    public void selectStartButton()
    {
        _selectedIndex = 0;
        HighlightButton();
    }
    public void selectCreditsButton()
    {
        _selectedIndex = 1;
        HighlightButton();
    }
    public void selectExitButton()
    {
        _selectedIndex = 2;
        HighlightButton();
    }

    void HighlightButton()
    {
        if (_selectedIndex == 0)
        {
            ButtonSelectHighlight.transform.DOMove(startButton.transform.position, 0.2f);
            startButton.GetComponent<Buttons>().MakeButtonBigger();
            creditsButton.GetComponent<Buttons>().MakeButtonOriginalSize();
            exitButton.GetComponent<Buttons>().MakeButtonOriginalSize();

        }
        else if (_selectedIndex == 1)
        {
            ButtonSelectHighlight.transform.DOMove(creditsButton.transform.position, 0.2f);
            startButton.GetComponent<Buttons>().MakeButtonOriginalSize();
            creditsButton.GetComponent<Buttons>().MakeButtonBigger();
            exitButton.GetComponent<Buttons>().MakeButtonOriginalSize();
        }
        else if (_selectedIndex == 2)
        {
            ButtonSelectHighlight.transform.DOMove(exitButton.transform.position, 0.2f);
            startButton.GetComponent<Buttons>().MakeButtonOriginalSize();
            creditsButton.GetComponent<Buttons>().MakeButtonOriginalSize();
            exitButton.GetComponent<Buttons>().MakeButtonBigger();
        }
    }
}