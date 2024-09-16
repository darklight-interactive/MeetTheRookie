
using System;
using System.Collections.Generic;


namespace Darklight.UnityExt.ObjectLibrary
{
    public interface IObjectLibrary<TObject>
        where TObject : UnityEngine.Object
    {
        List<TObject> Objects { get; }
        Type ObjectType { get; }
        TObject DefaultObject { get; }

        void AddDefaultObject();
        void Add(TObject value);
        bool Remove(TObject value);
        bool Contains(TObject value);
    }

    public interface IObjectLibrary<TKey, TObject> : IObjectLibrary<TObject>
        where TKey : notnull
        where TObject : UnityEngine.Object
    {
        void RegisterObject(TKey key, TObject value);
        bool RemoveObjectAt(TKey key);
        TObject GetObject(TKey key);
    }
}