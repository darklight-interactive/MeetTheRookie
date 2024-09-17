
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Darklight.UnityExt.Editor;

namespace Darklight.UnityExt.ObjectLibrary
{

    public class ObjectLibrary<TObject> : ScriptableObject, IObjectLibrary<TObject>
        where TObject : Object
    {
        protected TObject _defaultObject;
        [SerializeField] protected List<TObject> _objects;

        public List<TObject> Objects => _objects;
        public Type ObjectType => typeof(TObject);
        public TObject DefaultObject => _objects.Count > 0 ? _objects[0] : null;

        public void AddDefaultObject()
        {
            _objects.Add(DefaultObject);
        }

        public void Add(TObject value)
        {
            if (!_objects.Contains(value))
                _objects.Add(value);
        }

        public bool Remove(TObject value)
        {
            return _objects.Remove(value);
        }

        public bool Contains(TObject value)
        {
            return _objects.Contains(value);
        }

    }

    public class ObjectLibrary<TKey, TObject> : ObjectLibrary<TObject>, IObjectLibrary<TKey, TObject>
        where TKey : notnull
        where TObject : Object
    {
        Dictionary<TKey, TObject> _dictionary = new Dictionary<TKey, TObject>();
        [SerializeField] List<TKey> _keys;

        public void RebuildDictionary()
        {
            if (_objects == null)
                _objects = new List<TObject>();
            if (_keys == null)
                _keys = new List<TKey>();

            _dictionary.Clear();
            for (int i = 0; i < _keys.Count; i++)
            {
                if (_objects[i] == null)
                    _objects[i] = _defaultObject;
                _dictionary.Add(_keys[i], _objects[i]);
            }
        }

        public void RegisterKey(TKey key)
        {
            RebuildDictionary();
            if (_dictionary.ContainsKey(key))
                return;

            _dictionary.Add(key, null);
            RefreshSerializedData();
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
}