#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapEditor : EditorWindow
{
    public GameObject tile;

    [MenuItem("KinnyMoms/MapEditor")]
    private static void Init()
    {
        MapEditor mapEditor =
            (MapEditor)GetWindow(typeof(MapEditor));
        mapEditor.Show();
    }

    //GUI 생성 함수
    private void OnGUI()
    {
        tile = (GameObject)EditorGUILayout.ObjectField("Tile", tile, typeof(GameObject), true);
    }
    
    public void test()
    {
        Debug.Log("WOW!");
    }
}

#endif