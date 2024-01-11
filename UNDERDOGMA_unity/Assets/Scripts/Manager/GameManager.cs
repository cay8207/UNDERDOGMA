using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject Tile;
    [SerializeField] GameObject Enemy;

    Character character;

    // 타일들에 대한 정보가 담긴 dictionary. 어떤 칸에 어떤 물체가 있는지를 알려준다. 
    private Dictionary<Vector2Int, List<int>> _tileDictionary = new Dictionary<Vector2Int, List<int>>();

    public Dictionary<Vector2Int, List<int>> TileDictionary
    {
        get => _tileDictionary;
        set => _tileDictionary = value;
    }
    // 적에 대한 정보가 담긴 dictionary. 어떤 타일에 적이 있는지를 저장해두고, 플레이어가 적을 공격했을 때 이벤트를 보여주기 위해서. 
    private Dictionary<Vector2Int, GameObject> _enemyDictionary = new Dictionary<Vector2Int, GameObject>();
    public Dictionary<Vector2Int, GameObject> EnemyDictionary
    {
        get => _enemyDictionary;
        set => _enemyDictionary = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        _tileDictionary.Add(new Vector2Int(-2, 0), new List<int> { 0 });
        _tileDictionary.Add(new Vector2Int(-2, -1), new List<int> { 1, 2, 6 });
        _tileDictionary.Add(new Vector2Int(-1, 0), new List<int> { 0 });
        _tileDictionary.Add(new Vector2Int(-1, -1), new List<int> { 0 });
        _tileDictionary.Add(new Vector2Int(0, 0), new List<int> { 0 });
        _tileDictionary.Add(new Vector2Int(1, 0), new List<int> { 0 });
        _tileDictionary.Add(new Vector2Int(1, 1), new List<int> { 1, 3, 2 });

        _tileDictionary.Add(new Vector2Int(-3, 0), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(-3, -1), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(-2, 1), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(-2, -2), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(-1, 1), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(-1, -2), new List<int> { -1 });

        _tileDictionary.Add(new Vector2Int(0, 1), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(0, -1), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(1, 2), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(1, -1), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(2, 0), new List<int> { -1 });
        _tileDictionary.Add(new Vector2Int(2, 1), new List<int> { -1 });

        GameObject Tiles = new GameObject("Tiles");

        foreach(var tile in _tileDictionary)
        {
            Vector3 tilePosition = new Vector3(tile.Key.x, tile.Key.y, 0);
            GameObject newTile = Instantiate(Tile, tilePosition, Quaternion.identity, Tiles.transform);

            // -1은 벽을 의미함. 나중에는 다른 스프라이트를 넣어줄건데, 지금은 일단 까만색으로 설정. 
            if(tile.Value[0] == -1)
            {
                newTile.GetComponent<SpriteRenderer>().color = Color.black;
            }

            // 1은 적을 의미함. 해당 타일에 적을 만들어주고, 공격력과 생명력을 설정해준다. 
            if(tile.Value[0] == 1)
            {
                GameObject newEnemy = Instantiate(Enemy, tilePosition, Quaternion.identity);
                newEnemy.GetComponent<Enemy>().Attack = tile.Value[1];
                newEnemy.GetComponent<Enemy>().Heart = tile.Value[2];

                _enemyDictionary.Add(new Vector2Int(tile.Key.x, tile.Key.y), newEnemy);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
