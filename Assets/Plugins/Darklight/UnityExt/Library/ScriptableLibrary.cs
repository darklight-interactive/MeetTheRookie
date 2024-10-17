using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Darklight.UnityExt.Library
{

#if UNITY_EDITOR
    using UnityEditor;
#endif
    public abstract class ScriptableLibrary<TKey, TValue, TLib> : ScriptableObject, ILibrary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
        where TLib : Library<TKey, TValue>, new()
    {
        public abstract TLib DataLibrary { get; }

        #region -- (( IDictionary<TKey, TValue> )) --
        public TValue this[TKey key]
        {
            get => DataLibrary[key];
            set => DataLibrary[key] = value;
        }
        public ICollection<TKey> Keys => DataLibrary.Keys;
        public ICollection<TValue> Values => DataLibrary.Values;
        public int Count => DataLibrary.Count;
        public bool IsReadOnly => DataLibrary.IsReadOnly;
        public void Add(TKey key, TValue value) => DataLibrary.Add(key, value);
        public bool ContainsKey(TKey key) => DataLibrary.ContainsKey(key);
        public bool Remove(TKey key) => DataLibrary.Remove(key);
        public bool TryGetValue(TKey key, out TValue value) => DataLibrary.TryGetValue(key, out value);
        public void Add(KeyValuePair<TKey, TValue> item) => DataLibrary.Add(item);
        public void Clear() => DataLibrary.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => DataLibrary.Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => DataLibrary.CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item) => DataLibrary.Remove(item);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => DataLibrary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => DataLibrary.GetEnumerator();
        #endregion

        // ILibrary<TKey, TValue> members
        public bool ReadOnlyKey { get => DataLibrary.ReadOnlyKey; set => DataLibrary.ReadOnlyKey = value; }
        public bool ReadOnlyValue { get => DataLibrary.ReadOnlyValue; set => DataLibrary.ReadOnlyValue = value; }
        public IEnumerable<TKey> RequiredKeys { get => DataLibrary.RequiredKeys; set => DataLibrary.RequiredKeys = value; }
        public List<LibraryItem<TKey, TValue>> Items => DataLibrary.Items;
        public TKey CreateDefaultKey() => DataLibrary.CreateDefaultKey();
        public TValue CreateDefaultValue() => DataLibrary.CreateDefaultValue();
        public void AddDefaultItem() => DataLibrary.AddDefaultItem();
        public void AddItemWithDefaultValue(TKey key) => DataLibrary.AddItemWithDefaultValue(key);
        public void RemoveAt(int index) => DataLibrary.RemoveAt(index);
        public void Refresh()
        {
            DataLibrary.Refresh();
#if UNITY_EDITOR
            EditorUtility.SetDirty(this); // Mark the ScriptableObject as dirty
            AssetDatabase.SaveAssets(); // Save changes to the asset
#endif
        }
        public void Reset() => DataLibrary.Reset();
        public bool HasUnsetKeysOrValues() => DataLibrary.HasUnsetKeysOrValues();
        public bool HasUnsetKeys() => DataLibrary.HasUnsetKeysOrValues();
        public bool TryGetKeyByValue(TValue value, out TKey key) => DataLibrary.TryGetKeyByValue(value, out key);
        public bool TryGetKeyByValue<T>(out TKey key) where T : TValue => DataLibrary.TryGetKeyByValue<T>(out key);

        public void SetValuesToDefault()
        {
            foreach (LibraryItem<TKey, TValue> item in Items)
            {
                DataLibrary[item.Key] = CreateDefaultValue();
            }
        }
    }


    public abstract class EnumObjectScriptableLibrary<TKey, TValue> : ScriptableLibrary<TKey, TValue, EnumObjectLibrary<TKey, TValue>>
        where TKey : Enum
        where TValue : UnityEngine.Object
    {
    }

    public abstract class EnumGameObjectScriptableLibrary<TKey> : ScriptableLibrary<TKey, GameObject, EnumGameObjectLibrary<TKey>>
        where TKey : Enum
    {
        public TComponent TryGetComponent<TComponent>(TKey key) where TComponent : Component
        {
            if (TryGetValue(key, out GameObject obj))
            {
                if (obj != null)
                {
                    return obj.GetComponent<TComponent>();
                }
            }

            return null;
        }
    }
}
