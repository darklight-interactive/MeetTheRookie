using System.Collections.Generic;
using UnityEngine;
using Darklight.UnityExt.ObjectLibrary.Editor;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.ObjectLibrary
{
    public static class ObjectLibraryPath
    {
        public const string OBJECT_LIBRARY_PATH = "Darklight/ObjectLibrary/";
        public const string PRIMITIVE_PATH = OBJECT_LIBRARY_PATH + "Primitive/";
        public const string KEY_VALUE_PATH = OBJECT_LIBRARY_PATH + "KeyValue/";
    }



    [CreateAssetMenu(menuName = ObjectLibraryPath.PRIMITIVE_PATH + "SpriteLibrary")]
    public class SpriteLibrary : ObjectLibrary<Sprite> { }

    [CreateAssetMenu(menuName = ObjectLibraryPath.KEY_VALUE_PATH + "StringObjectLibrary")]
    public class StringObjectLibrary : ObjectLibrary<string, Object> { }

}