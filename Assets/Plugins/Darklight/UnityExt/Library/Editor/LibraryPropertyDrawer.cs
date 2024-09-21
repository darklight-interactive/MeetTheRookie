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
        const float ITEM_PROP_SPACING = 10f;
        readonly List<string> PROP_BLACKLIST = new List<string> { "_items" };
        readonly float SINGLE_LINE_HEIGHT = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        SerializedObject _serializedObject;
        SerializedProperty _libraryProperty;
        SerializedProperty _itemsProperty;

        /// <summary>
        /// The full height of the property drawer. <br/>
        /// Calculated at the end of the OnGUI method and returned in GetPropertyHeight.
        /// </summary>
        float _fullPropertyHeight;

        /// <summary>
        /// The full width of the property drawer. <br/>
        /// Calculated at the beginning of the OnGUI method as the width of the rect.
        /// </summary>
        float _fullPropertyWidth;
        float _fullPropertyStartXPos;

        ReorderableList _list;
        float list_itemID_XPos => _fullPropertyStartXPos + ITEM_PROP_SPACING;
        float list_itemID_Width => ITEM_PROP_SPACING * 2;
        float list_itemProp_Width
        {
            get
            {
                float outWidth = _fullPropertyWidth;

                // Subract the width of the ID and the surrounding spacing
                float id_columnWidth = ITEM_PROP_SPACING + list_itemID_Width + ITEM_PROP_SPACING;
                outWidth -= id_columnWidth;

                // Subtract the right-side spacing of two properties
                outWidth -= ITEM_PROP_SPACING * 2;
                return outWidth / 2; // Divide the remaining width by 2 to return the width of each individual property
            }
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // << INITIALIZATION >>
            // Store the serialized object and properties
            if (_serializedObject == null)
                _serializedObject = property.serializedObject;
            if (_libraryProperty == null)
                _libraryProperty = property;
            if (_itemsProperty == null)
                _itemsProperty = property.FindPropertyRelative("_items");

            _fullPropertyStartXPos = rect.x;
            _fullPropertyWidth = rect.width;

            // << BEGIN PROPERTY SCOPE >> -------------------------------------
            // Begin the property scope
            EditorGUI.BeginProperty(rect, label, property);

            Vector2 position = new Vector2(rect.x, rect.y);

            // << Get the Field Type and Generic Arguments >>
            Type libraryFieldType = fieldInfo.FieldType;
            string typeName = GetGenericTypeName(libraryFieldType);

            // ( Title )---------------------------------------------
            Rect titleRect = new Rect(position.x, position.y, rect.width, SINGLE_LINE_HEIGHT);
            EditorGUI.LabelField(titleRect, $"{label} : {typeName}", new GUIStyle(EditorStyles.boldLabel));
            position.y += SINGLE_LINE_HEIGHT;

            // ( Properties )-------------------------------------------------
            Rect propRect = new Rect(position.x, position.y, rect.width, SINGLE_LINE_HEIGHT);
            DrawProperties(propRect, out float propertiesHeight);
            position.y += propertiesHeight;

            // ( ReorderableList )--------------------------------------------
            Rect listRect = new Rect(position.x, position.y, rect.width, SINGLE_LINE_HEIGHT);
            DrawReorderableList(listRect, out float listHeight);
            position.y += listHeight;

            // (Calculate Property Height)-------------------------------------
            _fullPropertyHeight = position.y - rect.y;

            // End the property scope
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _fullPropertyHeight;
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
            Rect idRect = new Rect(list_itemID_XPos, rect.y, list_itemID_Width, SINGLE_LINE_HEIGHT);

            Rect keyRect = new Rect(idRect.x + list_itemID_Width + ITEM_PROP_SPACING, rect.y, list_itemProp_Width, SINGLE_LINE_HEIGHT);

            Rect valueRect = new Rect(keyRect.x + list_itemProp_Width + ITEM_PROP_SPACING, rect.y, list_itemProp_Width, SINGLE_LINE_HEIGHT);

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

            Rect idRect = new Rect(list_itemID_XPos, rect.y, list_itemID_Width, SINGLE_LINE_HEIGHT);

            Rect keyRect = new Rect(idRect.x + list_itemID_Width + ITEM_PROP_SPACING, rect.y, list_itemProp_Width, SINGLE_LINE_HEIGHT);

            Rect valueRect = new Rect(keyRect.x + list_itemProp_Width + ITEM_PROP_SPACING, rect.y, list_itemProp_Width, SINGLE_LINE_HEIGHT);

            // << DRAW INDEX >>
            EditorGUI.LabelField(idRect, idProp.intValue.ToString());

            // << DRAW KEY >>
            if (keyProp == null)
                EditorGUI.LabelField(keyRect, "Null Key");
            else
                EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);

            // << DRAW VALUE >>
            if (valueProp == null)
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
            Rect btn1Rect = new Rect(position.x, position.y, btnWidth, SINGLE_LINE_HEIGHT);
            PropertyDrawerUtility.DrawButton(btn1Rect, "Reset", out float resetButtonHeight, () =>
            {
                InvokeSerializedObjectMethod("Reset");
            });

            Rect btn2Rect = new Rect(btn1Rect.x + btnWidth, position.y, btnWidth, SINGLE_LINE_HEIGHT);
            PropertyDrawerUtility.DrawButton(btn2Rect, "Clear", out float clearButtonHeight, () =>
            {
                InvokeSerializedObjectMethod("Clear");
            });
            position.y += SINGLE_LINE_HEIGHT;

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

        string GetGenericTypeName(Type type)
        {
            if (type == null)
                return "Unknown Type";

            // Get the generic type arguments
            Type[] genericArgs = type.GetGenericArguments();

            // Remove the backtick and number from the type name if it exists
            string baseTypeName = type.Name;
            int backtickIndex = baseTypeName.IndexOf('`');
            if (backtickIndex > 0)
            {
                baseTypeName = baseTypeName.Substring(0, backtickIndex);
            }

            // If the type has generic arguments, format them
            if (genericArgs.Length > 0)
            {
                string genericArgsString = string.Join(", ", Array.ConvertAll(genericArgs, t => t.Name));
                return $"{baseTypeName}<{genericArgsString}>";
            }

            // Return the cleaned-up type name
            return baseTypeName;
        }

    }
}