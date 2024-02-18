using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static DialogueManager _instance;

    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (DialogueManager)FindObjectOfType(typeof(DialogueManager));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(DialogueManager)} (Singleton)");
                    _instance = singletonObject.AddComponent<DialogueManager>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }

    #endregion

    // 현재 텍스트를 담아두는 변수. 
    [SerializeField] private Canvas DialogueCanvas;
    [SerializeField] private UnityEngine.UI.Image DialogueBackGround;

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI DialogueText;
    [SerializeField] private UnityEngine.UI.Image DialogueWindow;
    [SerializeField] private UnityEngine.UI.Image DialogueImage1;
    [SerializeField] private Vector3 DialogueImage1Position;
    [SerializeField] private UnityEngine.UI.Image DialogueImage2;
    [SerializeField] private Vector3 DialogueImage2Position;

    // 대사와 함께 나올 스탠딩 cg들을 저장하는 배열. 
    [SerializeField] private List<String> Names;
    [SerializeField] private List<Sprite> listSprites1;
    [SerializeField] private List<Sprite> listSprites2;

    public bool _isDialogueRunning = false;
    public bool _isDialogueTextRunning = false;

    private DialogueData _dialogueData;

    public DialogueData DialogueData
    {
        get => _dialogueData;
        set => _dialogueData = value;
    }

    //  현재 몇번째 텍스트를 읽고 있는지 저장하기 위한 변수.
    private int count = 0;

    public void Awake()
    {
        _isDialogueRunning = true;

        SetActiveImages(false);
    }

    public void Start()
    {
        string path = "Stage" + StageManager.Instance.stage.ToString();
        _dialogueData = DialogueDataLoader.Instance.LoadDialogueData(path);

        if (StageManager.Instance.stage == 1 || StageManager.Instance.stage == 4
                || StageManager.Instance.stage == 10 || StageManager.Instance.stage == 11)
        {
            SetActiveImages(true);
            StartCoroutine(StartDialogueCoroutine());
        }
        else
        {
            _isDialogueRunning = false;
            StageManager.Instance.ActivateTooltip();
        }
    }

    private void Update()
    {
        if (StageManager.Instance.stage == 1 || StageManager.Instance.stage == 4
                || StageManager.Instance.stage == 10 || StageManager.Instance.stage == 11)
        {
            // 대화를 읽는다.
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                && count < _dialogueData.DialogueList.Count && _isDialogueTextRunning == false)
            {
                // 모든 대화를 읽은 경우
                if (count == _dialogueData.DialogueList.Count - 1)
                {
                    count++;
                    // 모든 코루틴 종료
                    StopAllCoroutines();
                    ExitDialogue();
                    _isDialogueRunning = false;
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

    }

    public IEnumerator StartDialogueCoroutine()
    {
        _isDialogueTextRunning = true;

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

    public void ExitDialogue()
    {
        count = 0;
        DialogueText.text = "";

        // 리스트 초기화
        _dialogueData.DialogueList.Clear();
        listSprites1.Clear();
        listSprites2.Clear();
        // 애니메이터 초기화

        SetActiveImages(false);
        StageManager.Instance.ActivateTooltip();
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