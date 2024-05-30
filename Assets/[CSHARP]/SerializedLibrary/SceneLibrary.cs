using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using UnityEngine;

[CreateAssetMenu(menuName = "Darklight/SceneLibrary")]
public class SceneLibrary : ScriptableObject
{
    [SerializeField]
    public List<SceneIdentifier> sceneIdentifiers = new List<SceneIdentifier>();
}

[System.Serializable]
public class SceneIdentifier 
{   
    public float sceneID;
    public SceneObject sceneObject;
}