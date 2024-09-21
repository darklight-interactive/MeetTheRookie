using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using System.Linq;
using Darklight.UnityExt.Editor.Utility;

namespace Darklight.UnityExt.Library.Editor
{
    [CustomPropertyDrawer(typeof(Library<,>), true)]
    public class LibraryPropertyDrawer : PropertyDrawer
    {
        const float ELEMENT_PADDING = 10f;

        readonly List<string> PROP_BLACKLIST = new List<string> { "_items" };
        readonly float SINGLE_LINE_HEIGHT = EditorGUIUtility.singleLineHeight;
        readonly float VERTICAL_SPACING = EditorGUIUtility.singleLineHeight * 0.5f;

        readonly Color HEADER_COLOR_1 = Color.white * 0.1f;
        readonly Color COLUMN_COLOR_1 = Color.white * 0.2f;
        readonly Color HEADER_COLOR_2 = Color.white * 0.3f;
        readonly Color COLUMN_COLOR_2 = Color.white * 0.4f;

        readonly GUIStyle LABEL_STYLE = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };


        SerializedObject _serializedObject;
        SerializedProperty _libraryProperty;
        SerializedProperty _itemsProperty;
        SerializedProperty _readOnlyKeyProperty;
        SerializedProperty _readOnlyValueProperty;
        ReorderableList _list;

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

        float element_propHeight => SINGLE_LINE_HEIGHT;
        float element_fullHeight => element_propHeight + VERTICAL_SPACING;

        float itemID_ColumnStartX => _fullPropertyStartXPos;
        float itemID_PropX => itemID_ColumnStartX + ELEMENT_PADDING;
        float itemID_PropWidth = ELEMENT_PADDING * 2;
        float itemID_ColumnWidth => itemID_PropWidth + (ELEMENT_PADDING * 2);
        float itemID_ColumnEndX => itemID_ColumnStartX + itemID_ColumnWidth;

        float itemElement_PropWidth
        {
            get
            {
                float outWidth = _fullPropertyWidth;

                // Subract the full width of the ID Column
                outWidth -= itemID_ColumnWidth;

                // Subtract the KeyProp padding
                outWidth -= ELEMENT_PADDING * 2;

                // Subtract the ValueProp padding
                outWidth -= ELEMENT_PADDING * 2;

                // Divide the remaining width by 2
                outWidth /= 2;
                return outWidth;
            }
        }


        float itemKey_ColumnStartX => itemID_ColumnEndX;
        float itemKey_PropX => itemKey_ColumnStartX + ELEMENT_PADDING;
        float itemKey_PropWidth => itemElement_PropWidth;
        float itemKey_ColumnWidth => itemKey_PropWidth + ELEMENT_PADDING * 2;
        float itemKey_ColumnEndX => itemKey_ColumnStartX + itemKey_ColumnWidth;

        float itemValue_ColumnStartX => itemKey_ColumnEndX;
        float itemValue_PropX => itemValue_ColumnStartX + ELEMENT_PADDING;
        float itemValue_PropWidth => itemElement_PropWidth;
        float itemValue_ColumnWidth => itemValue_PropWidth + ELEMENT_PADDING * 2;
        float itemValue_ColumnEndX => itemValue_ColumnStartX + itemValue_ColumnWidth;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // << INITIALIZATION >>
            // Store the serialized object and properties
            if (_serializedObject == null)
                _serializedObject = property.serializedObject;
            if (_libraryProperty == null || _libraryProperty.propertyPath != property.propertyPath)
                _libraryProperty = property;
            if (_itemsProperty == null || _itemsProperty.propertyPath != property.FindPropertyRelative("_items").propertyPath)
                _itemsProperty = property.FindPropertyRelative("_items");

            if (_readOnlyKeyProperty == null
                || _readOnlyKeyProperty.propertyPath != property.FindPropertyRelative("_readOnlyKey").propertyPath)
                _readOnlyKeyProperty = property.FindPropertyRelative("_readOnlyKey");
            if (_readOnlyValueProperty == null
                || _readOnlyValueProperty.propertyPath != property.FindPropertyRelative("_readOnlyValue").propertyPath)
                _readOnlyValueProperty = property.FindPropertyRelative("_readOnlyValue");


