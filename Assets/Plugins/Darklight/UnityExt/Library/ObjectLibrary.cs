using System;
using System.Collections.Generic;
using UnityEngine;

namespace Darklight.UnityExt.Library
{
    [Serializable]
    public class EnumObjectLibrary<TEnum, TObj> : EnumKeyLibrary<TEnum, TObj>
        where TEnum : System.Enum
        where TObj : UnityEngine.Object
    {
        public EnumObjectLibrary() { }
        public EnumObjectLibrary(bool defaultToAllKeys = false) : base(defaultToAllKeys) { }
    }

    [Serializable]
    public class EnumGameObjectLibrary<TEnum> : EnumObjectLibrary<TEnum, GameObject>
        where TEnum : System.Enum
    {
        public EnumGameObjectLibrary() { }
        public TComponent TryGetComponent<TComponent>(TEnum key) where TComponent : Component
        {
            if (TryGetValue(key, out GameObject obj))
            {
                if (obj != null)
                {
                    return obj.GetComponent<TComponent>();
                }
            }

            return null;
        }
    }

    [Serializable]
    public class EnumComponentLibrary<TEnum, TComponent> : EnumObjectLibrary<TEnum, TComponent>
        where TEnum : System.Enum
        where TComponent : Component
    {

    }
}