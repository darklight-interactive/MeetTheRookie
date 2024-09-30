using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Darklight.UnityExt.Editor;
using System.Collections;


namespace Darklight.UnityExt.Library
{
    public interface ILibrary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        bool ReadOnlyKey { get; set; }
        bool ReadOnlyValue { get; set; }
        IEnumerable<TKey> RequiredKeys { get; set; }
        List<LibraryItem<TKey, TValue>> Items { get; }

        TKey CreateDefaultKey();
        TValue CreateDefaultValue();
        void AddDefaultItem();
        void AddItemWithDefaultValue(TKey key);
        void RemoveAt(int index);
        void Refresh();
        void Reset();

        bool HasUnsetKeysOrValues();
        bool TryGetKeyByValue(TValue value, out TKey key);
        bool TryGetKeyByValue<T>(out TKey key) where T : TValue;
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

        public void Update(int id, TKey key, TValue value)
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
        Dictionary<TValue, TKey> _reverseDictionary = new Dictionary<TValue, TKey>();
        HashSet<TKey> _requiredKeys = new HashSet<TKey>();

        [SerializeField] List<LibraryItem<TKey, TValue>> _items = new List<LibraryItem<TKey, TValue>>();
        [SerializeField] bool _readOnlyKey = false;
        [SerializeField] bool _readOnlyValue = false;

        #region ==== [[ PROPERTIES ]] ================================== >>>>
        public bool ReadOnlyKey { get => _readOnlyKey; set => _readOnlyKey = value; }
        public bool ReadOnlyValue { get => _readOnlyValue; set => _readOnlyValue = value; }
        public IEnumerable<TKey> RequiredKeys
        {
            get => _requiredKeys.ToList();
            set => _requiredKeys = new HashSet<TKey>(value);
        }

        public List<LibraryItem<TKey, TValue>> Items => _items;
        #endregion

        #region ==== [[ EVENTS ]] ===================================== >>>>
        public event Action<TKey, TValue> ItemAdded;
        public event Action<TKey> ItemRemoved;
        #endregion

        // ======== [[ CONSTRUCTORS ]] ===================================== >>>>
        public Library() => InternalReset();

        // ======== [[ METHODS ]] ===================================== >>>>
        #region -- ( InternalUtility ) <PRIVATE_METHODS> --

