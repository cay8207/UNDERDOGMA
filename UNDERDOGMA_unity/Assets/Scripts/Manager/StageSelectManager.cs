using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StageIcon;

public class StageSelectManager : Singleton<StageSelectManager>
{
    public StageIcon SelectedStage;
    private StageIcon nextSelectedStage;
    void Start()
    {
        SelectedStage.IsSelected = true;
        nextSelectedStage = SelectedStage;
        SelectedStage.SelectWorld(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            nextSelectedStage = SelectedStage.GetStageIcon(StageIcon.StageDirection.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            nextSelectedStage = SelectedStage.GetStageIcon(StageIcon.StageDirection.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            nextSelectedStage = SelectedStage.GetStageIcon(StageIcon.StageDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            nextSelectedStage = SelectedStage.GetStageIcon(StageIcon.StageDirection.Right);
        }

        if(nextSelectedStage != SelectedStage)
        {
            SelectedStage.SelectWorld(false);
            SelectedStage = nextSelectedStage;
            SelectedStage.SelectWorld(true);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadSelectedStage();
        }
    }

    public void LoadSelectedStage()
    {
        Debug.Log("Load");
    }
}
