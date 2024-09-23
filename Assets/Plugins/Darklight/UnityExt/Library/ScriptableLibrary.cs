using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Darklight.UnityExt.Library
{
    public abstract class ScriptableLibrary<TKey, TValue, TLib> : ScriptableObject, ILibrary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
        where TLib : Library<TKey, TValue>, new()
    {
        protected TLib library = new TLib();

        #region -- (( IDictionary<TKey, TValue> )) --
        public TValue this[TKey key]
        {
            get => library[key];
            set => library[key] = value;
        }
        public ICollection<TKey> Keys => library.Keys;
        public ICollection<TValue> Values => library.Values;
        public int Count => library.Count;
        public bool IsReadOnly => library.IsReadOnly;
        public void Add(TKey key, TValue value) => library.Add(key, value);
        public bool ContainsKey(TKey key) => library.ContainsKey(key);
        public bool Remove(TKey key) => library.Remove(key);
        public bool TryGetValue(TKey key, out TValue value) => library.TryGetValue(key, out value);
        public void Add(KeyValuePair<TKey, TValue> item) => library.Add(item);
        public void Clear() => library.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => library.Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => library.CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item) => library.Remove(item);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => library.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => library.GetEnumerator();
        #endregion

        // ILibrary<TKey, TValue> members
        public bool ReadOnlyKey { get => library.ReadOnlyKey; set => library.ReadOnlyKey = value; }
        public bool ReadOnlyValue { get => library.ReadOnlyValue; set => library.ReadOnlyValue = value; }
        public IEnumerable<TKey> RequiredKeys { get => library.RequiredKeys; set => library.RequiredKeys = value; }
        public List<LibraryItem<TKey, TValue>> Items => library.Items;
        public TKey CreateDefaultKey() => library.CreateDefaultKey();
        public TValue CreateDefaultValue() => library.CreateDefaultValue();
        public void AddDefaultItem() => library.AddDefaultItem();
        public void AddItemWithDefaultValue(TKey key) => library.AddItemWithDefaultValue(key);
        public void RemoveAt(int index) => library.RemoveAt(index);
        public void Refresh() => library.Refresh();
        public void Reset() => library.Reset();
        public bool HasUnsetKeysOrValues() => library.HasUnsetKeysOrValues();
        public bool HasUnsetKeys() => library.HasUnsetKeysOrValues();
        public bool TryGetKeyByValue(TValue value, out TKey key) => library.TryGetKeyByValue(value, out key);
        public bool TryGetKeyByValue<T>(out TKey key) where T : TValue => library.TryGetKeyByValue<T>(out key);
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
