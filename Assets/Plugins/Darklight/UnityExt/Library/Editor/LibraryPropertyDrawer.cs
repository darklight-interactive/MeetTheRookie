using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Darklight.UnityExt.Library.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Library<,>), true)]
    public class LibraryPropertyDrawer : PropertyDrawer
    {
        const string ITEMS_PROP = "_items";
        const string READ_ONLY_KEY = "_readOnlyKey";
        const string READ_ONLY_VALUE = "_readOnlyValue";

        readonly float SINGLE_LINE_HEIGHT = EditorGUIUtility.singleLineHeight;
        readonly float VERTICAL_SPACING = EditorGUIUtility.singleLineHeight * 0.5f;

        readonly GUIStyle LABEL_STYLE = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };


        SerializedObject _serializedObject;
        SerializedProperty _libraryProperty;
        SerializedProperty _itemsProperty;
        SerializedProperty _readOnlyKeyProperty;
        SerializedProperty _readOnlyValueProperty;
        LibraryReorderableList _list;

        bool _foldout;
        float _fullPropertyHeight;


        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // << INITIALIZATION >>
            // Store the serialized object and properties
            if (_serializedObject == null)
                _serializedObject = property.serializedObject;
            if (_libraryProperty == null || _libraryProperty.propertyPath != property.propertyPath)
                _libraryProperty = property;
            if (_itemsProperty == null || _itemsProperty.propertyPath != property.FindPropertyRelative(ITEMS_PROP).propertyPath)
                _itemsProperty = property.FindPropertyRelative(ITEMS_PROP);

            if (_readOnlyKeyProperty == null
                || _readOnlyKeyProperty.propertyPath != property.FindPropertyRelative(READ_ONLY_KEY).propertyPath)
                _readOnlyKeyProperty = property.FindPropertyRelative(READ_ONLY_KEY);
            if (_readOnlyValueProperty == null
                || _readOnlyValueProperty.propertyPath != property.FindPropertyRelative(READ_ONLY_VALUE).propertyPath)
                _readOnlyValueProperty = property.FindPropertyRelative(READ_ONLY_VALUE);

            // Initialize the ReorderableList
            if (_list == null)
            {
                _list = new LibraryReorderableList(_serializedObject, _itemsProperty, _readOnlyKeyProperty, _readOnlyValueProperty, fieldInfo);

                _list.onChangedCallback += (list) =>
                {
                    property.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                    Debug.Log($"Library<,> items changed.", property.serializedObject.targetObject);
                };

                _list.onAddDropdownCallback = (rect, list) =>
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Item"), false, () =>
                    {
                        InvokeLibraryMethod("AddDefaultItem", out object returnValue);
                    });
                    menu.AddItem(new GUIContent("Reset Library"), false, () =>
                    {
                        InvokeLibraryMethod("Reset", out object returnValue);
                    });
                    menu.AddItem(new GUIContent("Clear Library"), false, () =>
                    {
                        InvokeLibraryMethod("Clear", out object returnValue);
                    });
                    menu.ShowAsContext();
                };

                _list.onRemoveCallback = (list) =>
                {
                    InvokeLibraryMethod("RemoveAt", out object returnValue, new object[] { list.index });
                };

                _list.drawNoneElementCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "No items in library.", LABEL_STYLE);
                };
            }

            float currentYPos = rect.y;

            // << BEGIN PROPERTY SCOPE >> -------------------------------------
            // Begin the property scope
            EditorGUI.BeginProperty(rect, label, property);

            // << Get the Field Type and Generic Arguments >>
            Type libraryFieldType = fieldInfo.FieldType;
            string typeName = GetGenericTypeName(libraryFieldType);

            int itemCount = _itemsProperty.arraySize;

            // ( Foldout Title )---------------------------------------------
            Rect titleRect = new Rect(rect.x, currentYPos, rect.width, SINGLE_LINE_HEIGHT);
            EditorGUI.LabelField(titleRect, $"{label} : {typeName} : Count: {itemCount}", new GUIStyle(EditorStyles.boldLabel));
            currentYPos += SINGLE_LINE_HEIGHT + VERTICAL_SPACING / 2;

            // ( Properties )-------------------------------------------------
            EditorGUI.BeginDisabledGroup(true);
            Rect propRect = new Rect(rect.x, currentYPos, rect.width, SINGLE_LINE_HEIGHT);
            EditorGUI.PropertyField(propRect, _readOnlyKeyProperty, true);
            currentYPos += SINGLE_LINE_HEIGHT;
            propRect.y = currentYPos;

            EditorGUI.PropertyField(propRect, _readOnlyValueProperty, true);
            currentYPos += SINGLE_LINE_HEIGHT + VERTICAL_SPACING;
            EditorGUI.EndDisabledGroup();

            // ( ReorderableList )--------------------------------------------
            Rect listRect = new Rect(EditorGUI.IndentedRect(rect).x, currentYPos, EditorGUI.IndentedRect(rect).width, SINGLE_LINE_HEIGHT);
            _list.DrawList(listRect);
            currentYPos += _list.GetHeight() + VERTICAL_SPACING;

            // (Calculate Property Height)-------------------------------------
            _fullPropertyHeight = currentYPos - rect.y;

            // End the property scope
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _fullPropertyHeight;
        }

        private void DrawPropertyField(Rect rect, SerializedProperty valueProperty)
        {
            if (valueProperty.propertyType == SerializedPropertyType.Integer)
            {
                // Example range for int
                int minValue = 0;
                int maxValue = 100;
                valueProperty.intValue = EditorGUI.IntSlider(rect, valueProperty.intValue, minValue, maxValue);
            }
            else if (valueProperty.propertyType == SerializedPropertyType.Float)
            {
                // Example range for float
                float minValue = 0f;
                float maxValue = 1f;
                valueProperty.floatValue = EditorGUI.Slider(rect, valueProperty.floatValue, minValue, maxValue);
            }
            else
            {
                // Fallback to default property field for other types
                EditorGUI.PropertyField(rect, valueProperty, GUIContent.none);
            }
        }


        /*
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
        */

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

            // Get the type of the target object (MonoBehaviour or its subclass)
            Type targetType = targetObject.GetType();

            // Traverse the class hierarchy to find the field if it's inherited
            FieldInfo libraryField = null;
            Type currentType = targetType;

            // Traverse class hierarchy to look for the field that matches the SerializedProperty
            while (currentType != null && libraryField == null)
            {
                libraryField = currentType.GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                currentType = currentType.BaseType;
            }

            if (libraryField != null)
            {
                // Get the instance of LibraryBase (or its derived type) from the field
                object libraryInstance = libraryField.GetValue(targetObject);

                if (libraryInstance != null && typeof(LibraryBase).IsAssignableFrom(libraryInstance.GetType()))
                {
                    // Get the method information from the Library<,> or its subclass instance
                    MethodInfo methodInfo = libraryInstance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (methodInfo != null)
                    {
                        try
                        {
                            // Invoke the method on the specific LibraryBase or its subclass instance
                            returnValue = methodInfo.Invoke(libraryInstance, parameters);

                            // Apply modified properties to ensure they are serialized
                            property.serializedObject.ApplyModifiedProperties();

                            // Mark the target object as dirty to ensure the changes are saved and visible
                            EditorUtility.SetDirty(targetObject);

                            // Force the editor to repaint the Inspector
                            property.serializedObject.UpdateIfRequiredOrScript();
                            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                            EditorApplication.QueuePlayerLoopUpdate(); // Repaints Scene view if necessary
                            Debug.Log($"Method '{methodName}' called on {targetObject.name}" +
                                $" with LibraryBase instance '{libraryInstance.GetType().Name}'.", targetObject);

                            // Force a repaint of the inspector window
                            EditorWindow.focusedWindow?.Repaint();
                        }
                        catch (TargetInvocationException e)
                        {
                            Debug.LogError($"Error invoking method '{methodName}': {e.InnerException?.Message}", targetObject);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Method '{methodName}' not found on type '{libraryInstance.GetType().Name}'.");
                    }
                }
                else
                {
                    Debug.LogWarning($"The field '{property.name}' is not a LibraryBase instance or its subclass.");
                }
            }
            else
            {
                Debug.LogWarning($"Field '{property.name}' not found on type '{targetType}' or its base classes.");
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
#endif
}