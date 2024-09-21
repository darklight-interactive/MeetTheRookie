







using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif


using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ReorderableList<type>
{
    public List<type> list_;
}

[Serializable]
public class IntReorderableList : ReorderableList<int>
{
}

public class ReorderableListTest : MonoBehaviour
{
    [ReorderableList] public IntReorderableList data_;
}

public class ReorderableListAttribute : PropertyAttribute { }



#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
public class ReorderableListDrawer : PropertyDrawer
{
    private ReorderableList list_;

    public void OnEnable()
    {
    }

    public override void OnGUI(Rect rect, SerializedProperty serializedProperty, GUIContent label)
    {
        SerializedProperty listProperty = serializedProperty.FindPropertyRelative("list_");
        ReorderableList list = GetList(listProperty);

        float height = 0f;
        for (var i = 0; i < listProperty.arraySize; i++)
        {
            height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
        }
        list.elementHeight = height;
        list.DoList(rect);
    }

    public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label)
    {
        SerializedProperty listProperty = serializedProperty.FindPropertyRelative("list_");
        return GetList(listProperty).GetHeight();
    }

    private ReorderableList GetList(SerializedProperty serializedProperty)
    {
        if (list_ == null)
        {
            list_ = new ReorderableList(serializedProperty.serializedObject, serializedProperty);
        }

        return list_;
    }
}

#endif