        /// <summary>
        /// Check if the object is the default value. This is used to check for default values of types.
        /// </summary>
        bool IsDefault<T>(T obj)
        {
            // Handle nullable types
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                return obj == null;
            }
            return EqualityComparer<T>.Default.Equals(obj, default(T));
        }

        /// <summary>
        /// Check if the object is empty. This is used to check for empty strings or collections.
        /// </summary>
        bool IsEmpty<T>(T obj)
        {
            if (obj is string str)
                return str == string.Empty;

            if (obj is ICollection collection)
                return collection.Count == 0;

            return false;
        }

        void GenerateReverseDictionary()
        {
            _reverseDictionary.Clear();
            foreach (KeyValuePair<TKey, TValue> entry in _dictionary)
            {
                _reverseDictionary.Add(entry.Value, entry.Key);
            }
        }

        void EnsureRequiredKeys()
        {
            if (_requiredKeys == null || _requiredKeys.Count == 0)
                return;
            foreach (TKey key in _requiredKeys)
            {
                if (!ContainsKey(key))
                {
                    AddItemWithDefaultValue(key);
                }
            }
        }
        #endregion -- (( Utility Methods )) --

        #region -- ( Internal ) <PROTECTED_METHODS> --

        protected virtual void InternalAdd(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
                return;

            // Add the new item
            _dictionary.Add(key, value);

            InternalRefresh();
            ItemAdded?.Invoke(key, value);
        }

        protected virtual bool InternalRemove(TKey key)
        {
            if (_requiredKeys.Contains(key))
            {
                Debug.LogError($"Cannot remove required key '{key}' from the library.");
                return false;
            }

            if (_dictionary.Remove(key))
            {
                InternalRefresh();
                ItemRemoved?.Invoke(key);
                return true;
            }
            return false;
        }

        protected virtual void InternalAddOrUpdateItem(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;
            }
            else
            {
                _dictionary.Add(key, value);
            }
            InternalRefresh();
        }

        protected virtual void InternalRefresh()
        {
            int index = 0;
            EnsureRequiredKeys();

            foreach (KeyValuePair<TKey, TValue> entry in _dictionary)
            {
                InternalItemRefresh(entry, index);
                index++;
            }

            // If there are extra items in _items, remove them
            if (index < _items.Count)
            {
                _items.RemoveRange(index, _items.Count - index);
            }
        }

        protected virtual void InternalItemRefresh(KeyValuePair<TKey, TValue> entry, int index)
        {
            if (index < _items.Count)
            {
                LibraryItem<TKey, TValue> item = _items[index];

                // Check if current item needs updating
                if (!EqualityComparer<TKey>.Default.Equals(item.Key, entry.Key) ||
                    !EqualityComparer<TValue>.Default.Equals(item.Value, entry.Value))
                {
                    // Update the item and mark that a refresh was needed
                    item.Update(index, entry.Key, entry.Value);
                }
            }
            else
            {
                // Add new item if index exceeds current _items count
                _items.Add(new LibraryItem<TKey, TValue>(index, entry.Key, entry.Value));
            }
        }


        protected virtual void InternalClear()
        {
            _dictionary.Clear();
            _items.Clear();
        }

        protected virtual void InternalReset()
        {
            InternalClear();
            InternalRefresh();
        }

        #endregion

        #region -- ( Public Handlers ) <PUBLIC_METHODS> --
        public bool HasUnsetKeysOrValues()
        {
            foreach (KeyValuePair<TKey, TValue> entry in _dictionary)
            {
                // Null check for both key and value
                if (entry.Key == null || entry.Value == null)
                    return true;

                // Check if key or value is an enum, in which case only null checks are relevant
                bool keyIsEnum = typeof(TKey).IsEnum;
                bool valueIsEnum = typeof(TValue).IsEnum;

                // Check for default values if the key or value is not an enum or nullable
                if ((!keyIsEnum && IsDefault(entry.Key)) || (!valueIsEnum && IsDefault(entry.Value)))
                    return true;

                // Additional checks for empty collections or strings
                if (IsEmpty(entry.Key) || IsEmpty(entry.Value))
                    return true;
            }
            return false;
        }
        public bool TryGetValue<T>(TKey key, out T value) where T : TValue
        {
            if (_dictionary.TryGetValue(key, out TValue val) && val is T)
            {
                value = (T)val;
                return true;
            }
            value = default(T);
            return false;
        }
        public bool TryGetKeyByValue(TValue value, out TKey key)
        {
            GenerateReverseDictionary();
            return _reverseDictionary.TryGetValue(value, out key);
        }
        public bool TryGetKeyByValue<T>(out TKey key) where T : TValue
        {
            GenerateReverseDictionary();
            foreach (KeyValuePair<TValue, TKey> entry in _reverseDictionary)
            {
                if (entry.Key is T)
                {
                    key = entry.Value;
                    return true;
                }
            }
            key = default(TKey);
            return false;
        }
        #endregion

        public virtual TKey CreateDefaultKey() => default(TKey);
        public virtual TValue CreateDefaultValue() => default(TValue);
        public virtual void AddDefaultItem() => InternalAdd(CreateDefaultKey(), CreateDefaultValue());
        public virtual void AddItemWithDefaultValue(TKey key) => InternalAdd(key, CreateDefaultValue());
        public void RemoveAt(int index) => InternalRemove(_items[index].Key);
        public void Refresh() => InternalRefresh();
        public void Reset() => InternalReset();
        public void SetRequiredKeys(IEnumerable<TKey> keys)
        {
            _requiredKeys = new HashSet<TKey>(keys);
            EnsureRequiredKeys();
        }

        #region ==== [[ INTERFACE IMPLEMENTATION ]] ================================== >>>>
        #region -- ( IDictionary<TKey, TValue> ) <PUBLIC_INTERFACE_METHODS> --
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
            set => InternalAddOrUpdateItem(key, value);
        }
        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
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
        System.Collections.IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region -- ( ISerializationCallbackReceiver ) <PUBLIC_INTERFACE_METHODS> --
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<TKey, TValue>();
            if (_items == null || _items.Count == 0) return;

            List<LibraryItem<TKey, TValue>> removalItems = new List<LibraryItem<TKey, TValue>>();
            foreach (LibraryItem<TKey, TValue> entry in _items)
            {
                if (entry == null || entry.Key == null)
                    continue;

                if (!_dictionary.ContainsKey(entry.Key))
                {
                    _dictionary.Add(entry.Key, entry.Value);
                }
                else
                {
                    removalItems.Add(entry);
                    Debug.LogWarning($"Duplicate key '{entry.Key}' found during deserialization. Removing the duplicate entry.");
                }
            }
            foreach (LibraryItem<TKey, TValue> entry in removalItems)
            {
                _items.Remove(entry);
            }
        }
        #endregion
        #endregion
    }
    #endregion
}