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
    public abstract class ScriptableLibrary<TKey, TValue> : ScriptableObject, ILibrary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        protected abstract Library<TKey, TValue> library { get; }

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
        public List<LibraryItem<TKey, TValue>> Items => library.Items;
        public TKey CreateDefaultKey() => library.CreateDefaultKey();
        public TValue CreateDefaultValue() => library.CreateDefaultValue();
        public void AddDefaultItem() => library.AddDefaultItem();
        public void Reset() => library.Reset();

    }

    public class EnumObjectScriptableLibrary<TEnum, TObj> : ScriptableLibrary<TEnum, TObj>
        where TEnum : System.Enum
        where TObj : UnityEngine.Object
    {
        [SerializeField] EnumObjectLibrary<TEnum, TObj> _library = new EnumObjectLibrary<TEnum, TObj>();

        protected override Library<TEnum, TObj> library => _library;
    }
}
