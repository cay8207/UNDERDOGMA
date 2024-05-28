using System;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class StageSelectorManager : MonoBehaviour
{
    public TMP_InputField inputField_StageNum;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayStage();
        }
    }

    public void PlayStage()
    {
        int stage = int.Parse(inputField_StageNum.text);

        string path = Application.streamingAssetsPath + "/Data/Stage/Stage" + stage + ".json";

        if (File.Exists(path))
        {
            GameManager.Instance.FromStageSelector = true;
            LoadingManager.Instance.LoadStage(stage / 100, stage % 100);
        }
        else
        {
            Debug.LogError("There is no file.");
            return;
        }
    }
}
