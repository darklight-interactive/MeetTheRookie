using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;

namespace Darklight.UnityExt.Library.Editor
{
    [CustomPropertyDrawer(typeof(Library<,>), true)]
    public class LibraryPropertyDrawer : PropertyDrawer
    {
        const float INDENT_WIDTH = 15f;
        const float PADDING = 5f;
        readonly List<string> PROP_BLACKLIST = new List<string> { "_items" };

        ReorderableList _list;
        SerializedObject _serializedObject;
        SerializedProperty _libraryProperty;
        SerializedProperty _itemsProperty;
        bool _foldoutToggle;
        float _propertyHeight;

        protected Type libraryType => _libraryProperty.GetType();
        protected string libraryName => libraryType.Name;
        protected Type objectType => _serializedObject.targetObject.GetType();
        protected string objectName => objectType.Name;
        protected float singleLineHeight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
        protected float indentWidth => EditorGUI.indentLevel * INDENT_WIDTH;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // << INITIALIZATION >>
            if (_serializedObject == null)
                _serializedObject = property.serializedObject;
            if (_libraryProperty == null)
                _libraryProperty = property;
            if (_itemsProperty == null)
                _itemsProperty = property.FindPropertyRelative("_items");

            // << DRAW PROPERTY >>
            // Begin the property scope
            EditorGUI.BeginProperty(rect, label, property);

            Vector2 position = new Vector2(rect.x + indentWidth, rect.y);

            // ( Foldout Toggle )---------------------------------------------
            Rect foldoutRect = new Rect(position.x, position.y, rect.width, EditorGUIUtility.singleLineHeight);
            _foldoutToggle = EditorGUI.Foldout(foldoutRect, _foldoutToggle, label, true);
            position.y += EditorGUIUtility.singleLineHeight;

            if (_foldoutToggle)
            {
                // ( Properties )-------------------------------------------------
                Rect propRect = new Rect(position.x, position.y, rect.width, EditorGUIUtility.singleLineHeight);
                DrawProperties(propRect, out float propertiesHeight);
                position.y += propertiesHeight;

                // ( ReorderableList )--------------------------------------------
                EditorGUI.indentLevel++;
                position.x = rect.x + indentWidth;
                float listWidth = rect.width - (indentWidth * 2);
                Rect listRect = new Rect(position.x, position.y, listWidth, rect.height - position.y);
                DrawReorderableList(listRect, out float listHeight);
                position.y += listHeight;

                _propertyHeight = position.y - rect.y;
            }

            // End the property scope
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _foldoutToggle ? _propertyHeight : EditorGUIUtility.singleLineHeight;
        }

        #region ======== [[ REORDERABLE LIST ]] ===================================== >>>>
        protected void InitializeReorderableList()
        {
            if (_list == null || _list.serializedProperty != _itemsProperty)
            {
                _list = new ReorderableList(_serializedObject, _itemsProperty, false, true, true, true);
                _list.drawElementCallback = DrawElementCallback;
                _list.drawHeaderCallback = DrawHeaderCallback;
                _list.onAddCallback = OnAddCallback;
                _list.onRemoveCallback = OnRemoveCallback;
            }
        }

