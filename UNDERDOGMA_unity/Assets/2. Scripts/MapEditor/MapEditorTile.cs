#if UNITY_EDITOR
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
    //[HideInInspector] 
    public bool IsSelected = false;
    [SerializeField] GameObject isSelectedSprite;
    public enum TileType
    {
        Wall, Empty, Player, Enemy, Meat, Ball
    }

    public TileType CurrentTileType;
    public enum SpriteDirection
    {
        Left = 0, Right = 1
    }
    public SpriteDirection CurrentSpriteDirection;

    public enum EnemyType
    {
        NormalCat, NormalEnemy, StrongAttack, SoulLink, ChaserEnemy, Angel, MiniBoss
    }
    public EnemyType CurrentEnemyType;
    public int SoulNum;

    [System.Serializable]
    public class TileSprites
    {
        public Sprite None;
        public Sprite Empty;
        public Sprite Player;
        public Sprite Meat;
        public Sprite Ball;
        public Sprite NormalCat;
        public Sprite NormalEnemy;
        public Sprite StrongAttack;
        public Sprite SoulLink;
        public Sprite ChaserEnemy;
        public Sprite Angel;
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

    public void SetEnemyDirection(SpriteDirection direction)
    {
        CurrentSpriteDirection = direction;
        UpdateUI();
    }

    public void SetEnemyHP(int hp)
    {
        EnemyHP = hp;
        UpdateUI();
    }

    public void SetSoulNum(int soulNum)
    {
        SoulNum = soulNum;
        UpdateUI();
    }

    public void SetMeatHP(int hp)
    {
        MeatHP = hp;
        UpdateUI();
    }

    public void UpdateUI()
    {
        isSelectedSprite.SetActive(IsSelected);
        //Get Tile Sprite
        tileImage.sprite = GetCurrentSprite();
        //Sprite Direction Arrow Update(Will be actual Sprite Direction afterwards)
        for (int i = 0; i < 2; i++)
        {
            enemyUI.transform.Find("Direction").transform.GetChild(i).gameObject.SetActive(false);
        }
        enemyUI.transform.Find("Direction").transform.GetChild((int)CurrentSpriteDirection).gameObject.SetActive(true);
        //SoulNum Update
        enemyUI.transform.Find("SoulNum").transform.GetChild(0).gameObject.SetActive(CurrentEnemyType == EnemyType.SoulLink);
        enemyUI.transform.Find("SoulNum").transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = SoulNum.ToString();
        //Enemy HP UI Update
        enemyUI.transform.Find("HP").transform.GetChild(1).transform.GetComponent<TextMeshProUGUI>().text = EnemyHP.ToString();
        //Meat HP UI Update
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
                    case EnemyType.NormalCat:
                        return tileSprites.NormalCat;
                    case EnemyType.NormalEnemy:
                        return tileSprites.NormalEnemy;
                    case EnemyType.StrongAttack:
                        return tileSprites.StrongAttack;
                    case EnemyType.SoulLink:
                        return tileSprites.SoulLink;
                    case EnemyType.ChaserEnemy:
                        return tileSprites.ChaserEnemy;
                    case EnemyType.Angel:
                        return tileSprites.Angel;
                    case EnemyType.MiniBoss:
                        return tileSprites.MiniBoss;
                    default: return null;
                }
            case TileType.Meat:
                return tileSprites.Meat;
            case TileType.Ball:
                return tileSprites.Ball;
            default:
                return null;
        }
    }

    public JObject ToJSON()
    {
        if (CurrentTileType == TileType.Player) SaveSystem.Instance.SetCharacterCoord(X, Y);

        var json = new JObject();

        json.Add("Type", CurrentTileType == TileType.Player ? "Empty" : CurrentTileType.ToString());

        if (CurrentTileType != TileType.Wall)
        {
            //Pattern and TileDirection were here...
        }

        switch (CurrentTileType)
        {
            case TileType.Enemy:
                json.Add("EnemyType", CurrentEnemyType.ToString());
                json.Add("IsAlive", true);
                json.Add("Heart", enemyHP);
                if (CurrentEnemyType == EnemyType.SoulLink) json.Add("SoulNum", SoulNum);
                json.Add("SpriteDirection", CurrentSpriteDirection.ToString());
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

        switch (CurrentTileType)
        {
            case TileType.Enemy:
                Enum.TryParse<EnemyType>(data["EnemyType"].ToString(), out CurrentEnemyType);
                int.TryParse(data["Heart"].ToString(), out enemyHP);
                if (CurrentEnemyType == EnemyType.SoulLink) int.TryParse(data["SoulNum"].ToString(), out SoulNum);
                Enum.TryParse<SpriteDirection>(data["SpriteDirection"].ToString(), out CurrentSpriteDirection);
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
#endif