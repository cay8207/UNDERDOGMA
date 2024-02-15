using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class World1BossClear : MonoBehaviour
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static TutorialManager _instance;

    public static TutorialManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (TutorialManager)FindObjectOfType(typeof(TutorialManager));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(TutorialManager)} (Singleton)");
                    _instance = singletonObject.AddComponent<TutorialManager>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }

    #endregion

    // 현재 텍스트를 담아두는 변수. 
    [SerializeField] private Canvas DialogueCanvas;
    [SerializeField] private UnityEngine.UI.Image BlackScreen;
    [SerializeField] private UnityEngine.UI.Image DialogueBackGround;
    [SerializeField] private UnityEngine.UI.Image DialogueImage1;
    [SerializeField] private Vector3 DialogueImage1Position;
    [SerializeField] private UnityEngine.UI.Image DialogueImage2;
    [SerializeField] private Vector3 DialogueImage2Position;
    [SerializeField] private UnityEngine.UI.Image DialogueWindow;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI DialogueText;

    // 대사와 함께 나올 스탠딩 cg들을 저장하는 배열. 
    [SerializeField] private List<String> Names;
    [SerializeField] private List<Sprite> listSprites1;
    [SerializeField] private List<Sprite> listSprites2;


    public bool _isDialogueRunning = false;
    public bool _isDialogueTextRunning = false;

    private DialogueData _dialogueData;

    //  현재 몇번째 텍스트를 읽고 있는지 저장하기 위한 변수.
    private int count = -1;

    public void Start()
    {
        string path = "World1BossClear";
        _dialogueData = DialogueDataLoader.Instance.LoadDialogueData(path);

        SetActiveImages(true);
        StartCoroutine(StartDialogueCoroutine());
    }

    private void Update()
    {
        // 대화를 읽는다.
        // 대화를 읽는다.
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            && count < _dialogueData.DialogueList.Count && _isDialogueTextRunning == false)
        {
            // 모든 대화를 읽은 경우
            if (count == _dialogueData.DialogueList.Count - 1)
            {
                // 모든 코루틴 종료
                StopAllCoroutines();
                ExitDialogue();
                _isDialogueRunning = false;
            }
            // 0인 상태에서 스페이스바를 눌렀을 경우 
            else if (count == 0)
            {
                count++;
                StopAllCoroutines();
                StartCoroutine(BlackScreenFadeOutCoroutine());
            }
            else if (count == 1)
            {
                count++;
                StopAllCoroutines();
                StartCoroutine(BlackScreenFadeInCoroutine());
            }
            else
            {
                count++;
                // 아닌 경우 다음 다이얼로그 출력.
                StopAllCoroutines();
                StartCoroutine(StartDialogueCoroutine());
            }
        }
    }

    public IEnumerator StartDialogueCoroutine()
    {
        _isDialogueTextRunning = true;

        if (count == -1)
        {
            yield return new WaitForSeconds(2.0f);

            DialogueWindow.GetComponent<UnityEngine.UI.Image>().enabled = true;
            DialogueText.GetComponent<TextMeshProUGUI>().enabled = true;
            Name.GetComponent<TextMeshProUGUI>().enabled = true;

            count++;
        }

        Name.text = Names[count];

        DialogueText.text = "";

        DialogueImage1.rectTransform.localPosition = DialogueImage1Position;
        DialogueImage2.rectTransform.localPosition = DialogueImage2Position;

        DialogueImage1.rectTransform.sizeDelta = listSprites1[count].rect.size;
        DialogueImage2.rectTransform.sizeDelta = listSprites2[count].rect.size;

        DialogueImage1.sprite = listSprites1[count];
        DialogueImage2.sprite = listSprites2[count];


        // 텍스트 출력 //
        for (int i = 0; i < _dialogueData.DialogueList[count].Length; i++)
        {
            DialogueText.text += _dialogueData.DialogueList[count][i]; // 한글자씩 출력
            yield return new WaitForSeconds(0.01f);
        }

        _isDialogueTextRunning = false;
    }

    public IEnumerator BlackScreenFadeOutCoroutine()
    {
        _isDialogueTextRunning = true;

        BlackScreen.DOFade(0.0f, 2.0f);
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(StartDialogueCoroutine());

        _isDialogueTextRunning = false;
    }

    public IEnumerator BlackScreenFadeInCoroutine()
    {
        _isDialogueTextRunning = true;

        BlackScreen.DOFade(1.0f, 2.0f);
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(StartDialogueCoroutine());

        _isDialogueTextRunning = false;
    }

    public void ExitDialogue()
    {
        count = 0;
        DialogueText.text = "";

        _dialogueData.DialogueList.Clear();
        listSprites1.Clear();
        listSprites2.Clear();

        DialogueImage1.GetComponent<UnityEngine.UI.Image>().enabled = false;
        Name.GetComponent<TextMeshProUGUI>().enabled = false;
        DialogueText.GetComponent<TextMeshProUGUI>().enabled = false;
        DialogueWindow.GetComponent<UnityEngine.UI.Image>().enabled = false;
        DialogueBackGround.GetComponent<UnityEngine.UI.Image>().enabled = false;

        SceneManager.LoadScene("WorldMap");
    }

    public void SetActiveImages(bool isActive)
    {
        Name.GetComponent<TextMeshProUGUI>().enabled = isActive;
        DialogueText.GetComponent<TextMeshProUGUI>().enabled = isActive;
        DialogueWindow.GetComponent<UnityEngine.UI.Image>().enabled = isActive;
        DialogueBackGround.GetComponent<UnityEngine.UI.Image>().enabled = isActive;

        DialogueImage1.GetComponent<Image>().enabled = isActive;
        DialogueImage2.GetComponent<Image>().enabled = isActive;
    }
}
