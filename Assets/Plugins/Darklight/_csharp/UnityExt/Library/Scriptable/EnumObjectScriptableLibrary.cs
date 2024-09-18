using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Darklight.UnityExt.Library
{
    public class EnumObjectScriptableLibrary<TEnum, TObj> : ScriptableLibrary<EnumObjectLibrary<TEnum, TObj>>
        where TEnum : System.Enum
        where TObj : UnityEngine.Object
    { }
}