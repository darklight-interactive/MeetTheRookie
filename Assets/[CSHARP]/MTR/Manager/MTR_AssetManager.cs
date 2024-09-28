
using Darklight.UnityExt.Utility;
using UnityEngine;

public static class MTR_AssetManager
{
    public const string RESOURCE_PATH = "Assets/Resources/MeetTheRookie";

    /// <summary>
    /// Create or load a ScriptableObject of type T from the Resources folder
    /// that is named after the type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T CreateOrLoadScriptableObject<T>() where T : ScriptableObject
    {
        T scriptableObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<T>(RESOURCE_PATH, typeof(T).Name);
        return scriptableObject;
    }

    /// <summary>
    /// Create or load a ScriptableObject of type T from the Resources folder
    /// that is named after the string name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T CreateOrLoadScriptableObject<T>(string name) where T : ScriptableObject
    {
        T scriptableObject = ScriptableObjectUtility.CreateOrLoadScriptableObject<T>(RESOURCE_PATH, name);
        return scriptableObject;
    }
}