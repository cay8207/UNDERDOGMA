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

    public static DialogueManager Instance => _instance;

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
    private int count = 0;

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

        _isDialogueRunning = true;

        SetActiveImages(false);
    }

    public void Start()
    {

    }

    public void Init(int world, int stage)
    {
        dialogueList = GameManager.Instance.DialogueDataTable.GetDialogueData(GameManager.Instance.Language, world, stage);

        foreach (var dialogue in dialogueList)
        {
            Debug.Log(dialogue);
        }

        if (GameManager.Instance.Stage == 1 || GameManager.Instance.Stage == 4
                || GameManager.Instance.Stage == 10 || GameManager.Instance.Stage == 11)
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
        if (GameManager.Instance.Stage == 1 || GameManager.Instance.Stage == 4
                || GameManager.Instance.Stage == 10 || GameManager.Instance.Stage == 11)
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

        // 리스트 초기화
        dialogueList.Clear();
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