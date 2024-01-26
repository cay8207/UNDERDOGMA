using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageData
{
    // 타일들에 대한 정보가 담긴 dictionary. 어떤 칸에 어떤 물체가 있는지를 알려준다. 
    // 적이 있는 칸은 1, 하트는 2, 덫은 3으로 저장. 
    // 적이 있는 칸에는 리스트로 몇가지를 추가해둔다. {1, 0, 1, 2, 6}과 같은 방식이면 
    // 1: 적이 있다. 0: 0번 타입의 적이다(일반 적). 1: 살아있다. 2: 공격력. 6: 생명력. 
    // 적의 타입은 0: 일반 적, 1: 감시자, 2: 추격자 등등... 나중에 버그 안 나도록 구조를 조금 바꿔야 할 듯. 
    [JsonIgnore]
    private Dictionary<Vector2Int, List<int>> _tileDictionary = new Dictionary<Vector2Int, List<int>>();
    public Dictionary<Vector2Int, List<int>> TileDictionary
    {
        get => _tileDictionary;
        set => _tileDictionary = value;
    }

    [JsonIgnore]
    // 처형 시스템에 대한 정보를 저장한다. 몇 번 이동하면 처형 이벤트가 작동하는지, 체력이 얼마 이상이어야 하는지. 
    private int _executionCount;
    public int ExecutionCount
    {
        get => _executionCount;
        set => _executionCount = value;
    }

    [JsonIgnore]
    private int _executionHealth;
    public int ExecutionHealth
    {
        get => _executionHealth;
        set => _executionHealth = value;
    }

    [JsonIgnore]
    private int _characterRow;
    public int CharacterRow
    {
        get => _characterRow;
        set => _characterRow = value;
    }

    [JsonIgnore]
    private int _characterCol;
    public int CharacterCol
    {
        get => _characterCol;
        set => _characterCol = value;
    }

    [JsonIgnore]
    private int _characterHeart;
    public int CharacterHeart
    {
        get => _characterHeart;
        set => _characterHeart = value;
    }

    // 데이터를 초기화하는 생성자
    public StageData(Dictionary<string, List<int>> tileDictionary, int executionCount, int executionHealth,
    int characterRow, int characterCol, int characterHeart)
    {
        foreach (var tile in tileDictionary)
        {
            int tilex = Int32.Parse(tile.Key.Split(',')[0]);
            int tiley = Int32.Parse(tile.Key.Split(',')[1]);
            _tileDictionary.Add(new Vector2Int(tilex, tiley), tile.Value);
        }
        this._executionCount = executionCount;
        this._executionHealth = executionHealth;
        this._characterRow = characterRow;
        this._characterCol = characterCol;
        this._characterHeart = characterHeart;
    }
}

public class StageDataLoader : Singleton<StageDataLoader>
{
    public void SaveStageData(StageData holder, string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/Stage/{dataName}.json";

        var converter = new StringEnumConverter();
        var pDataStringSave = JsonConvert.SerializeObject(holder, converter);
        File.WriteAllText(path, pDataStringSave);
    }

    public StageData LoadStageData(string dataName)
    {
        string path = Application.streamingAssetsPath;
        path += $"/Data/Stage/{dataName}.json";

        var converter = new StringEnumConverter();
        var pDataStringLoad = File.ReadAllText(path);
        StageData playerData = JsonConvert.DeserializeObject<StageData>(pDataStringLoad, converter);

        return playerData;
    }
}
