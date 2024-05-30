#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

public class MapEditor : OdinEditorWindow
{
    public MapEditorTile tile;

    [MenuItem("KinnyMoms/MapEditor")]
    private static void Init()
    {
        MapEditor mapEditor = (MapEditor)GetWindow(typeof(MapEditor));
        mapEditor.Show();
    }

    [EnumToggleButtons, OnValueChanged("OnTypeChanged")]
    public MapEditorTile.TileType tileType;

    //if TileType == Enemy
    [ShowIf("tileType", MapEditorTile.TileType.Enemy), EnumToggleButtons, OnValueChanged("OnEnemyTypeChanged")]
    public MapEditorTile.EnemyType EnemyType;

    [ShowIf("tileType", MapEditorTile.TileType.Enemy), EnumToggleButtons, OnValueChanged("OnEnemyDirectionChanged")]
    public MapEditorTile.EnemyDirection EnemyDirection;

    [ShowIf("tileType", MapEditorTile.TileType.Enemy), OnValueChanged("OnEnemyHPChanged")]
    public int EnemyHP;

    //if TileType == Meat
    [ShowIf("tileType", MapEditorTile.TileType.Meat), OnValueChanged("OnMeatHPChanged")]
    public int MeatHP;

    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            KeyCode keyCode = Event.current.keyCode;
            switch (keyCode)
            {
                case KeyCode.Alpha1:
                case KeyCode.Keypad1:
                    tileType = MapEditorTile.TileType.Wall;
                    break;
                case KeyCode.Alpha2:
                case KeyCode.Keypad2:
                    tileType = MapEditorTile.TileType.Empty;
                    break;
                case KeyCode.Alpha3:
                case KeyCode.Keypad3:
                    tileType = MapEditorTile.TileType.Player;
                    break;
                case KeyCode.Alpha4:
                case KeyCode.Keypad4:
                    tileType = MapEditorTile.TileType.Enemy;
                    break;
                case KeyCode.Alpha5:
                case KeyCode.Keypad5:
                    tileType = MapEditorTile.TileType.Meat;
                    break;
                case KeyCode.Alpha6:
                case KeyCode.Keypad6:
                    tileType = MapEditorTile.TileType.Ball;
                    break;
            }
            tile.SetTileType(tileType);
            UpdateTileValues();
            Debug.Log(keyCode);
        }
    }

    public void setTile(MapEditorTile t)
    {
        tile = t;
        tileType = t.CurrentTileType;
        UpdateTileValues();
    }

    public void OnTypeChanged()
    {
        tile.SetTileType(tileType);
        UpdateTileValues();
    }

    private void UpdateTileValues()
    {
        switch (tileType)
        {
            case MapEditorTile.TileType.Enemy:
                EnemyType = tile.CurrentEnemyType;
                EnemyDirection = tile.CurrentEnemyDirection;
                EnemyHP = tile.EnemyHP;
                break;
            case MapEditorTile.TileType.Meat:
                MeatHP = tile.MeatHP;
                break;
            default:
                break;
        }
        Repaint();
    }
    
    public void OnEnemyTypeChanged()
    {
        tile.SetEnemyType(EnemyType);
    }

    public void OnEnemyDirectionChanged()
    {
        tile.SetEnemyDirection(EnemyDirection);
    }

    public void OnEnemyHPChanged()
    {
        tile.SetEnemyHP(EnemyHP);
    }

    public void OnMeatHPChanged()
    {
        tile.SetMeatHP(MeatHP);
    }
}

#endif