
using System;
using System.Collections;
using System.Collections.Generic;

namespace Darklight.UnityExt.Library
{
    public interface ILibrary
    {
        void AddDefaultItem();
    }

    public interface ILibrary<TKey, TValue> : IDictionary<TKey, TValue>, ILibrary
        where TKey : notnull
        where TValue : notnull
    {
        TValue DefaultValue { get; }

        TKey CreateDefaultKey();
        TValue CreateDefaultValue();

        event EventHandler<ItemAddedEventArgs<TKey, TValue>> ItemAdded;
        event EventHandler<ItemRemovedEventArgs<TKey>> ItemRemoved;
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
}