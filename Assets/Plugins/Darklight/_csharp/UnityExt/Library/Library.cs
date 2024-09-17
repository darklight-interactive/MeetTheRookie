using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditorInternal;
using Darklight.UnityExt.Editor;
using System.Linq;
using UnityEngine.PlayerLoop;







#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Library
{
    public static class LibraryUtility
    {
        public const string OBJECT_LIBRARY_PATH = "Darklight/ObjectLibrary/";
        public const string PRIMITIVE_PATH = OBJECT_LIBRARY_PATH + "Primitive/";
        public const string KEY_VALUE_PATH = OBJECT_LIBRARY_PATH + "KeyValue/";
    }

    #region == (( CLASS : Library<TKey, TValue> )) ============================================= ))
    [System.Serializable]
    public class Library<TKey, TValue> : ILibrary<TKey, TValue>, ISerializationCallbackReceiver
        where TKey : notnull
        where TValue : notnull
    {
        // ======== [[ FIELDS ]] ===================================== >>>>
        Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        [SerializeField] protected List<SerializableKeyValuePair> _items = new List<SerializableKeyValuePair>();

        // ======== [[ EVENTS ]] ===================================== >>>>
        public event EventHandler<ItemAddedEventArgs<TKey, TValue>> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs<TKey>> ItemRemoved;

        // ======== [[ PROPERTIES ]] ================================== >>>>
        #region -- (( IDictionary<TKey, TValue> )) --
        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException($"Key '{key}' not found in the library.");
                }
            }
            set
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }
        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        #endregion

        public List<SerializableKeyValuePair> Items => _items;

        // ======== [[ CONSTRUCTORS ]] ===================================== >>>>
        public Library()
        {
            _items = new List<SerializableKeyValuePair>();
        }

        // ======== [[ METHODS ]] ===================================== >>>>        
        #region -- (( IDictionary<TKey, TValue> )) --
        public void Add(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
                return;

            _dictionary.Add(key, value);
            _items.Add(new SerializableKeyValuePair(key, value));
            OnItemAdded(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out TValue value))
            {
                bool removed = _dictionary.Remove(key);
                if (removed)
                {
                    OnItemRemoved(key);
                }
                return removed;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(_dictionary[item.Key], item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        #endregion

        #region -- (( ISerializationCallbackReceiver )) --
        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            _dictionary.Clear();
            foreach (var entry in _items)
            {
                if (!_dictionary.ContainsKey(entry.Key))
                {
                    _dictionary.Add(entry.Key, entry.Value);
                }
                else
                {
                    Debug.LogWarning($"Duplicate key '{entry.Key}' found during deserialization. Skipping.");
                }
            }
        }


        #endregion

        public void Add(SerializableKeyValuePair item)
        {
            if (!_dictionary.ContainsKey(item.Key))
            {
                _dictionary.Add(item.Key, item.Value);
                _items.Add(item);
                OnItemAdded(item.Key, item.Value);
            }
            else
            {
                Debug.LogWarning($"Key '{item.Key}' already exists in the library. Skipping.");
            }
        }

        public virtual TValue CreateDefaultValue()
        {
            return default;
        }


        public virtual void AddKeys(IEnumerable<TKey> keys)
        {
            foreach (TKey key in keys)
            {
                if (!_dictionary.ContainsKey(key))
                {
                    Add(key, CreateDefaultValue());
                }
                else
                {
                    if (this[key] == null || this[key].Equals(default(TValue)))
                    {
                        this[key] = CreateDefaultValue();
                    }
                }
            }
        }

        public void RemoveAllKeysExcept(IEnumerable<TKey> keys)
        {
            List<TKey> keysToRemove = new List<TKey>();
            foreach (TKey key in _dictionary.Keys)
            {
                if (!keys.Contains(key))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (TKey key in keysToRemove)
            {
                Remove(key);
            }
        }

        public void RebuildLibrary()
        {
            _dictionary.Clear();
            foreach (var item in _items)
            {
                _dictionary.Add(item.Key, item.Value);
            }
        }

        // Event Invokers
        protected virtual void OnItemAdded(TKey key, TValue value)
        {
            ItemAdded?.Invoke(this, new ItemAddedEventArgs<TKey, TValue>(key, value));
        }

        protected virtual void OnItemRemoved(TKey key)
        {
            ItemRemoved?.Invoke(this, new ItemRemovedEventArgs<TKey>(key));
        }


        // ======== [[ NESTED CLASEES ]] ===================================== >>>>
        [System.Serializable]
        public class SerializableKeyValuePair
        {
            public TKey Key;
            public TValue Value;
            public SerializableKeyValuePair() { }
            public SerializableKeyValuePair(TKey key) { Key = key; }
            public SerializableKeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

    }
    #endregion

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(Library<,>), true)]
    public class LibraryPropertyDrawer : PropertyDrawer
    {
        private ReorderableList _list;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty itemsProperty = property.FindPropertyRelative("_items");

            // Initialize the ReorderableList if necessary
            if (_list == null)
            {
                _list = new ReorderableList(property.serializedObject, itemsProperty, true, true, true, true);
                _list.drawElementCallback = DrawElementCallback;
                _list.drawHeaderCallback = DrawHeaderCallback;
                _list.onAddCallback = OnAddCallback;
                _list.onRemoveCallback = OnRemoveCallback;
            }

            // Update the serialized object and ReorderableList
            _list.DoList(position);
            property.serializedObject.Update();
            property.serializedObject.ApplyModifiedProperties();

            // Add some space at the bottom
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 2);
            EditorGUI.EndProperty();
        }

        private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty itemsProperty = _list.serializedProperty;
            SerializedProperty itemProp = itemsProperty.GetArrayElementAtIndex(index);
            SerializedProperty keyProp = itemProp.FindPropertyRelative("Key");
            SerializedProperty valueProp = itemProp.FindPropertyRelative("Value");

            rect.y += 2;
            float halfWidth = rect.width / 2 - 5;

            Rect keyRect = new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(rect.x + halfWidth + 10, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
        }

        private void DrawHeaderCallback(Rect rect)
        {
            float halfWidth = rect.width / 2 - 5;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Key");
            EditorGUI.LabelField(new Rect(rect.x + halfWidth + 10, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Value");
        }

        private void OnAddCallback(ReorderableList list)
        {
            SerializedProperty itemsProperty = list.serializedProperty;
            itemsProperty.arraySize++;
            list.index = itemsProperty.arraySize - 1;

            SerializedProperty newItem = itemsProperty.GetArrayElementAtIndex(list.index);
            SerializedProperty keyProp = newItem.FindPropertyRelative("Key");
            SerializedProperty valueProp = newItem.FindPropertyRelative("Value");

            // Initialize the new KeyValuePair with default values
            InitializeProperty(keyProp);
            InitializeProperty(valueProp);

            itemsProperty.serializedObject.ApplyModifiedProperties();
        }

        private void OnRemoveCallback(ReorderableList list)
        {
            SerializedProperty itemsProperty = list.serializedProperty;
            itemsProperty.DeleteArrayElementAtIndex(list.index);
            itemsProperty.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_list != null)
            {
                return _list.GetHeight();
            }
            return base.GetPropertyHeight(property, label);
        }

        private void InitializeProperty(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = default;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = default;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = default;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = string.Empty;
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = Color.white;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.LayerMask:
                    property.intValue = default;
                    break;
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = 0;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = Vector4.zero;
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = new Rect();
                    break;
                case SerializedPropertyType.ArraySize:
                    property.intValue = default;
                    break;
                case SerializedPropertyType.Character:
                    property.intValue = default;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = new AnimationCurve();
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = new Bounds();
                    break;
                case SerializedPropertyType.Gradient:
                    // Gradient cannot be set directly
                    break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = Quaternion.identity;
                    break;
                case SerializedPropertyType.ExposedReference:
                    property.exposedReferenceValue = null;
                    break;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = Vector2Int.zero;
                    break;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = Vector3Int.zero;
                    break;
                case SerializedPropertyType.RectInt:
                    property.rectIntValue = new RectInt();
                    break;
                case SerializedPropertyType.BoundsInt:
                    property.boundsIntValue = new BoundsInt();
                    break;
                case SerializedPropertyType.ManagedReference:
                    property.managedReferenceValue = null;
                    break;
                default:
                    // For any other property types, set to default or null
                    property.managedReferenceValue = null;
                    break;
            }
        }
    }
#endif

    /*

    #if UNITY_EDITOR
        [CustomEditor(typeof(Library<,>), true)]
        public class ScriptableLibraryEditor : UnityEditor.Editor
        {
            protected SerializedProperty keysProperty;
            protected SerializedProperty valuesProperty;
            protected ReorderableList reorderableList;
            protected ILibrary library;

            protected virtual void OnEnable()
            {
                library = (ILibrary)target;
                if (library != null)
                {
                    library.AddDefaultItem();
                    Debug.Log("Added default item");
                }

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
                keysProperty.InsertArrayElementAtIndex(index);
                valuesProperty.InsertArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                //Debug.Log($"OnAddCallback: inserting at index {index}");
            }

            protected virtual void OnRemoveCallback(ReorderableList list)
            {
                keysProperty.DeleteArrayElementAtIndex(list.index);
                valuesProperty.DeleteArrayElementAtIndex(list.index);
                serializedObject.ApplyModifiedProperties();
                //Debug.Log($"OnRemoveCallback: removing at index {list.index}");
            }
    #endif
        }
        */
}