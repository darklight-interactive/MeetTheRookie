using UnityEngine;

namespace Darklight.UnityExt.Library
{
    public class EnumObjectLibrary<TKey, TValue> : Library<TKey, TValue>
        where TKey : System.Enum
        where TValue : UnityEngine.Object
    {
        public override TValue CreateDefaultValue() => default(TValue);
    }
}