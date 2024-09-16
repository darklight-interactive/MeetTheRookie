
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
        [SerializeField] List<TObject> _objects = new List<TObject>();

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
        [SerializeField] List<TKey> _keys = new List<TKey>();

        public void RegisterObject(TKey key, TObject value)
        {
            _dictionary[key] = value;
        }

        public bool RemoveObjectAt(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public TObject GetObject(TKey key)
        {
            return _dictionary.TryGetValue(key, out TObject value) ? value : null;
        }

    }
}