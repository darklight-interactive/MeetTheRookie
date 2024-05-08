using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = Application.dataPath + "/AssetBundles";
        try
        {
            if (!System.IO.Directory.Exists(assetBundleDirectory))
            {
                System.IO.Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

    }
}

#endif
