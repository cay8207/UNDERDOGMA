using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
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
    [SerializeField] private TextMeshProUGUI DialogueText;
    [SerializeField] private UnityEngine.UI.Image DialogueWindow;

    public List<string> listSentences = new List<string>();
    public List<Sprite> listSprites = new List<Sprite>();
    public List<Sprite> listWindows = new List<Sprite>();

    //  현재 몇번째 텍스트를 읽고 있는지 저장하기 위한 변수.
    private int count;

    public void Start()
    {

    }

    private void Update()
    {
        // 대화를 읽는다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            count++;
            // 모든 대화를 읽은 경우
            if (count == listSentences.Count)
            {
                // 모든 코루틴 종료
                StopAllCoroutines();
                ExitDialogue();
            }
            else
            {
                // 아닌 경우 다음 다이얼로그 출력.
                StopAllCoroutines();
                StartCoroutine(StartDialogueCoroutine());
            }
        }
    }

    // 다이어로그 시작 //
    // 나중에는 스테이지마다 다른 다이어로그를 보여주기 위해서, 매개변수로 스테이지를 받아올 수 있도록 구현할 예정.
    public void ShowDialogue(Dialogue dialogue)
    {

        // 리스트 채우기 //
        for (int i = 0; i < dialogue.sentences.Count; ++i)
        {
            listSentences.Add(dialogue.sentences[i]);
            // listSprites.Add(dialogue.sprites[i]);
            // listWindows.Add(dialogue.dialogueWindow[i]);
        }

        // 다이어로그를 보여주는 코루틴 실행
        StartCoroutine(StartDialogueCoroutine());
    }

    public IEnumerator StartDialogueCoroutine()
    {
        DialogueText.text = "";

        // 텍스트 출력 //
        for (int i = 0; i < listSentences[count].Length; i++)
        {
            DialogueText.text += listSentences[count][i]; // 한글자씩 출력
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void ExitDialogue()
    {
        //// 초기화 ////
        count = 0;
        DialogueText.text = "";
        // 리스트 초기화 //
        listSentences.Clear();
        listWindows.Clear();
        listSprites.Clear();
        // 애니메이터 초기화 //

        DialogueText.GetComponent<TextMeshProUGUI>().enabled = false;
        DialogueWindow.GetComponent<UnityEngine.UI.Image>().enabled = false;
    }
}