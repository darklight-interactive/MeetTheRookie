using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton Interface for MonoBehaviour. Meant to be a singleton that only exists for the length of a scene.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISceneSingleton<T> where T : MonoBehaviour {
    public static T Instance { get; protected set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init() {
        Instance = null;
    }

    public void Initialize() {
        if (Instance == null) {
            Instance = (T)this;
        } else {
            Debug.LogWarning("Singleton " + typeof(T).Name + " already exists.");
            Object.Destroy((T)this);
        }
    }
}

/// <summary>
/// Singelton Interface meant to persist across scenes
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGameSingleton<T> where T : MonoBehaviour
{
    public static T Instance { get; protected set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init()
    {
        Instance = null;
    }

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            GameObject instanceObject = Instance.gameObject;
            Object.DontDestroyOnLoad(instanceObject);
            Debug.Log($"IGameSingleton - {Instance.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Singleton " + typeof(T).Name + " already exists.");
            Object.Destroy((T)this);
        }
    }
}