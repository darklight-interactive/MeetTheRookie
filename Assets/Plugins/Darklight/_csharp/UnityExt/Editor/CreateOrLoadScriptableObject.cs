using UnityEngine;
using UnityEditor;
using System.IO;
using System;

#if UNITY_EDITOR
public static class ScriptableObjectUtility
{
    public static T CreateOrLoadScriptableObject<T>(string path, string assetName) where T : ScriptableObject
    {
        // Ensure the path is formatted correctly
        if (!path.EndsWith("/"))
        {
            path += "/";
        }

        // Create the directory if it doesn't exist
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        // Combine the path and asset name to get the full path
        string assetPath = path + assetName + ".asset";

        // Load the asset if it exists
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            // Create and save the asset if it doesn't exist
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        return asset;
    }
}
#endif