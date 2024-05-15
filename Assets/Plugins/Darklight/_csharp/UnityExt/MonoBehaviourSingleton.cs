using System;
using UnityEngine;
using Darklight.Console;

/// <summary>
/// Defines a basic singleton pattern for a Unity MonoBehaviour type T.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            // If the instance is already set, return it.
            if (_instance != null)
                return _instance;

            // Check if an instance of T already exists in the scene.
            _instance = FindFirstObjectByType<T>();
            if (_instance != null)
                return _instance;

            // Create a new GameObject with the value of T.
            GameObject singletonObject = new GameObject($"MonoBehaviourSingleton :: {typeof(T).Name}");
            _instance = singletonObject.AddComponent<T>();
            DontDestroyOnLoad(singletonObject);
            return _instance;
        }
    }


    public static string Prefix => $"[{typeof(T).Name}]";
    public static ConsoleGUI Console = new ConsoleGUI();

    /// <summary>
    /// On Awake, check is this GameObject is the singleton instance.
    /// </summary>
    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (Application.isPlaying)
                DontDestroyOnLoad(this.gameObject);
        }

        /*
                if (_instance != null && _instance != this)
                {
                    Debug.LogWarning($"{Prefix} Singleton instance already exists. Disabling this instance.");
                    this.enabled = false;
                }
        */

    }
}