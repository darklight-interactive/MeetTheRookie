using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class SceneLoaderUtility
{
    public static List<string> LoadScenesFromDirectory(string directoryPath)
    {
        string[] scenePaths = Directory.GetFiles(directoryPath, "*.unity", SearchOption.AllDirectories);
        return scenePaths.ToList();
    }
}
