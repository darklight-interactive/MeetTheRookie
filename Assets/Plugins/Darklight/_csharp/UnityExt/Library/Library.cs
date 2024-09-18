using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditorInternal;
using Darklight.UnityExt.Editor;
using System.Linq;
using UnityEngine.PlayerLoop;







#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Library
{
    public interface ILibrary<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        TValue CreateDefaultValue();

        event EventHandler<ItemAddedEventArgs<TKey, TValue>> ItemAdded;
        event EventHandler<ItemRemovedEventArgs<TKey>> ItemRemoved;
    }


    [System.Serializable]
    public class LibraryItem<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public LibraryItem(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    #region == (( CLASS : Library<TKey, TValue> )) ============================================= ))
    [System.Serializable]
    public class Library<TKey, TValue> : ILibrary<TKey, TValue>, ISerializationCallbackReceiver
        where TKey : notnull
        where TValue : notnull
    {
        // ======== [[ FIELDS ]] ===================================== >>>>
        Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        [SerializeField] protected List<LibraryItem<TKey, TValue>> _items = new List<LibraryItem<TKey, TValue>>();

        // ======== [[ EVENTS ]] ===================================== >>>>
        public event EventHandler<ItemAddedEventArgs<TKey, TValue>> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs<TKey>> ItemRemoved;

        // ======== [[ PROPERTIES ]] ================================== >>>>
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
        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            _dictionary.Clear();
            foreach (var entry in _items)
            {
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
                OnItemAdded(item.Key, item.Value);
            }
            else
            {
                Debug.LogWarning($"Key '{item.Key}' already exists in the library. Skipping.");
            }
        }

        public virtual TValue CreateDefaultValue()
        {
            return default;
        }


        public virtual void AddKeys(IEnumerable<TKey> keys)
        {
            foreach (TKey key in keys)
            {
                if (!_dictionary.ContainsKey(key))
                {
                    Add(key, CreateDefaultValue());
                }
                else
                {
                    if (this[key] == null || this[key].Equals(default(TValue)))
                    {
                        this[key] = CreateDefaultValue();
                    }
                }
            }
        }

        public void RemoveAllKeysExcept(IEnumerable<TKey> keys)
        {
            List<TKey> keysToRemove = new List<TKey>();
            foreach (TKey key in _dictionary.Keys)
            {
                if (!keys.Contains(key))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (TKey key in keysToRemove)
            {
                Remove(key);
            }
        }

        public void RebuildLibrary()
        {
            _dictionary.Clear();
            foreach (var item in _items)
            {
                _dictionary.Add(item.Key, item.Value);
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
    }

    public class ItemAddedEventArgs<TKey, TValue> : EventArgs
    {
        public TKey Key { get; }
        public TValue Value { get; }

        public ItemAddedEventArgs(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public class ItemRemovedEventArgs<TKey> : EventArgs
    {
        public TKey Key { get; }

        public ItemRemovedEventArgs(TKey key)
        {
            Key = key;
        }
    }

    public static class LibraryUtility
    {
        public const string LIBRARY_PATH = "Darklight/ObjectLibrary/";
        public const string PRIMITIVE_PATH = LIBRARY_PATH + "Primitive/";
        public const string KEY_VALUE_PATH = LIBRARY_PATH + "KeyValue/";
    }
    #endregion



}