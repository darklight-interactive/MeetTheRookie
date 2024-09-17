
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Darklight.UnityExt.Editor;

namespace Darklight.UnityExt.ObjectLibrary
{
    public interface IObjectLibrary<TKey, TObject>
        where TKey : notnull
        where TObject : UnityEngine.Object
    {
        List<TKey> AllKeys { get; }
        Type KeyType { get; }
        List<TObject> AllObjects { get; }
        Type ObjectType { get; }

        TKey CreateDefaultKey();
        TObject CreateDefaultObject();

        void RegisterKey(TKey key);
        void RegisterObject(TKey key, TObject value);
        bool RemoveObjectAt(TKey key);
        TObject GetObject(TKey key);
    }

    public abstract class ObjectLibrary<TKey, TObject> : ScriptableObject, IObjectLibrary<TKey, TObject>
        where TKey : notnull
        where TObject : Object
    {
        Dictionary<TKey, TObject> _dictionary = new Dictionary<TKey, TObject>();
        [SerializeField] TObject _defaultObject;
        [SerializeField] List<TObject> _objects;
        [SerializeField] List<TKey> _keys;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        public TObject DefaultObject => _defaultObject;
        public List<TKey> AllKeys
        {
            get
            {
                if (_keys == null)
                    _keys = new List<TKey>();
                return _keys;
            }
        }
        public Type KeyType => typeof(TKey);
        public List<TObject> AllObjects
        {
            get
            {
                if (_objects == null)
                    _objects = new List<TObject>();
                return _objects;
            }
        }
        public Type ObjectType => typeof(TObject);

        public virtual TKey CreateDefaultKey()
        {
            return default;
        }

        public virtual TObject CreateDefaultObject()
        {
            return _defaultObject;
        }

        public void RebuildDictionary()
        {
            if (_objects == null)
                _objects = new List<TObject>();
            if (_keys == null)
                _keys = new List<TKey>();

            _dictionary.Clear();
            for (int i = 0; i < _keys.Count; i++)
            {
                if (_keys[i] == null)
                    _keys[i] = CreateDefaultKey();
                if (_objects[i] == null)
                    _objects[i] = CreateDefaultObject();
                _dictionary.Add(_keys[i], _objects[i]);
            }
        }

        public void RegisterKey(TKey key)
        {
            RebuildDictionary();

            if (!_dictionary.ContainsKey(key))
            {
                _dictionary.Add(key, CreateDefaultObject());
                RefreshSerializedData();
            }
        }

        public void RegisterObject(TKey key, TObject value)
        {
            RebuildDictionary();

            _dictionary[key] = value;
            RefreshSerializedData();
        }

        public bool RemoveObjectAt(TKey key)
        {
            bool flag = _dictionary.Remove(key);
            RefreshSerializedData();
            return flag;
        }

        public TObject GetObject(TKey key)
        {
            return _dictionary.TryGetValue(key, out TObject value) ? value : null;
        }

        public void RefreshSerializedData()
        {
            _keys.Clear();
            _keys.AddRange(_dictionary.Keys);

            _objects.Clear();
            _objects.AddRange(_dictionary.Values);
        }
    }

    public abstract class ObjectLibrary<TObject> : ObjectLibrary<int, TObject>
        where TObject : Object
    {
        public override int CreateDefaultKey()
        {
            return 0;
        }

        public override TObject CreateDefaultObject()
        {
            return null;
        }
    }
}