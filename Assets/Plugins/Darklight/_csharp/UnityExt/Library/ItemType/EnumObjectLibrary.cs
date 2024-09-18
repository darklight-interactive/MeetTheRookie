using System;
using UnityEngine;

namespace Darklight.UnityExt.Library
{
    [Serializable]
    public class EnumObjectLibrary<TEnum, TObj> : EnumKeyLibrary<TEnum, TObj>
        where TEnum : System.Enum
        where TObj : UnityEngine.Object
    {
        [SerializeField] TObj _defaultObject = default(TObj);
        public override TObj CreateDefaultValue()
        {
            return _defaultObject;
        }
    }
}