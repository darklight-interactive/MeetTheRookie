using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


namespace Darklight.UnityExt.Library
{

    public interface ILibrary
    {
        void SetToDefaults();
    }

    public interface ILibrary<TKey, TValue> : ILibrary, IDictionary<TKey, TValue>
    {
        TKey CreateDefaultKey();
        TValue CreateDefaultValue();
    }

    [System.Serializable]
    public class LibraryItem<TKey, TValue>
    {
        [SerializeField] TKey _key;
        [SerializeField] TValue _value;

        public TKey Key { get => _key; set => _key = value; }
        public TValue Value { get => _value; set => _value = value; }

        public LibraryItem(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    #region == (( CLASS : Library<TKey, TValue> )) ============================================= ))
    [System.Serializable]
    public abstract class Library<TKey, TValue> : ILibrary<TKey, TValue>, ISerializationCallbackReceiver
        where TKey : notnull
        where TValue : notnull
    {
        // ======== [[ FIELDS ]] ===================================== >>>>
        Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        [SerializeField] List<LibraryItem<TKey, TValue>> _items = new List<LibraryItem<TKey, TValue>>();

        #region ======== [[ PROPERTIES ]] ================================== >>>>
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

        public List<LibraryItem<TKey, TValue>> Items => _items;
        #endregion

        public event Action<TKey, TValue> ItemAdded;
        public event Action<TKey> ItemRemoved;

        // ======== [[ CONSTRUCTORS ]] ===================================== >>>>
        public Library()
        {
            _items = new List<LibraryItem<TKey, TValue>>();
        }

        // ======== [[ METHODS ]] ===================================== >>>>        
        #region -- (( IDictionary<TKey, TValue> )) --
        public void Add(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
                return;

            _dictionary.Add(key, value);
            _items.Add(new LibraryItem<TKey, TValue>(key, value));

            ItemAdded?.Invoke(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.Remove(key))
            {
                LibraryItem<TKey, TValue> item = _items.FirstOrDefault(i => EqualityComparer<TKey>.Default.Equals(i.Key, key));
                if (item != null)
                {
                    _items.Remove(item);
                }
                return true;
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
            _items.Clear();
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

        #region -- (( ISerializationCallbackReceiver )) --
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            _dictionary.Clear();
            if (_items == null || _items.Count == 0) return;

            foreach (LibraryItem<TKey, TValue> entry in _items)
            {
                if (entry == null)
                {
                    Debug.LogWarning("Null entry found during deserialization. Skipping.");
                    continue;
                }

                if (!_dictionary.ContainsKey(entry.Key))
                {
                    _dictionary.Add(entry.Key, entry.Value);
                }
                else
                {
                    Debug.LogWarning($"Duplicate key '{entry.Key}' found during deserialization. Skipping.");
                }
            }
        }


        #endregion

        public void Add(LibraryItem<TKey, TValue> item)
        {
            if (!_dictionary.ContainsKey(item.Key))
            {
                _dictionary.Add(item.Key, item.Value);
                _items.Add(item);
            }
            else
            {
                Debug.LogWarning($"Key '{item.Key}' already exists in the library. Skipping.");
            }
        }

        public abstract TKey CreateDefaultKey();
        public abstract TValue CreateDefaultValue();
        public virtual void SetToDefaults()
        {
            Clear();
            Add(CreateDefaultKey(), CreateDefaultValue());
        }
    }
    #endregion

    [Serializable]
    public class StringObjectLibrary : Library<string, UnityEngine.Object>
    {
        public override string CreateDefaultKey()
        {
            return "Default";
        }

        public override UnityEngine.Object CreateDefaultValue()
        {
            return default(UnityEngine.Object);
        }
    }

    public static class LibraryUtility
    {
        public const string LIBRARY_PATH = "Darklight/ObjectLibrary/";
        public const string PRIMITIVE_PATH = LIBRARY_PATH + "Primitive/";
        public const string KEY_VALUE_PATH = LIBRARY_PATH + "KeyValue/";
    }

}