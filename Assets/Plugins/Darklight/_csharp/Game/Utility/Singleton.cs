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

}


