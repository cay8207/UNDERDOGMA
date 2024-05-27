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
    // 적이 있는 칸에는 리스트로 몇가지를 추가해둔다. {1, 0, 1, 2, 6, 3}과 같은 방식이면 
    // 1: 적이 있다. 0: 0번 타입의 적이다(일반 적). 1: 살아있다. 2: 공격력. 6: 생명력, 3: 공격방향(우) 
    // 적의 타입은 0: 일반 적, 1: 감시자, 2: 추격자 등등... 나중에 버그 안 나도록 구조를 조금 바꿔야 할 듯. 
    // {1, 1, 1, 3, 2, 2}라면  1: 적이 있다. 1: 1번 타입의 적이다(감시자). 1: 살아있다. 3: 공격력. 2: 생명력, 2: 공격방향(좌)
    [JsonIgnore]
    private Dictionary<Vector2Int, TileObject> _tileDictionary = new Dictionary<Vector2Int, TileObject>();
    public Dictionary<Vector2Int, TileObject> TileDictionary => _tileDictionary;

    [JsonIgnore]
    // 처형 시스템에 대한 정보를 저장한다. 몇 번 이동하면 처형 이벤트가 작동하는지, 체력이 얼마 이상이어야 하는지. 
    private int _executionCount;
    public int ExecutionCount
    {
        get => _executionCount;
        set => _executionCount = value;
    }

    [JsonIgnore]
    private int _characterHeart;
    public int CharacterHeart
    {
        get => _characterHeart;
        set => _characterHeart = value;
    }

    [JsonIgnore]
    private int _characterX;
    public int CharacterX
    {
        get => _characterX;
        set => _characterX = value;
    }

    [JsonIgnore]
    private int _characterY;
    public int CharacterY
    {
        get => _characterY;
        set => _characterY = value;
    }

    [JsonIgnore]
    private int _stageXSize;
    public int StageXSize
    {
        get => _stageXSize;
        set => _stageXSize = value;
    }

    [JsonIgnore]
    private int _stageYSize;
    public int StageYSize
    {
        get => _stageYSize;
        set => _stageYSize = value;
    }

    // 데이터를 초기화하는 생성자
    public StageData(Dictionary<string, Dictionary<string, string>> tileDictionary, int executionCount, int characterHeart,
    int characterX, int characterY, int stageXSize, int StageYSize)
    {
        foreach (var tile in tileDictionary)
        {
            tile.Key.Replace(" ", "");
            int tilex = Int32.Parse(tile.Key.Split(',')[0]);
            int tiley = Int32.Parse(tile.Key.Split(',')[1]);

            // Stage 데이터에 Type이 없을 경우 데이터를 잘못 입력한 것. 에러를 출력한다. 
            if (tile.Value.ContainsKey("Type") == false)
            {
                Debug.Log("Stage 데이터에 Type이 없습니다. \"-2, -1\": {\"Type\": \"Wall\"}와 같은 형식으로 입력해주세요.");
                continue;
            }

            switch (tile.Value["Type"])
            {
                case "Wall":
                    _tileDictionary
                        .Add(
                            new Vector2Int(tilex, tiley),
                                new TileObject(TileType.Wall));
                    break;
                case "Empty":
                    _tileDictionary
                        .Add(new(tilex, tiley),
                                new TileObject(TileType.Empty));
                    break;
                case "Enemy":
                    if (tile.Value.ContainsKey("EnemyType") == false
                        || tile.Value.ContainsKey("IsAlive") == false
                        || tile.Value.ContainsKey("Heart") == false
                        || tile.Value.ContainsKey("SpriteDirection") == false)
                    {
                        Debug.Log("Enemy 데이터가 잘못 입력되었습니다. EnemyType, IsAlive, Heart, SpriteDirection 중 빠진 게 없는지 체크해주세요.");
                        continue;
                    }
                    _tileDictionary
                        .Add(new(tilex, tiley),
                                new TileObject(TileType.Enemy,
                                                createEnemyData(tile.Value)));
                    break;
                case "Meat":
                    if (tile.Value.ContainsKey("Amount") == false
                        != tile.Value.ContainsKey("IsExist") == false)
                    {
                        Debug.Log("Meat 데이터가 잘못 입력되었습니다. Amount, IsExist 중 빠진 게 없는지 체크해주세요.");
                        continue;
                    }
                    _tileDictionary
                        .Add(new(tilex, tiley),
                                new TileObject(TileType.Meat,
                                                createMeatData(tile.Value)));
                    break;
                case "Ball":
                    _tileDictionary
                        .Add(new(tilex, tiley),
                                new TileObject(TileType.Ball));
                    break;

            }
        }
        this._executionCount = executionCount;
        this._characterHeart = characterHeart;
        this._characterX = characterX;
        this._characterY = characterY;
        this._stageXSize = stageXSize;
        this._stageYSize = StageYSize;
    }

    public EnemyData createEnemyData(Dictionary<String, String> data)
    {
        EnemyData enemyData;

        EnemyType enemyType = new EnemyType();
        bool isAlive = true;
        int heart;
        SpriteDirection spriteDirection = new SpriteDirection();

        switch (data["EnemyType"])
        {
            case "NormalEnemy":
                enemyType = EnemyType.NormalEnemy;
                break;
            case "Chaser":
                enemyType = EnemyType.Chaser;
                break;
            case "MiniBoss":
                enemyType = EnemyType.MiniBoss;
                break;
            case "StrongAttack":
                enemyType = EnemyType.StrongAttack;
                break;
            case "AllDirectionsAttack":
                enemyType = EnemyType.AllDirectionsAttack;
                break;
            case "Angel":
                enemyType = EnemyType.Angel;
                break;
            case "KickBoss":
                enemyType = EnemyType.KickBoss;
                break;

        }

        switch (data["IsAlive"])
        {
            case "true":
                isAlive = true;
                break;
            case "false":
                isAlive = false;
                break;
        }

        heart = Int32.Parse(data["Heart"]);

        switch (data["SpriteDirection"])
        {
            case "Up":
                spriteDirection = SpriteDirection.Up;
                break;
            case "Down":
                spriteDirection = SpriteDirection.Down;
                break;
            case "Left":
                spriteDirection = SpriteDirection.Left;
                break;
            case "Right":
                spriteDirection = SpriteDirection.Right;
                break;
        }

        enemyData = new EnemyData(enemyType, isAlive, heart, spriteDirection);

        return enemyData;
    }

    public MeatData createMeatData(Dictionary<String, String> data)
    {
        MeatData meatData;

        int amount = Int32.Parse(data["Amount"]);
        bool isExist = true;

        switch (data["IsExist"])
        {
            case "true":
                isExist = true;
                break;
            case "false":
                isExist = false;
                break;
        }

        meatData = new MeatData(amount, isExist);

        return meatData;
    }

    public TileType returnTileType(String tileType)
    {
        switch (tileType)
        {
            case "Wall":
                return TileType.Wall;
            case "Empty":
                return TileType.Empty;
            case "Enemy":
                return TileType.Enemy;
            case "Meat":
                return TileType.Meat;
            case "Ball":
                return TileType.Ball;
            default:
                return TileType.Invalid;
        }
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
