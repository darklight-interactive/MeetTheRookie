using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Library;
using UnityEngine;

public abstract class EnumKeyLibrary<TKey, TValue> : Library<TKey, TValue>
    where TKey : System.Enum
    where TValue : notnull
{
    [SerializeField] bool _defaultToAllKeys = false;
    public EnumKeyLibrary(bool defaultToAllKeys = false) : base()
    {
        _defaultToAllKeys = defaultToAllKeys;
    }

    protected List<TKey> allEnumKeys => Enum.GetValues(typeof(TKey)).Cast<TKey>().ToList();
    TKey GetAvailableKey()
    {
        foreach (TKey key in allEnumKeys)
        {
            if (!ContainsKey(key))
            {
                return key;
            }
        }
        return CreateDefaultKey();
    }

    public sealed override TKey CreateDefaultKey() => GetAvailableKey();
    public sealed override void AddDefaultItem()
    {
        TKey key = GetAvailableKey();
        Add(key, CreateDefaultValue());

        Debug.LogWarning("Added default item with key: " + key);
    }

    protected sealed override void InternalRefresh()
    {
        if (_defaultToAllKeys) SetRequiredKeys(allEnumKeys);
        base.InternalRefresh();
    }
}

public abstract class IntKeyLibrary<TValue> : Library<int, TValue>
    where TValue : notnull
{
    protected override void InternalReset()
    {
        base.Reset();
        for (int i = 0; i < 10; i++)
        {
            Add(i, CreateDefaultValue());
        }
    }
}

public class StringKeyLibrary<TValue> : Library<string, TValue>
    where TValue : notnull
{
    protected override void InternalReset()
    {
        base.Reset();
        for (int i = 0; i < 10; i++)
        {
            string key = $"DefaultKey_{i}";
            Add(i.ToString(), CreateDefaultValue());
        }
    }
}