            _fullPropertyStartXPos = rect.x;
            _fullPropertyWidth = rect.width;

            float currentYPos = rect.y;

            // << BEGIN PROPERTY SCOPE >> -------------------------------------
            // Begin the property scope
            EditorGUI.BeginProperty(rect, label, property);

            // << Get the Field Type and Generic Arguments >>
            Type libraryFieldType = fieldInfo.FieldType;
            string typeName = GetGenericTypeName(libraryFieldType);

            // ( Title )---------------------------------------------
            Rect titleRect = new Rect(rect.x, currentYPos, rect.width, SINGLE_LINE_HEIGHT);
            EditorGUI.LabelField(titleRect, $"{label} : {typeName}", new GUIStyle(EditorStyles.boldLabel));
            currentYPos += SINGLE_LINE_HEIGHT + VERTICAL_SPACING / 2;

            // ( Properties )-------------------------------------------------
            //Rect propRect = new Rect(rect.x, currentYPos, rect.width, SINGLE_LINE_HEIGHT);
            //DrawLibraryProperties(propRect, ref currentYPos);
            //currentYPos += VERTICAL_SPACING / 2;

            // ( ReorderableList )--------------------------------------------
            Rect listRect = new Rect(rect.x, currentYPos, rect.width, SINGLE_LINE_HEIGHT);
            DrawReorderableList(listRect, out float listHeight);
            currentYPos += listHeight;

            // (Calculate Property Height)-------------------------------------
            _fullPropertyHeight = currentYPos - rect.y;

