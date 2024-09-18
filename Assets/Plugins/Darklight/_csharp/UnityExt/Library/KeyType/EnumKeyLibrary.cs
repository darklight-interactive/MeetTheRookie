using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Darklight.UnityExt.Library
{
    public abstract class EnumKeyLibrary<TKey, TValue> : Library<TKey, TValue>
        where TKey : System.Enum
        where TValue : notnull
    {
        [SerializeField] TKey _defaultKey;

        public override TKey CreateDefaultKey()
        {
            return _defaultKey;
        }

        public override void SetToDefaults()
        {
            Clear();

            foreach (TKey key in Enum.GetValues(typeof(TKey)).Cast<TKey>())
            {
                Add(key, CreateDefaultValue());
            }
        }
    }
}
