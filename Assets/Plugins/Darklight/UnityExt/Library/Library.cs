using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Darklight.UnityExt.Editor;
using System.Collections;


namespace Darklight.UnityExt.Library
{
    public interface HasNullOrUnsetItems<TKey, TValue> : IDictionary<TKey, TValue>
    {
        List<LibraryItem<TKey, TValue>> Items { get; }

        TKey CreateDefaultKey();
        TValue CreateDefaultValue();
        void AddDefaultItem();
        void Reset();

        bool HasUnsetKeysOrValues();
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
    public class Library<TKey, TValue> : LibraryBase, HasNullOrUnsetItems<TKey, TValue>, ISerializationCallbackReceiver
        where TKey : notnull
        where TValue : notnull
    {
        // ======== [[ FIELDS ]] ===================================== >>>>
        Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        [SerializeField] List<LibraryItem<TKey, TValue>> _items = new List<LibraryItem<TKey, TValue>>();
        [SerializeField] protected bool readOnlyKey = false;
        [SerializeField] protected bool readOnlyValue = false;

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
            set => InternalAddOrUpdateItem(key, value);
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
            Reset();
        }

        public Library(bool readOnlyKey) : this() => this.readOnlyKey = readOnlyKey;
        public Library(bool readOnlyKey, bool readOnlyValue) : this(readOnlyKey) => this.readOnlyValue = readOnlyValue;

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

        #region -- <PRIVATE_METHODS> (( Internal Methods )) --
        void InternalAdd(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
                return;

            // Add the new item
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

        void InternalAddOrUpdateItem(TKey key, TValue value)
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

        void InternalRefresh()
        {
            int index = 0;
            bool needsRefresh = false;

            foreach (KeyValuePair<TKey, TValue> entry in _dictionary)
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
                        needsRefresh = true;
                    }
                }
                else
                {
                    // Add new item if index exceeds current _items count
                    _items.Add(new LibraryItem<TKey, TValue>(index, entry.Key, entry.Value));
                    needsRefresh = true;
                }
                index++;
            }

            // If there are extra items in _items, remove them
            if (index < _items.Count)
            {
                _items.RemoveRange(index, _items.Count - index);
                needsRefresh = true;
            }

            // Log refresh if needed
            if (needsRefresh)
            {
                Debug.Log($"{GetType().Name} has been refreshed.");
            }
        }

        void InternalClear()
        {
            _dictionary.Clear();
            _items.Clear();
            Debug.Log($"{GetType().Name} has been cleared.");
        }
        #endregion

        public virtual TKey CreateDefaultKey() => default(TKey);
        public virtual TValue CreateDefaultValue() => default(TValue);
        public virtual void AddDefaultItem() => InternalAdd(CreateDefaultKey(), CreateDefaultValue());
        public virtual void Reset() => InternalClear();
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

        // Helper method to check if a value is default for its type
        private bool IsDefault<T>(T obj)
        {
            // Handle nullable types
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                return obj == null;
            }
            return EqualityComparer<T>.Default.Equals(obj, default(T));
        }

        // Helper method to check if a value is empty (e.g., string or collection)
        private bool IsEmpty<T>(T obj)
        {
            if (obj is string str)
                return str == string.Empty;

            if (obj is ICollection collection)
                return collection.Count == 0;

            return false;
        }
    }
    #endregion

    public abstract class EnumKeyLibrary<TKey, TValue> : Library<TKey, TValue>
        where TKey : System.Enum
        where TValue : notnull
    {
        protected bool containAllKeyValues = false;
        List<TKey> enumValues => Enum.GetValues(typeof(TKey)).Cast<TKey>().ToList();

        public EnumKeyLibrary(bool containAllKeyValues, bool readOnlyKey, bool readOnlyValue)
            : base(readOnlyKey, readOnlyValue)
        {
            this.containAllKeyValues = containAllKeyValues;
        }

        TKey GetAvailableKey()
        {
            foreach (TKey key in enumValues)
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
            //Debug.Log($"Resetting {GetType().Name} :  containAllKeyValues = {containAllKeyValues}");

            if (containAllKeyValues)
            {
                foreach (TKey key in enumValues)
                {
                    Add(key, CreateDefaultValue());
                }
            }
        }
    }

    public abstract class IntKeyLibrary<TValue> : Library<int, TValue>
        where TValue : notnull
    {
        public IntKeyLibrary(bool readOnlyKey, bool readOnlyValue)
            : base(readOnlyKey, readOnlyValue)
        {
        }

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
        public StringKeyLibrary(bool readOnlyKey, bool readOnlyValue)
            : base(readOnlyKey, readOnlyValue)
        {
        }

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