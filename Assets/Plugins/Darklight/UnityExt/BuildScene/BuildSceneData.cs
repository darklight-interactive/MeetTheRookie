//  ================================ [[ BUILD SCENE DATA ]] ================================
using System.Linq;
using Darklight.UnityExt.Editor;
using UnityEngine;

public interface IBuildSceneData
{
    string Name { get; }
    string Path { get; set; }

    void Refresh();
}

/// <summary>
/// A Serializable class that stores the data for a scene in the UnityEditor's build settings.
/// </summary>
[System.Serializable]
public class BuildSceneData : IBuildSceneData
{
    [SerializeField, ShowOnly] string _name = "Null Scene";
    [SerializeField, ShowOnly] string _path;

    public string Name => _name;
    public string Path
    {
        get => _path;
        set => SetPath(value);
    }

    public BuildSceneData() { } // Empty constructor for serialization.
    public BuildSceneData(string path) => SetPath(path);

    void SetPath(string path)
    {
        _path = FormatPath(path);
        _name = FormatNameFromPath(_path);
    }
    string FormatPath(string path) => path.Replace("\\", "/"); // Replace all backslashes with forward slashes
    string FormatNameFromPath(string path) => path.Split('/').Last().Split('.').First(); // Get the name of the scene from the path

    public virtual void Refresh()
    {
        _name = FormatNameFromPath(_path);
    }
}