            // End the property scope
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _fullPropertyHeight;
        }

        #region ======== [[ REORDERABLE LIST ]] ===================================== >>>>
        protected void InitializeReorderableList(SerializedProperty property)
        {
            if (_list == null || _list.serializedProperty != property.FindPropertyRelative("_items"))
            {
                _list = new ReorderableList(property.serializedObject, _itemsProperty, false, true, true, false);

                _list.drawHeaderCallback = DrawHeaderCallback;
                _list.drawElementCallback = DrawElementCallback;
                _list.drawElementBackgroundCallback = DrawElementBackgroundCallback;

                _list.elementHeightCallback = ElementHeightCallback;

                //_list.onAddCallback = OnAddCallback;
                _list.onRemoveCallback = OnRemoveCallback;

                _list.onAddDropdownCallback = OnAddDropdownCallback;
            }

        }

        void DrawReorderableList(Rect rect, out float listHeight)
        {
            InitializeReorderableList(_libraryProperty);

            // Calculate the height for the list
            listHeight = _list.GetHeight();

            // Draw the ReorderableList
            _list.DoList(new Rect(rect.x, rect.y, rect.width, listHeight));
        }


        void DrawHeaderCallback(Rect rect)
        {
            float headerHeight = rect.height;

            // << DRAW LABELS >> ------------------------------------- >>
            Rect idRect = new Rect(itemID_PropX, rect.y, itemID_PropWidth, headerHeight);
            Rect keyRect = new Rect(itemKey_PropX, rect.y, itemKey_PropWidth, headerHeight);
            Rect valueRect = new Rect(itemValue_PropX, rect.y, itemValue_PropWidth, headerHeight);
            EditorGUI.LabelField(idRect, "ID", LABEL_STYLE);
            EditorGUI.LabelField(keyRect, "Key");
            EditorGUI.LabelField(valueRect, "Value");

            // << DRAW BACKGROUND >> ------------------------------------- >>
            Rect idBkgRect = new Rect(itemID_ColumnStartX, rect.y, itemID_ColumnWidth, SINGLE_LINE_HEIGHT);
            Rect keyBkgRect = new Rect(itemKey_ColumnStartX, rect.y, itemKey_ColumnWidth, SINGLE_LINE_HEIGHT);
            Rect valueBkgRect = new Rect(itemValue_ColumnStartX, rect.y, itemValue_ColumnWidth, SINGLE_LINE_HEIGHT);
            EditorGUI.DrawRect(idBkgRect, HEADER_COLOR_1);
            EditorGUI.DrawRect(keyBkgRect, HEADER_COLOR_2);
            EditorGUI.DrawRect(valueBkgRect, HEADER_COLOR_1);
        }


        void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            bool readOnlyKey = _readOnlyKeyProperty.boolValue;
            bool readOnlyValue = _readOnlyValueProperty.boolValue;

            SerializedProperty itemProp = _itemsProperty.GetArrayElementAtIndex(index);
            SerializedProperty idProp = itemProp.FindPropertyRelative("_id");
            SerializedProperty keyProp = itemProp.FindPropertyRelative("_key");
            SerializedProperty valueProp = itemProp.FindPropertyRelative("_value");

            // << DRAW VALUES >> ------------------------------------- >>
            float elementY = rect.y;
            elementY += (element_fullHeight - element_propHeight) / 2; // Center the element vertically

            // Create rects for the properties
            Rect idRect = new Rect(itemID_PropX, elementY, itemID_PropWidth, element_propHeight);
            Rect keyRect = new Rect(itemKey_PropX, elementY, itemKey_PropWidth, element_propHeight);
            Rect valueRect = new Rect(itemValue_PropX, elementY, itemValue_PropWidth, element_propHeight);

            // Draw the properties
            EditorGUI.LabelField(idRect, idProp.intValue.ToString(), LABEL_STYLE);
            DrawElementProperty(keyRect, keyProp, readOnlyKey);
            DrawElementProperty(valueRect, valueProp, readOnlyValue);
        }

        void DrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = element_fullHeight;

            // << DRAW BACKGROUND >> ------------------------------------- >>
            float backgroundY = rect.y;
            float backgroundHeight = rect.height;

            Rect idBkgRect = new Rect(itemID_ColumnStartX, backgroundY, itemID_ColumnWidth, backgroundHeight);
            Rect keyBkgRect = new Rect(itemKey_ColumnStartX, backgroundY, itemKey_ColumnWidth, backgroundHeight);
            Rect valueBkgRect = new Rect(itemValue_ColumnStartX, backgroundY, itemValue_ColumnWidth, backgroundHeight);
            EditorGUI.DrawRect(idBkgRect, COLUMN_COLOR_1);
            EditorGUI.DrawRect(keyBkgRect, COLUMN_COLOR_2);
            EditorGUI.DrawRect(valueBkgRect, COLUMN_COLOR_1);
        }

        float ElementHeightCallback(int index) => element_fullHeight;

        void OnAddDropdownCallback(Rect rect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();

            // Custom icon
            GUIContent resetContent = new GUIContent("Reset", EditorGUIUtility.IconContent("d_Refresh").image);
            menu.AddItem(resetContent, false, () =>
            {
                InvokeLibraryMethod("Reset", out object result);
                _serializedObject.ApplyModifiedProperties();
            });

            menu.ShowAsContext();
        }


        void OnRemoveCallback(ReorderableList list)
        {
            SerializedProperty itemsProperty = list.serializedProperty;
            itemsProperty.DeleteArrayElementAtIndex(list.index);
            itemsProperty.serializedObject.ApplyModifiedProperties();
        }

        #endregion

        // ======== [[ DRAW METHODS ]] ===================================== >>>>
        void DrawElementProperty(Rect rect, SerializedProperty property, bool readOnly = false)
        {
            if (property == null)
            {
                EditorGUI.LabelField(rect, "Null Property");
                return;
            }

            if (readOnly)
            {
                // Convert simple types to strings
                if (SerializedPropertyUtility.IsSimpleType(property) && !SerializedPropertyUtility.IsReferenceType(property))
                {
                    EditorGUI.LabelField(rect, SerializedPropertyUtility.ConvertPropertyToString(property));
                    return;
                }

                // Draw the property as disabled
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(rect, property, GUIContent.none);
                EditorGUI.EndDisabledGroup();
                return;
            }
            else
            {
                EditorGUI.PropertyField(rect, property, GUIContent.none);
            }
        }

        void DrawLibraryProperties(Rect rect, ref float currentYPos)
        {
            // Get the Library<,> instance
            object libraryInstance = GetLibraryInstance(_libraryProperty);

            // If the Library<,> instance is not found, return
            if (libraryInstance == null)
            {
                return;
            }

            // Use reflection to draw properties of the Library<,> instance
            int count = 0;
            foreach (FieldInfo field in libraryInstance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // Skip fields that are not serialized
                if (field.IsNotSerialized) continue;

                object fieldValue = field.GetValue(libraryInstance);
                string label = ObjectNames.NicifyVariableName(field.Name);

                // Create a rect for the field
                float propertyHeight = EditorGUIUtility.singleLineHeight;
                Rect propertyRect = new Rect(rect.x, currentYPos, rect.width, propertyHeight);

                // Draw the field based on its type
                if (field.FieldType == typeof(int))
                {
                    int intValue = (int)fieldValue;
                    intValue = EditorGUI.IntField(propertyRect, label, intValue);
                    field.SetValue(libraryInstance, intValue);
                }
                else if (field.FieldType == typeof(float))
                {
                    float floatValue = (float)fieldValue;
                    floatValue = EditorGUI.FloatField(propertyRect, label, floatValue);
                    field.SetValue(libraryInstance, floatValue);
                }
                else if (field.FieldType == typeof(string))
                {
                    string stringValue = (string)fieldValue;
                    stringValue = EditorGUI.TextField(propertyRect, label, stringValue);
                    field.SetValue(libraryInstance, stringValue);
                }
                else if (field.FieldType == typeof(bool))
                {
                    bool boolValue = (bool)fieldValue;
                    boolValue = EditorGUI.Toggle(propertyRect, label, boolValue);
                    field.SetValue(libraryInstance, boolValue);
                }
                else
                {
                    // Skip fields that are not supported
                    continue;
                }

                // Increment y position
                currentYPos += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
                count++;
            }
        }


        // ======== [[ HELPER METHODS ]] ===================================== >>>>
        /// <summary>
        /// Gets the instance of the Library<,> for the current SerializedProperty.
        /// </summary>
        /// <param name="property">The SerializedProperty representing the Library<,> field.</param>
        /// <returns>The Library<,> instance, or null if not found.</returns>
        object GetLibraryInstance(SerializedProperty property)
        {
            // Get the target object (the script holding the Library<,> field)
            UnityEngine.Object targetObject = property.serializedObject.targetObject;
            Type targetType = targetObject.GetType();

            // Find the Library<,> field that matches the property
            foreach (FieldInfo field in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Library<,>))
                {
                    if (property.name == field.Name)
                    {
                        return field.GetValue(targetObject);
                    }
                }
            }

            Debug.LogWarning($"No matching Library<,> instance found for property '{property.name}' on type '{targetType}'.");
            return null;
        }



        void InvokeLibraryMethod(string methodName, out object returnValue, object[] parameters = null)
        {
            returnValue = null;

            SerializedProperty property = _libraryProperty;


            // Get the target object (the script holding the Library<,> field)
            UnityEngine.Object targetObject = property.serializedObject.targetObject;

            // Get the type of the target object (MonoBehaviour)
            Type targetType = targetObject.GetType();

            // Find the specific field that matches the SerializedProperty
            FieldInfo libraryField = targetType.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (libraryField != null)
            {
                // Get the instance of Library<,> (or its derived type) from the field
                object libraryInstance = libraryField.GetValue(targetObject);

                // Ensure that the instance is a subclass of LibraryBase
                if (libraryInstance != null && libraryInstance.GetType().IsSubclassOf(typeof(LibraryBase)))
                {
                    // Get the method information from the Library<,> instance
                    MethodInfo methodInfo = libraryInstance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (methodInfo != null)
                    {
                        // Invoke the method on the specific Library<,> type instance
                        returnValue = methodInfo.Invoke(libraryInstance, parameters);
                        property.serializedObject.ApplyModifiedProperties();
                        Debug.Log($"Method '{methodName}' called on {libraryInstance.GetType().Name}.");
                    }
                    else
                    {
                        Debug.LogWarning($"Method '{methodName}' not found on type '{libraryInstance.GetType().Name}'.");
                    }
                }
                else
                {
                    Debug.LogWarning($"The field '{property.name}' is not a Library<,> instance or its subclass.");
                }
            }
            else
            {
                Debug.LogWarning($"Field '{property.name}' not found on type '{targetType}'.");
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