using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public class Buttons : MonoBehaviour
{
    private Vector3 OriginalScale;
    public float SizeMultiplier;
    public Ease ease;
    private void Start()
    {
        OriginalScale = this.GetComponent<RectTransform>().localScale;
    }
    public void ReturnToWorldMap()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WorldMap");
    }
    public void MakeButtonBigger()
    {
        this.GetComponent<RectTransform>().DOScale(OriginalScale * SizeMultiplier, 0.5f).SetEase(ease);
    }
    public void MakeButtonOriginalSize()
    {
        this.GetComponent<RectTransform>().DOScale(OriginalScale, 0.5f).SetEase(ease);
    }
    public void ReturnToStageMap()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("World" + (StageManager.Instance.stage <= 16 ? 1 : 2).ToString());
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void ReturnToMainScreen()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameStart");
    }

    public void GoToCredit()
    {
        SceneManager.LoadScene("Credit");
    }

    public void GoToTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
