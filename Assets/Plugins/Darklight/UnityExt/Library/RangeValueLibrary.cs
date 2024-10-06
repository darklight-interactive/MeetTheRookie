using System;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Library;
using UnityEngine;

public abstract class RangeValueLibrary<TKey, TValue> : Library<TKey, TValue>
    where TKey : notnull
    where TValue : struct
{

}

public class IntRangeValueLibrary<TKey> : RangeValueLibrary<TKey, int>
    where TKey : notnull
{

}

public class FloatRangeValueLibrary<TKey> : RangeValueLibrary<TKey, float>
    where TKey : notnull
{

}
