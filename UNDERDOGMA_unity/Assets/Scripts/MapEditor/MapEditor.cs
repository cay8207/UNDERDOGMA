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

    [ShowIf("tileType", MapEditorTile.TileType.Enemy), EnumToggleButtons, OnValueChanged("OnEnemyTypeChanged")]
    public MapEditorTile.EnemyType EnemyType;

    [ShowIf("tileType", MapEditorTile.TileType.Enemy), EnumToggleButtons, OnValueChanged("OnEnemyDirectionChanged")]
    public MapEditorTile.EnemyDirection EnemyDirection;

    [ShowIf("tileType", MapEditorTile.TileType.Enemy), OnValueChanged("OnEnemyHPChanged")]
    public int EnemyHP;

    [ShowIf("tileType", MapEditorTile.TileType.Meat), OnValueChanged("OnMeatHPChanged")]
    public int MeatHP;


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