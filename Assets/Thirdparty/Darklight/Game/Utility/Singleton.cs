using System;
using UnityEngine;

namespace Darklight.Game.Utility
{
    /// <summary>
    /// Defines a basic, thread-safe singleton pattern for a data type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly Lazy<T> instance = new Lazy<T>(() => new T());
        public static T Instance => instance.Value;
        protected Singleton() { }
        public abstract void Initialize();
    }


    /// <summary>
    /// Defines a basic singleton pattern for a Unity MonoBehaviour type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static string Prefix => $"[{typeof(T).Name}]";
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

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                Debug.Log($"{Prefix} Awake: Setting instance to {name}.");
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                Debug.Log($"{Prefix} OnDestroy: Setting instance to null.");
                _instance = null;
            }
        }
    }

    public class SceneSingleton<T> : MonoBehaviourSingleton<T> where T : MonoBehaviour
    {
        protected SceneSingleton() { }

    }
}


