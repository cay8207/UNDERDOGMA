using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // 싱글톤 패턴. 인스턴스를 하나만 만들어서 사용한다. 
    private static DialogueManager _instance;

    public static DialogueManager Instance
    {
        get => _instance;
        set => _instance = value;
    }

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

    private List<string> dialogueList;

    //  현재 몇번째 텍스트를 읽고 있는지 저장하기 위한 변수.
    private int count;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        SetActiveImages(false);
    }

    public void Start()
    {

    }

    // 데이터를 가져오고, 만약 현재 스테이지에 다이얼로그가 존재하는 경우 다이얼로그를 출력한다.
    public void Init(DialogueEvent dialogueEvent, Language language, int world, int stage)
    {
        _isDialogueRunning = true;

        dialogueList = GameManager.Instance.DialogueDataTable.GetDialogueData(dialogueEvent, language, world, stage);

        foreach (var dialogue in dialogueList)
        {
            Debug.Log(dialogue);
        }

        // 현재 다이얼로그의 위치를 가리키는 카운트를 0으로 초기화한다. 
        count = 0;

        // 만약 다이얼로그가 존재하는 경우, 다이얼로그를 출력한다.
        if (dialogueList.Count > 0)
        {
            SetActiveImages(true);
            StartCoroutine(StartDialogueCoroutine());
        }
        // 다이얼로그가 존재하지 않으며, 시작 다이얼로그인 경우, 다이얼로그를 종료하고 툴팁을 활성화한다.
        else if (dialogueList.Count == 0 && dialogueEvent == DialogueEvent.Start)
        {
            _isDialogueRunning = false;
            StageManager.Instance.ActivateTooltip();
        }
        // 다이얼로그가 존재하지 않으며, 엔딩 다이얼로그인 경우 다이얼로그를 종료한다. 
        else if (dialogueList.Count == 0 && dialogueEvent == DialogueEvent.Ending)
        {
            _isDialogueRunning = false;
        }
    }

    private void Update()
    {
        if (dialogueList.Count > 0)
        {
            // 대화를 읽는다.
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                && count < dialogueList.Count && _isDialogueTextRunning == false)
            {
                // 모든 대화를 읽은 경우
                if (count == dialogueList.Count - 1)
                {
                    count++;
                    // 모든 코루틴 종료
                    StopAllCoroutines();
                    ExitDialogue();
                    _isDialogueRunning = false;
                }
                else
                {
                    Debug.Log("count: " + count + " / " + "dialogueList.Count: " + dialogueList.Count);
                    count++;
                    // 아닌 경우 다음 다이얼로그 출력.
                    StopAllCoroutines();
                    StartCoroutine(StartDialogueCoroutine());
                }
            }
        }

    }

    // 다이얼로그를 하나씩 출력하는 코루틴. 
    public IEnumerator StartDialogueCoroutine()
    {
        // 1. 다이얼로그가 출력되는 동안 캐릭터의 움직임을 멈춰준다.
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
        for (int i = 0; i < dialogueList[count].Length; i++)
        {
            DialogueText.text += dialogueList[count][i]; // 한글자씩 출력
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.5f); //텍스트 바로 안넘어가게
        _isDialogueTextRunning = false;
    }

    public void ExitDialogue()
    {
        count = 0;
        DialogueText.text = "";

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