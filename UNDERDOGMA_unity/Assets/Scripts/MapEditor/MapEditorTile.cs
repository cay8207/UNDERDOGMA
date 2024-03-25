using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System;
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

    private void Awake()
    {
        tileImage = GetComponent<Image>();
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
        UpdateTileTypeUI();
    }

    private void UpdateTileTypeUI()
    {
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
        UpdateUI();
    }

    public void SetEnemyType(EnemyType enemyType)
    {
        CurrentEnemyType = enemyType;
        UpdateUI();
    }

    public void SetEnemyDirection(EnemyDirection direction)
    {
        CurrentEnemyDirection = direction;
        UpdateUI();
    }

    public void SetEnemyHP(int hp)
    {
        EnemyHP = hp;
        UpdateUI();
    }

    public void SetMeatHP(int hp)
    {
        MeatHP = hp;
        UpdateUI();
    }

    public void UpdateUI()
    {
        //타일 이미지 변경
        tileImage.sprite = GetCurrentSprite();
        //적 방향 UI 변경
        for (int i = 0; i < 4; i++)
        {
            enemyUI.transform.Find("Direction").transform.GetChild(i).gameObject.SetActive(false);
        }
        enemyUI.transform.Find("Direction").transform.GetChild((int)CurrentEnemyDirection).gameObject.SetActive(true);
        //적 HP 텍스트 변경
        enemyUI.transform.Find("HP").transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = EnemyHP.ToString();
        //고기 HP 텍스트 변경
        meatUI.transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = MeatHP.ToString();
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

    public JObject ToJSON()
    {
        if (CurrentTileType == TileType.Player) SaveSystem.Instance.SetCharacterCoord(X, Y);
        var json = new JObject();
        json.Add("Type", CurrentTileType == TileType.Player? "Empty" : CurrentTileType.ToString());
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
        if (!Enum.TryParse<TileType>(data["Type"].ToString(), out CurrentTileType))
        {
            Debug.Log("TileTypeError");
            return;
        }
        //추후 Round, Pattern, TileDirection 쓸 일 있으면 여기에 추가

        switch (CurrentTileType)
        {
            case TileType.Enemy:
                Enum.TryParse<EnemyType>(data["EnemyType"].ToString(), out CurrentEnemyType);
                int.TryParse(data["Heart"].ToString(), out enemyHP);
                Enum.TryParse<EnemyDirection>(data["AttackDirection"].ToString(), out CurrentEnemyDirection);
                break;

            case TileType.Meat:
                int.TryParse(data["Amount"].ToString(), out meatHP);
                break;

            default:
                break;
        }

        UpdateTileTypeUI();
    }

}