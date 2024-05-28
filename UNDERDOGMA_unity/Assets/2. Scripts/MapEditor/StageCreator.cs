#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageCreator : MonoBehaviour
{
    public GridLayoutGroup grid;
    public GameObject mapEditorTile;
    public TMP_InputField inputField_X;
    public TMP_InputField inputField_Y;
    public Button createButton;
    public List<MapEditorTile> Tiles = new List<MapEditorTile>();
    public Dictionary<int, Dictionary<int, MapEditorTile>> TileDictionary = new Dictionary<int, Dictionary<int, MapEditorTile>>();
    [HideInInspector] public int ySize;
    [HideInInspector] public int xSize;

    void Start()
    {
        ySize = 0;
        xSize = 0;
        InitializeGrid();
    }

    //그리드 내부 초기화
    private void InitializeGrid()
    {
        foreach (Transform child in grid.gameObject.transform)
        {
            Destroy(child.gameObject);
        }
        Tiles.Clear();
        TileDictionary.Clear();
    }

    public void CreateButtonClick()
    {
        xSize = 0;
        ySize = 0;
        bool resultX = int.TryParse(inputField_X.text, out xSize);
        bool resultY = int.TryParse(inputField_Y.text, out ySize);
        if (resultX == false)
        {
            Debug.Log("X should be an INT value.");
            return;
        }
        if (resultY == false)
        {
            Debug.Log("Y should be an INT value.");
            return;
        }
        CreateStage(xSize, ySize);
    }

    public void CreateStage(int x, int y)
    {
        InitializeGrid();
        grid.constraintCount = x;

        for (int i = 0; i < x * y; i++)
        {
            int curX = i % x;
            int curY = i / x;
            GameObject temp = Instantiate(mapEditorTile);
            MapEditorTile tile = temp.GetComponent<MapEditorTile>();
            temp.transform.SetParent(grid.gameObject.transform);
            tile.X = curX;
            tile.Y = curY;
            Tiles.Add(tile);
            if (!TileDictionary.ContainsKey(curX))
            {
                TileDictionary.Add(curX, new Dictionary<int, MapEditorTile>());
            }
            TileDictionary[curX][curY] = tile;
            tile.SetTileType(MapEditorTile.TileType.Wall);
        }
    }
}
#endif