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
    public class ScriptableLibrary<TKey, TValue> : ScriptableObject, ILibrary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        public Library<TKey, TValue> _library = new Library<TKey, TValue>();

        public TValue this[TKey key] { get => _library[key]; set => _library[key] = value; }

        public ICollection<TKey> Keys => _library.Keys;
        public ICollection<TValue> Values => _library.Values;

        public int Count => _library.Count;
        public bool IsReadOnly => _library.IsReadOnly;
        public TValue DefaultValue => _library.DefaultValue;

        public event EventHandler<ItemAddedEventArgs<TKey, TValue>> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs<TKey>> ItemRemoved;

        // ======== [[ METHODS ]] ===================================== >>>>
        #region == (( Library<> Methods )) =====================================  ))
        public void Add(TKey key, TValue value) => _library.Add(key, value);
        public void Add(KeyValuePair<TKey, TValue> item) => _library.Add(item);
        public void Clear() => _library.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => _library.Contains(item);
        public bool ContainsKey(TKey key) => _library.ContainsKey(key);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _library.CopyTo(array, arrayIndex);
        public bool Remove(TKey key) => _library.Remove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => _library.Remove(item);
        public bool TryGetValue(TKey key, out TValue value) => _library.TryGetValue(key, out value);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _library.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _library.GetEnumerator();
        #endregion

        public void AddDefaultItem() => _library.AddDefaultItem();
        public TKey CreateDefaultKey() => _library.CreateDefaultKey();
        public TValue CreateDefaultValue() => _library.CreateDefaultValue();
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableLibrary<,>), true)]
    public class ScriptableLibraryEditor : UnityEditor.Editor
    {
        protected SerializedProperty keysProperty;
        protected SerializedProperty valuesProperty;
        protected ReorderableList reorderableList;

        protected virtual void OnEnable()
        {
            keysProperty = serializedObject.FindProperty("_keys");
            valuesProperty = serializedObject.FindProperty("_values");

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

            // Draw the rest of the inspector
            CustomInspectorGUI.DrawHorizontalLine(Color.grey);
            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect keyRect = new Rect(rect.x, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(rect.x + rect.width / 2 + 5, rect.y, rect.width / 2 - 5, EditorGUIUtility.singleLineHeight);

            SerializedProperty keyProp = keysProperty.GetArrayElementAtIndex(index);
            SerializedProperty objProp = valuesProperty.GetArrayElementAtIndex(index);

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
            if (keysProperty != null)
                keysProperty.InsertArrayElementAtIndex(index);
            valuesProperty.InsertArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            //Debug.Log($"OnAddCallback: inserting at index {index}");
        }

        protected virtual void OnRemoveCallback(ReorderableList list)
        {
            if (keysProperty != null)
                keysProperty.DeleteArrayElementAtIndex(list.index);
            valuesProperty.DeleteArrayElementAtIndex(list.index);
            serializedObject.ApplyModifiedProperties();
            //Debug.Log($"OnRemoveCallback: removing at index {list.index}");
        }
#endif
    }
}