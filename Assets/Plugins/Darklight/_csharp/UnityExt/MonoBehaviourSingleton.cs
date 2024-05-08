using System;
using UnityEngine;
using Darklight.Console;

/// <summary>
/// Defines a basic singleton pattern for a Unity MonoBehaviour type T.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static string Prefix => $"[{typeof(T).Name}]";
    public static ConsoleGUI Console = new ConsoleGUI();
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    // Create a new instance of the singleton if one doesn't exist
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    Debug.Log($"{Prefix} Created new instance.");
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        // If an instance does not exist, set it to this object
        if (_instance == null)
        {
            _instance = this as T;

            // Initialize the singletonObject
            if (Application.isPlaying)
                DontDestroyOnLoad(this.gameObject);
            Debug.Log($"{Prefix} Awake: Instance created.");
        }
        else if (_instance != this)
        {
            if (Application.isPlaying)
                Destroy(this.gameObject);
#if UNITY_EDITOR
            else
                DestroyImmediate(this.gameObject);
#endif
            return;
        }


    }
}