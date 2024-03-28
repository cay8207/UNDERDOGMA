using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;

public class SaveSystem : Singleton<SaveSystem>
{
    private StageData stageData;
    public StageCreator stageCreator;
    public TMP_InputField inputField_ExecutionCount;
    public TMP_InputField inputField_CharacterHeart;
    public TMP_InputField inputField_StageNum;

    [ContextMenu("SaveMapData")]
    public void SaveMapDataToJson()
    {
        // 1. StageData 객체를 생성하고, 
        stageData = new StageData();
        stageData.TileDictionary = new Dictionary<string, JObject>();
        List<MapEditorTile> mapEditorTiles = stageCreator.Tiles;

        // 
        foreach (var tile in mapEditorTiles)
        {
            stageData.TileDictionary.Add($"{tile.X}, {tile.Y}", tile.ToJSON());
        }

        if (stageData.CharacterX == -1 || stageData.CharacterY == -1)
        {
            Debug.LogWarning("Player Tile Not Set");
        }

        for (int x = -1; x <= stageCreator.xSize; x++)
        {
            for (int y = -1; y <= stageCreator.ySize; y++)
            {
                if (x == -1 || x == stageCreator.xSize || y == -1 || y == stageCreator.ySize)
                {
                    var jsonTypeWall = new JObject();
                    jsonTypeWall.Add("Type", "Wall");
                    stageData.TileDictionary.Add($"{x}, {y}", jsonTypeWall);
                }
            }
        }

        bool resultEC = int.TryParse(inputField_ExecutionCount.text, out stageData.ExecutionCount);
        bool resultCH = int.TryParse(inputField_CharacterHeart.text, out stageData.CharacterHeart);
        if (resultEC == false)
        {
            Debug.Log("Execution Count�� �������� �Է��ϼ���.");
            return;
        }

        if (resultCH == false)
        {
            Debug.Log("Character Heart�� �������� �Է��ϼ���.");
            return;
        }
        bool resultSN = int.TryParse(inputField_StageNum.text, out int stageNum);

        if (resultSN == false)
        {
            Debug.Log("StageNum�� �������� �Է��ϼ���.");
            return;
        }
        stageData.StageXSize = stageCreator.xSize;
        stageData.StageYSize = stageCreator.ySize;
        string jsonData = JsonConvert.SerializeObject(stageData, Formatting.Indented);
        string path = Application.streamingAssetsPath + "/Data/Stage/Stage" + stageNum + ".json";
        File.WriteAllText(path, jsonData);
        Debug.Log("Stage" + stageNum + ".json�� ����Ǿ����ϴ�!");
    }

    [ContextMenu("LoadMapData")]
    public void LoadMapDataFromJson()
    {
        bool resultSN = int.TryParse(inputField_StageNum.text, out int stageNum);

        if (resultSN == false)
        {
            Debug.Log("StageNum�� �������� �Է��ϼ���.");
            return;
        }

        string path = Application.streamingAssetsPath + "/Data/Stage/Stage" + stageNum + ".json";
        if (!File.Exists(path))
        {
            Debug.LogError(stageNum + ".json ������ �������� �ʽ��ϴ�!");
            return;
        }

        string jsonData = File.ReadAllText(path);
        stageData = JsonConvert.DeserializeObject<StageData>(jsonData);
        stageCreator.ySize = stageData.StageYSize;
        stageCreator.xSize = stageData.StageXSize;
        stageCreator.CreateStage(stageData.StageXSize, stageData.StageYSize);

        for (int x = 0; x < stageCreator.xSize; x++)
        {
            for (int y = 0; y < stageCreator.ySize; y++)
            {
                string key = $"{x}, {y}";
                stageCreator.TileDictionary[x][y].FromJSON(stageData.TileDictionary[key]);
            }
        }

        if (stageData.CharacterX == -1 || stageData.CharacterY == -1)
        {
            Debug.LogWarning("Player Tile Not Set");
        }
        else
        {
            stageCreator.TileDictionary[stageData.CharacterX][stageData.CharacterY].SetTileType(MapEditorTile.TileType.Player);
        }

        inputField_ExecutionCount.text = stageData.ExecutionCount.ToString();
        inputField_CharacterHeart.text = stageData.CharacterHeart.ToString();
        stageCreator.inputField_X.text = stageData.StageXSize.ToString();
        stageCreator.inputField_Y.text = stageData.StageYSize.ToString();
    }

    public void SetCharacterCoord(int X, int Y)
    {
        stageData.CharacterX = X;
        stageData.CharacterY = Y;
    }

    [System.Serializable]
    private class StageData
    {
        public Dictionary<string, JObject> TileDictionary = new Dictionary<string, JObject>();
        [HideInInspector] public int ExecutionCount;
        [HideInInspector] public int CharacterHeart;
        [HideInInspector] public int CharacterX = -1;
        [HideInInspector] public int CharacterY = -1;
        [HideInInspector] public int StageXSize;
        [HideInInspector] public int StageYSize;
    }
}

