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
        readonly List<string> PROP_BLACKLIST = new List<string> { "_items" };

        public static GUIStyle HeaderStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            fontStyle = FontStyle.Bold
        };


        ReorderableList _list;
        SerializedObject _serializedObject;
        SerializedProperty _property;
        SerializedProperty _itemsProperty;

        protected Type serializedObjectType => _serializedObject.targetObject.GetType();
        protected string serializedObjectName => serializedObjectType.Name;
        protected float singleLineHeight => EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
        protected float currentIndent => EditorGUI.indentLevel * INDENT_WIDTH;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // << INITIALIZATION >>
            if (_serializedObject == null)
                _serializedObject = property.serializedObject;
            if (_property == null)
                _property = property;
            if (_itemsProperty == null)
                _itemsProperty = property.FindPropertyRelative("_items");

            // Initialize variables for positioning
            Vector2 position = new Vector2(rect.x, rect.y);

            // << DRAW PROPERTY >>
            // Begin the property scope
            EditorGUI.BeginProperty(rect, label, property);

            DrawHeader(rect, out float headerHeight);
            position.y += headerHeight;

            Rect propRect = new Rect(position.x, position.y, rect.width, EditorGUIUtility.singleLineHeight);
            DrawProperties(propRect, out float propertiesHeight);
            position.y += propertiesHeight;

            // Draw the ReorderableList
            DrawReorderableList(position, rect.width, out float listHeight);

            EditorGUILayout.Space(singleLineHeight);

            // End the property scope
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Start with the height of the foldout
            float totalHeight = singleLineHeight;

            if (property.isExpanded)
            {
                // Increase indent level
                EditorGUI.indentLevel++;

                // Create copies to iterate through properties
                SerializedProperty iterator = property.Copy();
                SerializedProperty endProperty = iterator.GetEndProperty();

                iterator.NextVisible(true); // Move to the first child property

                // Iterate through all properties
                while (iterator.NextVisible(false) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    // Skip the '_items' property
                    if (iterator.name == "_items")
                        continue;

                    // Add the height of the property
                    totalHeight += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                }

                // Add the height of the ReorderableList
                InitializeReorderableList();
                totalHeight += _list.GetHeight() + EditorGUIUtility.standardVerticalSpacing;

                // Decrease indent level
                EditorGUI.indentLevel--;
            }

            return totalHeight;
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

        protected virtual void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty itemsProperty = _list.serializedProperty;
            SerializedProperty itemProp = itemsProperty.GetArrayElementAtIndex(index);
            SerializedProperty keyProp = itemProp.FindPropertyRelative("_key");
            SerializedProperty valueProp = itemProp.FindPropertyRelative("_value");

            rect.y += 2;
            float halfWidth = (rect.width - 10) / 2;

            Rect keyRect = new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(rect.x + halfWidth + 10, rect.y, halfWidth, EditorGUIUtility.singleLineHeight);

            if (keyProp == null)
                EditorGUI.LabelField(keyRect, "Null Key");
            else
                EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);

            if (keyProp == null)
                EditorGUI.LabelField(valueRect, "Null Value");
            else
                EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
        }

        protected virtual void DrawHeaderCallback(Rect rect)
        {
            float halfWidth = (rect.width - 10) / 2;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Key");
            EditorGUI.LabelField(new Rect(rect.x + halfWidth + 10, rect.y, halfWidth, EditorGUIUtility.singleLineHeight), "Value");
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


        void DrawHeader(Rect rect, out float headerHeight)
        {
            int btnWidth = 50;
            Vector2 position = new Vector2(rect.x, rect.y);

            Rect titleRect = rect;
            PropertyDrawerUtility.DrawLabel(rect, serializedObjectName, HeaderStyle, out float titleHeight);

            position.x = rect.x + rect.width;
            position.x -= btnWidth * 2;

            Rect btn1Rect = new Rect(position.x, position.y, btnWidth, singleLineHeight);
            PropertyDrawerUtility.DrawButton(btn1Rect, "Reset", out float resetButtonHeight, () =>
            {
                InvokeSerializedObjectMethod("Reset");
            });

            Rect btn2Rect = new Rect(position.x + btnWidth, position.y, btnWidth, singleLineHeight);
            PropertyDrawerUtility.DrawButton(btn2Rect, "Clear", out float clearButtonHeight, () =>
            {
                InvokeSerializedObjectMethod("Clear");
            });
            headerHeight = singleLineHeight;
        }

        void DrawReorderableList(Vector2 position, float width, out float listHeight)
        {
            InitializeReorderableList();

            // Calculate the height for the list
            listHeight = _list.GetHeight();

            // Draw the ReorderableList
            _list.DoList(new Rect(position.x, position.y, width, listHeight));
        }

        void DrawProperties(Rect rect, out float height)
        {
            float currentYPos = rect.y;

            // Create copies to iterate through properties
            SerializedProperty iterator = _property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            // Increase indent level for child properties
            EditorGUI.indentLevel++;

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