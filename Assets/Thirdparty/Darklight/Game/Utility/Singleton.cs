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


        /// <summary>
        /// Destroy the singleton instance and the GameObject it is attached to.
        /// </summary>
        public static void DestroySingleton()
        {
            if (_instance != null)
            {
                if (Application.isPlaying)
                {
                    //Debug.Log($"{Prefix} Destroying instance.");
                    Destroy(_instance.gameObject);
                }
#if UNITY_EDITOR
                else
                {
                    //Debug.Log($"{Prefix} Destroying instance in editor.");
                    DestroyImmediate(_instance.gameObject);
                }
#endif
            }
            _instance = null;
        }

        public virtual void Awake()
        {
            // If an instance does not exist, set it to this object
            if (_instance == null)
            {
                _instance = this as T;
                Debug.Log($"{Prefix} Awake: Setting instance to {name}.");
            }
            else
            {
                DestroySingleton();
                return;
            }

            // Initialize the singletonObject
            DontDestroyOnLoad(this.gameObject);
        }
    }
}


