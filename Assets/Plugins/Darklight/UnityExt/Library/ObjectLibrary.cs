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

        public EnumObjectLibrary() : base(false, false, false) { }
        public EnumObjectLibrary(bool containAllKeyValues) : base(containAllKeyValues, true, false) { }
        public EnumObjectLibrary(bool containAllKeyValues, bool readOnlyKey, bool readOnlyValue) : base(containAllKeyValues, readOnlyKey, readOnlyValue) { }

        public override TEnum CreateDefaultKey()
        {
            return default(TEnum);
        }

        public override TObj CreateDefaultValue()
        {
            return _defaultObject;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}