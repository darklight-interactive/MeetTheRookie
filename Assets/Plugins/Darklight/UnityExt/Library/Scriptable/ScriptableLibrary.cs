using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using Darklight.UnityExt.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Darklight.UnityExt.Library
{
    public abstract class ScriptableLibrary<TLibrary> : ScriptableObject
        where TLibrary : ILibrary
    {
        [SerializeField] TLibrary _library = default(TLibrary);

        public TLibrary Library => _library;

    }
}
