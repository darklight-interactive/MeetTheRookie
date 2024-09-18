using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Darklight.UnityExt.Library
{
    public class EnumObjectScriptableLibrary<TKey, TValue> : ScriptableLibrary<TKey, TValue>
        where TKey : System.Enum
        where TValue : UnityEngine.Object
    {
        public override TValue CreateDefaultValue() => default(TValue);

        /// <summary>
        /// Resets the library to contain all possible keys from the enum TKey,
        /// with each value set to the default value of TValue.
        /// </summary>
        public void SetToDefaults()
        {
            // Get all possible values of the enum TKey
            IEnumerable<TKey> enumValues = Enum.GetValues(typeof(TKey)).Cast<TKey>();

            // Clear the current contents of the library
            Clear();

            // Add each enum value as a key with the default value for TValue
            foreach (TKey key in enumValues)
            {
                Add(key, CreateDefaultValue());
            }
        }
    }
}