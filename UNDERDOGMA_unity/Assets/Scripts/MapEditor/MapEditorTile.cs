using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class MapEditorTile : MonoBehaviour
{
    public enum TileType
    {
        None, Empty, Player, Enemy, Meat
    }

    public TileType CurrentTileType;
    public enum EnemyDirection
    {
        Up = 0, Down = 1, Left = 2, Right = 3
    }
    public EnemyDirection CurrentEnemyDirection;

    [System.Serializable]
    public class TileSprites
    {
        public Sprite None;
        public Sprite Empty;
        public Sprite Player;
        public Sprite Enemy;
        public Sprite Meat;
    }

    [Header ("Coordinate")]
    [SerializeField]
    int x;
    [SerializeField]
    int y;

    [Header("Sprites")]
    [SerializeField]
    TileSprites tileSprites;

    [Header("Settings")]
    [SerializeField]
    GameObject enemyUI;
    [SerializeField]
    GameObject meatUI;

    private Image tileImage;

    private int enemyHP = 0;
    private int enemyAtk = 0;
    private int meatHP = 0;

    private MapEditor mapEditor;

    private void Start()
    {
        tileImage = GetComponent<Image>();
        SetTileType(TileType.None);
    }

    private Sprite GetCurrentSprite()
    {
        switch (CurrentTileType)
        {
            case TileType.None:
                return tileSprites.None;
            case TileType.Empty:
                return tileSprites.Empty;
            case TileType.Player:
                return tileSprites.Player;
            case TileType.Enemy:
                return tileSprites.Enemy;
            case TileType.Meat:
                return tileSprites.Meat;
            default:
                return null;
        }
    }

    public int X
    {
        get { return x; }
        set { x = value; }
    }
    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    public int EnemyHP
    {
        get { return enemyHP; }
        set { enemyHP = value; }
    }
    public int EnemyAtk
    {
        get { return enemyAtk; }
        set { enemyHP = value; }
    }
    public int MeatHP
    {
        get { return meatHP; }
        set { meatHP = value; }
    }

    public void SelectTile()
    {
        mapEditor =
            (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
        mapEditor.setTile(this);
    }

    public void SetTileType(TileType tileType)
    {
        CurrentTileType = tileType;
        tileImage.sprite = GetCurrentSprite();
        switch (CurrentTileType)
        {
            case TileType.Enemy:
                enemyUI.SetActive(true);
                meatUI.SetActive(false);
                break;

            case TileType.Meat:
                meatUI.SetActive(true);
                enemyUI.SetActive(false);
                break;

            default:
                enemyUI.SetActive(false);
                meatUI.SetActive(false);
                break;
        }
    }

    public void SetEnemyDirection(EnemyDirection direction)
    {
        CurrentEnemyDirection = direction;
        for(int i = 0; i < 4; i++)
        {
            enemyUI.transform.Find("Direction").transform.GetChild(i).gameObject.SetActive(false);
        }
        enemyUI.transform.Find("Direction").transform.GetChild((int)direction).gameObject.SetActive(true);
    }
    public void SetEnemyHP(int hp)
    {
        EnemyHP = hp;
        enemyUI.transform.Find("HP").transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = hp.ToString();
    }
    public void SetEnemyAtk(int atk)
    {
        EnemyAtk = atk;
        enemyUI.transform.Find("ATK").transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = atk.ToString();
    }
    public void SetMeatHP(int hp)
    {
        MeatHP = hp;
        meatUI.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = hp.ToString();
    }
}
