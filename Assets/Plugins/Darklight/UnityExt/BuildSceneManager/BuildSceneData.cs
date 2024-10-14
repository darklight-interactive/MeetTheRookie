//  ================================ [[ BUILD SCENE DATA ]] ================================
using System.Linq;
using Darklight.UnityExt.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
/// </summary>
[System.Serializable]
public abstract class BuildSceneData
{
    Scene _scene;
    [SerializeField, ShowOnly] string _name = "Null Scene";
    [SerializeField, ShowOnly] string _path = "Null Path";

    //  ---------------- [ PROPERTIES ] -----------------------------
    public Scene Scene => _scene;
    public string Name => _name;
    public string Path => _path;

    // ---------------- [ CONSTRUCTORS ] -----------------------------
    public BuildSceneData() { } // Empty constructor for serialization.

    //  ---------------- [ Private Methods ] -----------------------------
    string FormatPath(string path) => path.Replace("\\", "/"); // Replace all backslashes with forward slashes

    //  ---------------- [ Public Abstract Methods ] -------------------------------
    public virtual void BuildFromPath(string path)
    {
        _scene = SceneManager.GetSceneByPath(path);
        _name = _scene.name;
        _path = FormatPath(_scene.path);
    }

    public virtual void Copy(BuildSceneData data)
    {
        _scene = data.Scene;
        _name = data.Name;
        _path = data.Path;
    }
}