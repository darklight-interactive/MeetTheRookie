using System.Collections.Generic;
using UnityEngine;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Library
{
    public static class LibraryUtility
    {
        public const string OBJECT_LIBRARY_PATH = "Darklight/ObjectLibrary/";
        public const string PRIMITIVE_PATH = OBJECT_LIBRARY_PATH + "Primitive/";
        public const string KEY_VALUE_PATH = OBJECT_LIBRARY_PATH + "KeyValue/";
    }

    #region == (( CLASS: Library<TKey, TValue> )) ============================================= ))

    [System.Serializable]
    public class Library<TKey, TValue> : ILibrary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        // ======== [[ FIELDS ]] ===================================== >>>>
        Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        [SerializeField] protected TValue _defaultValue;
        [SerializeField] protected List<TKey> _keys = new List<TKey>();
        [SerializeField] protected List<TValue> _values = new List<TValue>();

        // ======== [[ EVENTS ]] ===================================== >>>>
        public event EventHandler<ItemAddedEventArgs<TKey, TValue>> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs<TKey>> ItemRemoved;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        public TValue DefaultValue => _defaultValue;

        #region -- (( IDictionary<TKey, TValue> )) --
        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException($"Key '{key}' not found in the library.");
                }
            }
            set
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                    RefreshSerializedData();
                }
                else
                {
                    Add(key, value);
                }
            }
        }
        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        #endregion

        // ======== [[ CONSTRUCTORS ]] ===================================== >>>>
        public Library()
        {
            _keys = new List<TKey>();
            _values = new List<TValue>();
        }

        // ======== [[ METHODS ]] ===================================== >>>>        
        #region -- (( IDictionary<TKey, TValue> )) --
        public void Add(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                throw new ArgumentException($"An item with the same key '{key}' already exists.");
            }

            _dictionary.Add(key, value);
            RefreshSerializedData();
            OnItemAdded(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out TValue value))
            {
                bool removed = _dictionary.Remove(key);
                if (removed)
                {
                    RefreshSerializedData();
                    OnItemRemoved(key);
                }
                return removed;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
            RefreshSerializedData();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(_dictionary[item.Key], item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        #endregion

        public virtual TKey CreateDefaultKey()
        {
            return default;
        }

        public virtual TValue CreateDefaultValue()
        {
            return _defaultValue;
        }

        public void AddDefaultItem()
        {
            Add(CreateDefaultKey(), CreateDefaultValue());
        }

        public void RebuildDictionary()
        {
            _dictionary.Clear();
            if (_keys.Count != _values.Count)
            {
                Debug.LogWarning("Keys and objects lists are out of sync. Rebuilding may result in data loss.");
                // Optionally, handle mismatched counts appropriately
            }

            int count = Mathf.Min(_keys.Count, _values.Count);
            for (int i = 0; i < count; i++)
            {
                TKey key = _keys[i];
                TValue value = _values[i];

                if (key == null)
                    key = CreateDefaultKey();
                if (value == null)
                    value = CreateDefaultValue();

                if (!_dictionary.ContainsKey(key))
                {
                    _dictionary.Add(key, value);
                }
                else
                {
                    Debug.LogWarning($"Duplicate key '{key}' found during rebuild. Skipping.");
                }
            }
        }

        public void RefreshSerializedData()
        {
            _keys.Clear();
            _values.Clear();

            foreach (KeyValuePair<TKey, TValue> kvp in _dictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        // Event Invokers
        protected virtual void OnItemAdded(TKey key, TValue value)
        {
            ItemAdded?.Invoke(this, new ItemAddedEventArgs<TKey, TValue>(key, value));
        }

        protected virtual void OnItemRemoved(TKey key)
        {
            ItemRemoved?.Invoke(this, new ItemRemovedEventArgs<TKey>(key));
        }

        // Unity's method called when the script is loaded or a value is changed in the inspector
        private void OnValidate()
        {
            RebuildDictionary();
        }
    }
    #endregion
}