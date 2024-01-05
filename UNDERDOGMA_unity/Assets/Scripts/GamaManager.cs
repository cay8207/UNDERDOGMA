using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamaManager : MonoBehaviour
{
    [SerializeField] GameObject Tile;

    // 타일들의 위치, 타일 위에 무엇이 있는지 관리하기 위한 딕셔너리. 
    // -1은 벽, 0은 아무것도 없는 상황, 1은 적, 2는 덫, 3은 하트. (0, 0), 3 과 같은 방식이면 (0, 0)에 하트가 있는 것. 
    private Dictionary<Vector2Int, int> tileDictionary = new Dictionary<Vector2Int, int>();
    // Start is called before the first frame update
    void Start()
    {
        tileDictionary.Add(new Vector2Int(-2, 0), 0);
        tileDictionary.Add(new Vector2Int(-2, -1), 0);
        tileDictionary.Add(new Vector2Int(-1, 0), 0);
        tileDictionary.Add(new Vector2Int(-1, -1), 0);
        tileDictionary.Add(new Vector2Int(0, 0), 0);
        tileDictionary.Add(new Vector2Int(0, -1), 0);
        tileDictionary.Add(new Vector2Int(1, 0), 0);
        tileDictionary.Add(new Vector2Int(1, 1), 0);
        tileDictionary.Add(new Vector2Int(2, 0), 0);
        tileDictionary.Add(new Vector2Int(2, 1), 0);

        tileDictionary.Add(new Vector2Int(-3, 0), -1);
        tileDictionary.Add(new Vector2Int(-3, -1), -1);
        tileDictionary.Add(new Vector2Int(-2, 1), -1);
        tileDictionary.Add(new Vector2Int(-2, -2), -1);
        tileDictionary.Add(new Vector2Int(-1, 1), -1);
        tileDictionary.Add(new Vector2Int(-1, -2), -1);
        tileDictionary.Add(new Vector2Int(0, 1), -1);
        tileDictionary.Add(new Vector2Int(0, -2), -1);
        tileDictionary.Add(new Vector2Int(1, 2), -1);
        tileDictionary.Add(new Vector2Int(1, -1),  -1);
        tileDictionary.Add(new Vector2Int(2, 2), -1);
        tileDictionary.Add(new Vector2Int(2, -1), -1);
        tileDictionary.Add(new Vector2Int(3, 1), -1);
        tileDictionary.Add(new Vector2Int(3, 0), -1);


        GameObject Tiles = new GameObject("Tiles");

        foreach(var tile in tileDictionary)
        {
            Vector3 tilePosition = new Vector3(tile.Key.x, tile.Key.y, 0);
            GameObject newTile = Instantiate(Tile, tilePosition, Quaternion.identity, Tiles.transform);

            if(tile.Value == -1)
            {
                newTile.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }

    // direction 1은 위, 2는 아래, 3은 왼쪽, 4는 오른쪽. 
    void CharacterMove(int row, int col, int direction)
    {
        int idx;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
