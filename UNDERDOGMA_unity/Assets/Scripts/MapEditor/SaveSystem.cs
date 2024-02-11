using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveSystem : MonoBehaviour
{
   public MapData mapData;

    [ContextMenu("SaveMapData")]
    void SaveMapDataToJson()
    {
        string jsonData = JsonConvert.SerializeObject(mapData,Formatting.Indented);
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
}

[System.Serializable]
public class MapData
{
    public Dictionary<string, List<int>> TileDictionary = new Dictionary<string, List<int>>();
    public int ExecutionCount;
    public int ExecutionHealth;
    public int CharacterRow;
    public int CharacterCol;
    public int CharacterHeart;
}