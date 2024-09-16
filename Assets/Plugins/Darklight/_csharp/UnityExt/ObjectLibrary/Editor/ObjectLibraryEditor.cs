using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Darklight.UnityExt.ObjectLibrary.Editor
{
    [CustomEditor(typeof(ObjectLibrary<>), true)]
    public class ObjectLibraryEditorBase : UnityEditor.Editor
    {
        protected SerializedProperty keysProperty;
        protected SerializedProperty objectsProperty;
        protected ReorderableList reorderableList;

        protected virtual void OnEnable()
        {
            keysProperty = serializedObject.FindProperty("_keys");
            objectsProperty = serializedObject.FindProperty("_objects");

            reorderableList = new ReorderableList(serializedObject, keysProperty, true, true, true, true);

            reorderableList.drawElementCallback = DrawElementCallback;
            reorderableList.drawHeaderCallback = DrawHeaderCallback;
            reorderableList.onAddCallback = OnAddCallback;
            reorderableList.onRemoveCallback = OnRemoveCallback;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the ReorderableList
            reorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect keyRect = new Rect(rect.x, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight);

            SerializedProperty keyProp = keysProperty.GetArrayElementAtIndex(index);
            SerializedProperty objProp = objectsProperty.GetArrayElementAtIndex(index);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                // Check for duplicate keys
                for (int i = 0; i < keysProperty.arraySize; i++)
                {
                    if (i != index && SerializedProperty.DataEquals(keyProp, keysProperty.GetArrayElementAtIndex(i)))
                    {
                        EditorUtility.DisplayDialog("Duplicate Key", "Keys must be unique.", "OK");
                        break;
                    }
                }
            }
            EditorGUI.PropertyField(valueRect, objProp, GUIContent.none);
        }

        private void DrawHeaderCallback(Rect rect)
        {
            float halfWidth = rect.width / 2 - 5;
            Rect keyHeaderRect = new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);
            Rect valueHeaderRect = new Rect(rect.x + halfWidth + 10, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(keyHeaderRect, "Key");
            EditorGUI.LabelField(valueHeaderRect, "Object");
        }

        protected virtual void OnAddCallback(ReorderableList list)
        {
            int index = list.count;
            keysProperty.InsertArrayElementAtIndex(index);
            objectsProperty.InsertArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            //Debug.Log($"OnAddCallback: inserting at index {index}");
        }

        protected virtual void OnRemoveCallback(ReorderableList list)
        {
            keysProperty.DeleteArrayElementAtIndex(list.index);
            objectsProperty.DeleteArrayElementAtIndex(list.index);
            serializedObject.ApplyModifiedProperties();
            //Debug.Log($"OnRemoveCallback: removing at index {list.index}");
        }


    }
}

