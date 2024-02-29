using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StageIcon;

public class StageSelectManager : MonoBehaviour
{
    #region Singleton
    // 싱글톤 패턴.
    // 싱글톤 클래스를 구현해두긴 했지만, stageManager와 executionMangaer, DialogueManager는 
    // dontdestroyonload가 필요없기 때문에 클래스 내부에 싱글톤 패턴을 간단히 구현.
    private static StageSelectManager _instance;

    public static StageSelectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (StageSelectManager)FindObjectOfType(typeof(StageSelectManager));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject($"{typeof(StageSelectManager)} (Singleton)");
                    _instance = singletonObject.AddComponent<StageSelectManager>();
                    singletonObject.transform.parent = null;
                }
            }

            return _instance;
        }
    }
    #endregion
    public StageIcon SelectedStage;
    public StageIcon NextSelectedStage;
    void Start()
    {
        SelectedStage.IsSelected = true;
        NextSelectedStage = SelectedStage;
        SelectedStage.SelectWorld(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            NextSelectedStage = SelectedStage.GetStageIcon(StageIcon.StageDirection.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            NextSelectedStage = SelectedStage.GetStageIcon(StageIcon.StageDirection.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            NextSelectedStage = SelectedStage.GetStageIcon(StageIcon.StageDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextSelectedStage = SelectedStage.GetStageIcon(StageIcon.StageDirection.Right);
        }

        if (NextSelectedStage != SelectedStage)
        {
            SwitchSelectedStage();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadSelectedStage();
        }
    }
    public void SwitchSelectedStage()
    {
        SelectedStage.SelectWorld(false);
        SelectedStage = NextSelectedStage;
        SelectedStage.SelectWorld(true);
    }
    public void LoadSelectedStage()
    {
        SceneManager.LoadScene(SelectedStage.StageScene);
    }
}
