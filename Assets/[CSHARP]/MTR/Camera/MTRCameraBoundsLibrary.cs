using System;
using System.Collections.Generic;
using Darklight.UnityExt.Library;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class MTRCameraBoundsLibrary : Library<string, MTRCameraRigBounds>
{
    List<string> _sceneNames = new List<string>();
    public MTRCameraBoundsLibrary(List<string> sceneNames)
    {
        ReadOnlyKey = true;
        ReadOnlyValue = true;
        _sceneNames = sceneNames;
        SetRequiredKeys(sceneNames);
    }

    protected override void InternalRefresh()
    {
        base.InternalRefresh();
        foreach (string sceneName in RequiredKeys)
        {
            if (this[sceneName] == null)
            {
                //Debug.Log($"Create or Load Camera Bounds for {sceneName}");
                this[sceneName] = MTRAssetManager.CreateOrLoadCameraBounds(sceneName);
            }
        }
    }

    public void GetActiveCameraBounds(out MTRCameraRigBounds cameraBounds)
    {
        cameraBounds = null;
        string activeSceneName = SceneManager.GetActiveScene().name;
        if (ContainsKey(activeSceneName))
        {
            cameraBounds = this[activeSceneName];
        }
    }
}