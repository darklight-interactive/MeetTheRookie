using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class TestReorderableList : MonoBehaviour
{

    [Serializable]
    public class TempClass
    {
        public List<string> myList = new List<string>();
    }

    public TempClass tempClass = new TempClass();
}

// Custom Property Drawer for the TempClass
[CustomPropertyDrawer(typeof(TestReorderableList.TempClass))]
public class MyListDrawer : PropertyDrawer
{
    private ReorderableList reorderableList;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Initialize ReorderableList if null
        if (reorderableList == null)
        {
            InitializeReorderableList(property);
        }

        // Draw the ReorderableList
        reorderableList.DoList(position);
    }

    private void InitializeReorderableList(SerializedProperty property)
    {
        SerializedProperty listProperty = property.FindPropertyRelative("myList");

        reorderableList = new ReorderableList(property.serializedObject, listProperty, true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "My Custom List Header");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);
        };

        reorderableList.elementHeightCallback = (int index) =>
        {
            return EditorGUIUtility.singleLineHeight + 6;
        };

        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            int index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            // Initialize new element if needed
            element.stringValue = "New Element";
        };

        reorderableList.onRemoveCallback = (ReorderableList list) =>
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this element?", "Yes", "No"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        };

        reorderableList.onSelectCallback = (ReorderableList list) =>
        {
            Debug.Log("Selected Element: " + list.index);
        };

        reorderableList.onReorderCallback = (ReorderableList list) =>
        {
            Debug.Log("List Reordered");
        };

        reorderableList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            if (isActive)
                GUI.Box(rect, "", "selectionRect");
        };
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return reorderableList == null ? base.GetPropertyHeight(property, label) : reorderableList.GetHeight();
    }
}

// Custom Editor to display TempClass in the Inspector
[CustomEditor(typeof(TestReorderableList))]
public class TestReorderableListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty tempClassProperty = serializedObject.FindProperty("tempClass");
        EditorGUILayout.PropertyField(tempClassProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
