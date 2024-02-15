using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static StageIcon;

public class StageSelectManager : Singleton<StageSelectManager>
{
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

        if(NextSelectedStage != SelectedStage)
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
