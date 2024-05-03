using System;
using UnityEngine;
using Darklight.Console;


#if UNITY_EDITOR
using UnityEditor;
#endif

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
}


