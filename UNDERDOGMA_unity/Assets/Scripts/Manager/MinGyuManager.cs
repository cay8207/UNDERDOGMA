using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MinGyuManager : MonoBehaviour
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static MinGyuManager _instance;

    public static MinGyuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (MinGyuManager)FindObjectOfType(typeof(MinGyuManager));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(MinGyuManager)} (Singleton)");
                    _instance = singletonObject.AddComponent<MinGyuManager>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }
    #endregion

    [SerializeField] GameObject World1;
    [SerializeField] GameObject World2;
    [SerializeField] GameObject World3;
    [SerializeField] GameObject World4;
    [SerializeField] GameObject World5;

    [SerializeField] GameObject Worlds;

    [SerializeField] Sprite World1select;
    [SerializeField] Sprite World1unselect;
    [SerializeField] Sprite World2select;
    [SerializeField] Sprite World2unselect;


    public int _selectedIndex = 0;


    public void Start()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_selectedIndex > 0)
            {
                _selectedIndex--;
            }
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_selectedIndex < 4)
            {
                _selectedIndex++;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (_selectedIndex == 0)
            {
                SceneManager.LoadScene("World1");
            }
            else if (_selectedIndex == 1)
            {
                SceneManager.LoadScene("World2");
            }
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.UI_Toggle);
        }

        HighlightButton(_selectedIndex);
    }

    public void OnClick()
    {

    }

    public void HighlightButton(int index)
    {
        switch (index)
        {
            case 0:
                World1.GetComponent<RectTransform>().DOScale(1.2f, 0.0f);
                World1.GetComponent<Image>().sprite = World1select;
                World2.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World2.GetComponent<Image>().sprite = World2unselect;
                World3.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World4.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World5.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                Worlds.transform.DOLocalMove(new Vector3(0, 0, 0), 0.2f);
                break;
            case 1:
                World1.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World1.GetComponent<Image>().sprite = World1unselect;
                World2.GetComponent<RectTransform>().DOScale(1.2f, 0.0f);
                World2.GetComponent<Image>().sprite = World2select;
                World3.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World4.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World5.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                Worlds.transform.DOLocalMove(new Vector3(0, -960.0f, 0), 0.2f);
                break;
            case 2:
                World1.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World1.GetComponent<Image>().sprite = World1unselect;
                World2.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World2.GetComponent<Image>().sprite = World2unselect;
                World3.GetComponent<RectTransform>().DOScale(1.2f, 0.0f);
                World4.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World5.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                Worlds.transform.DOLocalMove(new Vector3(0, -1920.0f, 0), 0.2f);
                break;
            case 3:
                World1.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World1.GetComponent<Image>().sprite = World1unselect;
                World2.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World2.GetComponent<Image>().sprite = World2unselect;
                World3.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World4.GetComponent<RectTransform>().DOScale(1.2f, 0.0f);
                World5.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                Worlds.transform.DOLocalMove(new Vector3(0, -2880.0f, 0), 0.2f);
                break;
            case 4:
                World1.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World1.GetComponent<Image>().sprite = World1unselect;
                World2.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World2.GetComponent<Image>().sprite = World2unselect;
                World3.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World4.GetComponent<RectTransform>().DOScale(1.0f, 0.0f);
                World5.GetComponent<RectTransform>().DOScale(1.2f, 0.0f);
                Worlds.transform.DOLocalMove(new Vector3(0, -3840.0f, 0), 0.2f);
                break;
        }
    }
}
