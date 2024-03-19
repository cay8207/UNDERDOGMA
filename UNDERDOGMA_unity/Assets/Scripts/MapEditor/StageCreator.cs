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
    [HideInInspector] public int RowSize;
    [HideInInspector] public int ColSize;

    void Start()
    {
        RowSize = 0;
        ColSize = 0;
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
    }

    public void CreateButtonClick()
    {
        ColSize= 0;
        RowSize = 0;
        bool resultCol = int.TryParse(inputField_X.text, out ColSize);
        bool resultRow = int.TryParse(inputField_Y.text, out RowSize);
        if (resultCol == false)
        {
            Debug.Log("X에 정수값을 입력하세요.");
            return;
        }
        if (resultRow == false)
        {
            Debug.Log("Y에 정수값을 입력하세요.");
            return;
        }
        CreateStage(ColSize, RowSize);
    }

    private void CreateStage(int col, int row)
    {
        InitializeGrid();
        grid.constraintCount = col;

        for (int i = 0; i < col * row; i++)
        {
            GameObject temp = Instantiate(mapEditorTile);
            temp.transform.SetParent(grid.gameObject.transform);
            temp.GetComponent<MapEditorTile>().X = i % col;
            temp.GetComponent<MapEditorTile>().Y = i / col;
            Tiles.Add(temp.GetComponent<MapEditorTile>());
        }
    }
}