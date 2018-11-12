using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class RoomDataUtility
{
    public static void SaveRoom(string roomName , RoomData data )
    {
        RoomData asset = ScriptableObject.CreateInstance<RoomData>();

        asset = data;

        string assetPathExtension = "Assets/LevelDesign/SavedRoom/" + roomName + ".asset";

        AssetDatabase.CreateAsset(asset, assetPathExtension);

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
    }

    public static RoomData LoadRoom(string roomName)
    {
        return (RoomData)AssetDatabase.LoadAssetAtPath("Assets/LevelDesign/SavedRoom/" + roomName + ".asset", typeof(RoomData));
    }
}