        protected virtual void DrawHeaderCallback(Rect rect)
        {
            float fullWidth = rect.width - (PADDING * 2);
            float idWidth = fullWidth / 4; // 25% of the full width
            float itemWidth = (fullWidth - idWidth) / 2; // Take 50% of the remaining width

            Rect idRect = new Rect(rect.x, rect.y, idWidth, EditorGUIUtility.singleLineHeight);
            Rect keyRect = new Rect(idRect.x + idWidth + PADDING, rect.y, itemWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(keyRect.x + itemWidth + PADDING, rect.y, itemWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(idRect, "ID");
            EditorGUI.LabelField(keyRect, "Key");
            EditorGUI.LabelField(valueRect, "Value");
        }

        protected virtual void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty itemsProperty = _list.serializedProperty;
            SerializedProperty itemProp = itemsProperty.GetArrayElementAtIndex(index);
            SerializedProperty idProp = itemProp.FindPropertyRelative("_id");
            SerializedProperty keyProp = itemProp.FindPropertyRelative("_key");
            SerializedProperty valueProp = itemProp.FindPropertyRelative("_value");

            rect.x += PADDING * 2;
            rect.y += 2;
            float fullWidth = rect.width - (PADDING * 2);
            float idWidth = fullWidth / 4; // 25% of the full width
            float itemWidth = (fullWidth - idWidth) / 2; // Take 50% of the remaining width

            Rect idRect = new Rect(rect.x + PADDING, rect.y, idWidth, EditorGUIUtility.singleLineHeight);
            Rect keyRect = new Rect(idRect.x + idWidth + PADDING, rect.y, itemWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(keyRect.x + itemWidth + PADDING, rect.y, itemWidth, EditorGUIUtility.singleLineHeight);

            // << DRAW INDEX >>
            EditorGUI.LabelField(idRect, idProp.intValue.ToString());

            // << DRAW KEY >>
            if (keyProp == null)
                EditorGUI.LabelField(keyRect, "Null Key");
            else
                EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);

            // << DRAW VALUE >>
            if (keyProp == null)
                EditorGUI.LabelField(valueRect, "Null Value");
            else
                EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
        }



        protected virtual void OnAddCallback(ReorderableList list)
        {
            /*
            SerializedProperty itemsProperty = list.serializedProperty;
            itemsProperty.arraySize++;
            list.index = itemsProperty.arraySize - 1;

            SerializedProperty newItem = itemsProperty.GetArrayElementAtIndex(list.index);
            SerializedProperty keyProp = newItem.FindPropertyRelative("_key");
            SerializedProperty valueProp = newItem.FindPropertyRelative("_value");

            // Initialize the new KeyValuePair with default values
            if (keyProp != null)
                InitializeProperty(keyProp);
            InitializeProperty(valueProp);
            */

            InvokeSerializedObjectMethod("AddDefaultItem");

            _serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnRemoveCallback(ReorderableList list)
        {
            SerializedProperty itemsProperty = list.serializedProperty;
            itemsProperty.DeleteArrayElementAtIndex(list.index);
            itemsProperty.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        // ======== [[ DRAW METHODS ]] ===================================== >>>>
        /*
        void DrawHeader(Rect rect, out float headerHeight)
        {
            int btnWidth = 50;
            Vector2 position = new Vector2(rect.x, rect.y);

            EditorGUI.indentLevel++;
            position.x += GetCurrentIndentValue();
            Rect btn1Rect = new Rect(position.x, position.y, btnWidth, singleLineHeight);
            PropertyDrawerUtility.DrawButton(btn1Rect, "Reset", out float resetButtonHeight, () =>
            {
                InvokeSerializedObjectMethod("Reset");
            });

            Rect btn2Rect = new Rect(btn1Rect.x + btnWidth, position.y, btnWidth, singleLineHeight);
            PropertyDrawerUtility.DrawButton(btn2Rect, "Clear", out float clearButtonHeight, () =>
            {
                InvokeSerializedObjectMethod("Clear");
            });
            position.y += singleLineHeight;

            headerHeight = position.y - rect.y;
        }
        */

        void DrawProperties(Rect rect, out float height)
        {
            float currentYPos = rect.y;

            // Create copies to iterate through properties
            SerializedProperty iterator = _libraryProperty.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            // Iterate through the properties of the object
            iterator.NextVisible(true); // Move to the first child property

            while (iterator.NextVisible(false) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                // Skip any properties in the blacklist
                if (PROP_BLACKLIST.Contains(iterator.name))
                    continue;

                // Calculate the height for the property
                float propertyHeight = EditorGUI.GetPropertyHeight(iterator, true);

                // Draw the property field
                EditorGUI.PropertyField(new Rect(rect.x, currentYPos, rect.width, propertyHeight), iterator, true);

                // Increment y position
                currentYPos += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            height = currentYPos - rect.y;
        }

        void DrawReorderableList(Rect rect, out float listHeight)
        {
            InitializeReorderableList();

            // Calculate the height for the list
            listHeight = _list.GetHeight();

            // Draw the ReorderableList
            _list.DoList(new Rect(rect.x, rect.y, rect.width, listHeight));
        }

        // ======== [[ HELPER METHODS ]] ===================================== >>>>

        void InvokeSerializedObjectMethod(string methodName)
        {
            // Get the target object
            UnityEngine.Object targetObject = _serializedObject.targetObject;

            // Check if the method exists on the target object and invoke it
            MethodInfo methodInfo = targetObject.GetType().GetMethod(methodName);
            if (methodInfo != null)
            {
                methodInfo.Invoke(targetObject, null);
                _serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning($"Method '{methodName}' not found on type '{targetObject.GetType()}'.");
            }
        }

        private void InvokeSerializedObjectMethod(string methodName, object[] parameters)
        {
            UnityEngine.Object targetObject = _serializedObject.targetObject;

            // Get the method information, matching the number and types of parameters
            MethodInfo methodInfo = targetObject.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (methodInfo != null)
            {
                // Invoke the method with the provided parameters
                methodInfo.Invoke(targetObject, parameters);
                _serializedObject.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning($"Method '{methodName}' with specified parameters not found on type '{targetObject.GetType()}'.");
            }
        }

        void InitializeProperty(SerializedProperty property)
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
}