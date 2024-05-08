using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Editor utility script to load scenes from a specified directory
public static class SceneLoaderUtility
{
    public static List<SceneAsset> LoadScenesFromDirectory(string directoryPath)
    {
        string[] scenePaths = Directory.GetFiles(directoryPath, "*.unity", SearchOption.AllDirectories);
        return scenePaths
            .Select(path => AssetDatabase.LoadAssetAtPath<SceneAsset>(path))
            .Where(scene => scene != null)
            .ToList();
    }
}