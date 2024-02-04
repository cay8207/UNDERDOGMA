using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorTile : MonoBehaviour
{
    public enum TileType
    {
        None, Empty, Player, Enemy, Meat
    }

    public TileType CurrentTileType;

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

    private Image tileImage;

    private int enemyDirection = 0;
    private int enemyHP = 0;
    private int enemyAtk = 0;
    private int meatHP = 0;

    private MapEditor mapEditor;

    private void Start()
    {
        tileImage = GetComponent<Image>();
        CurrentTileType = TileType.None;
        UpdateTileImage();
    }

    public void ChangeTile()
    {
        int currentIndex = (int)CurrentTileType;
        currentIndex = (currentIndex + 1) % System.Enum.GetValues(typeof(TileType)).Length;
        CurrentTileType = (TileType)currentIndex;
        UpdateTileImage();
    }

    private void UpdateTileImage()
    {
        tileImage.sprite = GetCurrentSprite();
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

    public int EnemyDirection
    {
        get { return enemyDirection; }
        set { enemyDirection = value; }
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

    public void getCurrentTile()
    {
        mapEditor = FindObjectOfType<MapEditor>();
        mapEditor.test();
    }
}
