using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Darklight.UnityExt.Editor;

namespace Darklight.UnityExt.Library.Editor
{
    /*
    public class ReadonlyKeyLibraryPropertyDrawer : LibraryPropertyDrawer
    {
        // Override the method that draws each element of the ReorderableList
        protected override void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty itemsProperty = _list.serializedProperty;
            SerializedProperty itemProp = itemsProperty.GetArrayElementAtIndex(index);
            SerializedProperty keyProp = itemProp.FindPropertyRelative("_key");
            SerializedProperty valueProp = itemProp.FindPropertyRelative("_value");

            rect.y += 2;
            float halfWidth = rect.width / 2 - 5;

            Rect keyRect = new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(rect.x + halfWidth + 10, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);

            // Display the key as a label
            string keyLabel = CustomInspectorGUI.ConvertElementToString(keyProp);
            EditorGUI.LabelField(keyRect, keyLabel);

            // Still allow the value to be editable
            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
        }
    }
    */
}
