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
    public class ScriptableLibrary<TKey, TValue> : ScriptableObject, ILibrary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        [SerializeField] Library<TKey, TValue> _library = new Library<TKey, TValue>();

        // ======== [[ PROPERTIES ]] ================================== >>>>
        public TValue this[TKey key] { get => _library[key]; set => _library[key] = value; }
        public ICollection<TKey> Keys => _library.Keys;
        public ICollection<TValue> Values => _library.Values;

        public int Count => _library.Count;
        public bool IsReadOnly => _library.IsReadOnly;

        public event EventHandler<ItemAddedEventArgs<TKey, TValue>> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs<TKey>> ItemRemoved;

        // ======== [[ METHODS ]] ===================================== >>>>

        #region == (( Library<> Methods )) ========  ))
        public void Add(TKey key, TValue value) => _library.Add(key, value);
        public void Add(KeyValuePair<TKey, TValue> item) => _library.Add(item);
        public void Clear() => _library.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => _library.Contains(item);
        public bool ContainsKey(TKey key) => _library.ContainsKey(key);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _library.CopyTo(array, arrayIndex);
        public bool Remove(TKey key) => _library.Remove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => _library.Remove(item);
        public bool TryGetValue(TKey key, out TValue value) => _library.TryGetValue(key, out value);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _library.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _library.GetEnumerator();
        #endregion

        public virtual TValue CreateDefaultValue() => default(TValue);
        public virtual void AddKeys(IEnumerable<TKey> keys) => _library.AddKeys(keys);

        public void UpdateSerializedValues()
        {
            foreach (LibraryItem<TKey, TValue> item in _library.Items)
            {
                if (item.Value == null)
                {
                    item.Value = CreateDefaultValue();
                }
            }
        }

    }

}
