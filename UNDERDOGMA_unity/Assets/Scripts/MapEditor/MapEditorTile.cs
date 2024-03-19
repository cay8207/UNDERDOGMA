using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class MapEditorTile : MonoBehaviour
{
    public enum TileType
    {
        Wall, Empty, Player, Enemy, Meat
    }

    public TileType CurrentTileType;
    public enum EnemyDirection
    {
        Up = 0, Down = 1, Left = 2, Right = 3
    }
    public EnemyDirection CurrentEnemyDirection;
    public enum EnemyType
    {
        NormalEnemy, ChaserEnemy, MiniBoss
    }
    public EnemyType CurrentEnemyType;

    [System.Serializable]
    public class TileSprites
    {
        public Sprite None;
        public Sprite Empty;
        public Sprite Player;
        public Sprite Meat;
        public Sprite NormalEnemy;
        public Sprite ChaserEnemy;
        public Sprite MiniBoss;
    }

    [Header("Coordinate")]
    [SerializeField]
    int x;
    [SerializeField]
    int y;

    [Header("Sprites")]
    [SerializeField]
    TileSprites tileSprites;

    [SerializeField]
    List<Sprite> tileShape;

    [Header("Settings")]
    [SerializeField]
    GameObject enemyUI;
    [SerializeField]
    GameObject meatUI;

    private Image tileImage;

    private int enemyHP = 0;
    private int meatHP = 0;

    private MapEditor mapEditor;

    private void Start()
    {
        tileImage = GetComponent<Image>();
        SetTileType(TileType.Wall);
    }

    private Sprite GetCurrentSprite()
    {
        switch (CurrentTileType)
        {
            case TileType.Wall:
                return tileSprites.None;
            case TileType.Empty:
                return tileSprites.Empty;
            case TileType.Player:
                return tileSprites.Player;
            case TileType.Enemy:
                switch (CurrentEnemyType)
                {
                    case EnemyType.NormalEnemy:
                        return tileSprites.NormalEnemy;
                    case EnemyType.ChaserEnemy:
                        return tileSprites.ChaserEnemy;
                    case EnemyType.MiniBoss:
                        return tileSprites.MiniBoss;
                    default: return null;
                }
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

    public void SetTileSprite(List<Sprite> tileSprite)
    {
        tileSprites.None = tileSprite[0];
        tileSprites.Empty = tileSprite[1];
        tileSprites.Player = tileSprite[2];
        tileSprites.Meat = tileSprite[3];
        tileSprites.NormalEnemy = tileSprite[4];
        tileSprites.ChaserEnemy = tileSprite[5];
        tileSprites.MiniBoss = tileSprite[6];
        tileImage.sprite = GetCurrentSprite();
    }

    public void SetEnemyType(EnemyType enemyType)
    {
        CurrentEnemyType = enemyType;
        tileImage.sprite = GetCurrentSprite();
    }

    public void SetEnemyDirection(EnemyDirection direction)
    {
        CurrentEnemyDirection = direction;
        for (int i = 0; i < 4; i++)
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
    public void SetMeatHP(int hp)
    {
        MeatHP = hp;
        meatUI.transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = hp.ToString();
    }
    private string SetPlayerCoordinate()
    {
        SaveSystem.Instance.SetCharacterCoord(X, Y);
        return "Empty";
    }
    public JObject ToJSON()
    {
        var json = new JObject();
        json.Add("Type", CurrentTileType == TileType.Player? SetPlayerCoordinate() : CurrentTileType.ToString());
        json.Add("Round", "0");
        json.Add("Pattern", "0");
        json.Add("TileDirection", "None");
        switch (CurrentTileType)
        {
            case TileType.Enemy:
                json.Add("EnemyType", CurrentEnemyType.ToString());
                json.Add("IsAlive", true);
                json.Add("Attack", 1);
                json.Add("Heart", enemyHP);
                json.Add("AttackDirection", CurrentEnemyDirection.ToString());
                break;

            case TileType.Meat:
                json.Add("Amount", meatHP);
                json.Add("IsExist", true);
                break;

            default:
                break;
        }
        return json;
    }
    public void FromJSON(JObject data)
    {
        data["Type"].ToString();
    }
}