using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;

public class SaveSystem : Singleton<SaveSystem>
{
    public MapData mapData;
    public StageCreator stageCreator;
    public TMP_InputField inputField_ExecutionCount;
    public TMP_InputField inputField_CharacterHeart;

    [ContextMenu("SaveMapData")]
    void SaveMapDataToJson()
    {
        mapData.TileDictionary = new Dictionary<string, JObject>();
        List<MapEditorTile> mapEditorTiles = stageCreator.Tiles;
        foreach(var tile in mapEditorTiles)
        {
            mapData.TileDictionary.Add($"{tile.X}, {tile.Y}", tile.ToJSON());
        }
        for(int x = -1; x <= stageCreator.ColSize; x++)
        {
            for(int y = -1; y <= stageCreator.RowSize; y++)
            {
                if(x == -1 || x == stageCreator.ColSize || y == -1 || y == stageCreator.RowSize)
                {
                    var jsonTypeWall = new JObject();
                    jsonTypeWall.Add("Type", "Wall");
                    mapData.TileDictionary.Add($"{x}, {y}", jsonTypeWall);
                }
            }
        }
        bool resultEC = int.TryParse(inputField_ExecutionCount.text, out mapData.ExecutionCount);
        bool resultCH = int.TryParse(inputField_CharacterHeart.text, out mapData.CharacterHeart);
        if (resultEC == false)
        {
            Debug.Log("Execution Count에 정수값을 입력하세요.");
            return;
        }
        if (resultCH == false)
        {
            Debug.Log("Character Heart에 정수값을 입력하세요.");
            return;
        }
        string jsonData = JsonConvert.SerializeObject(mapData, Formatting.Indented);
        string path = Application.streamingAssetsPath + "/Data/Stage/" + "testStage.json";
        File.WriteAllText(path, jsonData);
    }

    [ContextMenu("LoadMapData")]
    void LoadMapDataFromJson()
    {
        string path = Application.streamingAssetsPath + "/Data/Stage/" + "Stage2.json";
        string jsonData = File.ReadAllText(path);
        mapData = JsonConvert.DeserializeObject<MapData>(jsonData);
    }

    public void SetCharacterCoord(int Row, int Col)
    {
        mapData.CharacterRow = Row;
        mapData.CharacterCol = Col;
    }
}

[System.Serializable]
public class MapData
{
    public Dictionary<string, JObject> TileDictionary = new Dictionary<string, JObject>();
    [HideInInspector] public int ExecutionCount;
    [HideInInspector] public int CharacterHeart;
    [HideInInspector] public int CharacterRow;
    [HideInInspector] public int CharacterCol;
}