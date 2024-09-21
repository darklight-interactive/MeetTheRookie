using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Darklight.UnityExt.Editor;


namespace Darklight.UnityExt.Library
{
    public interface ILibrary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        List<LibraryItem<TKey, TValue>> Items { get; }

        TKey CreateDefaultKey();
        TValue CreateDefaultValue();
        void AddDefaultItem();
        void Reset();
    }

    [System.Serializable]
    public class LibraryItem<TKey, TValue>
    {
        [SerializeField, ShowOnly] int _id;
        [SerializeField] TKey _key;
        [SerializeField] TValue _value;

        public int Id { get => _id; }
        public TKey Key { get => _key; set => _key = value; }
        public TValue Value { get => _value; set => _value = value; }

        public LibraryItem(int id, TKey key, TValue value)
        {
            _id = id;
            _key = key;
            _value = value;
        }
    }

    public abstract class LibraryBase
    {
        public LibraryBase() { }
    }

    #region == (( CLASS : Library<TKey, TValue> )) ============================================= ))
    [System.Serializable]
    public class Library<TKey, TValue> : LibraryBase, ILibrary<TKey, TValue>, ISerializationCallbackReceiver
        where TKey : notnull
        where TValue : notnull
    {
        // ======== [[ FIELDS ]] ===================================== >>>>
        Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        [SerializeField] bool _showOnly = false;
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

        public Library(bool showOnly) : this()
        {
            _showOnly = showOnly;
        }

        // ======== [[ METHODS ]] ===================================== >>>>
        #region -- <PUBLIC_METHODS> (( IDictionary<TKey, TValue> )) --
        public void Add(TKey key, TValue value) => InternalAdd(key, value);
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(_dictionary[item.Key], item.Value);
        }
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public bool Remove(TKey key) => InternalRemove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        public void Clear() => InternalClear();
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.ToList().CopyTo(array, arrayIndex);
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region -- <PUBLIC_METHODS> (( ISerializationCallbackReceiver )) --
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

        #region -- <PRIVATE_METHODS> (( Internal Methods )) --
        void InternalAdd(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
                return;

            _dictionary.Add(key, value);
            InternalRefresh();

            ItemAdded?.Invoke(key, value);
        }

        bool InternalRemove(TKey key)
        {
            if (_dictionary.Remove(key))
            {
                InternalRefresh();
                ItemRemoved?.Invoke(key);
                return true;
            }
            return false;
        }

        void InternalRefresh()
        {
            _items.Clear();
            int index = 0;
            foreach (KeyValuePair<TKey, TValue> entry in _dictionary)
            {
                _items.Add(new LibraryItem<TKey, TValue>(index, entry.Key, entry.Value));
                index++;
            }
        }

        void InternalClear()
        {
            _dictionary.Clear();
            _items.Clear();
        }
        #endregion

        public virtual TKey CreateDefaultKey() => default(TKey);
        public virtual TValue CreateDefaultValue() => default(TValue);
        public virtual void AddDefaultItem() => Add(CreateDefaultKey(), CreateDefaultValue());
        public virtual void Reset() => Clear();
    }
    #endregion

    public abstract class EnumKeyLibrary<TKey, TValue> : Library<TKey, TValue>
        where TKey : System.Enum
        where TValue : notnull
    {
        List<TKey> _enumValues = Enum.GetValues(typeof(TKey)).Cast<TKey>().ToList();
        TKey GetAvailableKey()
        {
            _enumValues = Enum.GetValues(typeof(TKey)).Cast<TKey>().ToList();
            foreach (TKey key in _enumValues)
            {
                if (!ContainsKey(key))
                {
                    return key;
                }
            }
            return CreateDefaultKey();
        }

        public override TKey CreateDefaultKey() => GetAvailableKey();
        public override void AddDefaultItem() => Add(GetAvailableKey(), CreateDefaultValue());
        public override void Reset()
        {
            base.Reset();
            foreach (TKey key in _enumValues)
            {
                Add(key, CreateDefaultValue());
            }
        }
    }

    public abstract class IntKeyLibrary<TValue> : Library<int, TValue>
        where TValue : notnull
    {
        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < 10; i++)
            {
                Add(i, CreateDefaultValue());
            }
        }
    }

    public abstract class StringKeyLibrary<TValue> : Library<string, TValue>
        where TValue : notnull
    {
        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < 10; i++)
            {
                string key = $"DefaultKey_{i}";
                Add(i.ToString(), CreateDefaultValue());
            }
        }
    }

    public static class LibraryUtility
    {
        public const string LIBRARY_PATH = "Darklight/ObjectLibrary/";
        public const string PRIMITIVE_PATH = LIBRARY_PATH + "Primitive/";
        public const string KEY_VALUE_PATH = LIBRARY_PATH + "KeyValue/";
    }

}