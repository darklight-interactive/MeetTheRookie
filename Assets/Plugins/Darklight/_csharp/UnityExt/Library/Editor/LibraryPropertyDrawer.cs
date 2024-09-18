using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Darklight.UnityExt.Library.Editor
{
    [CustomPropertyDrawer(typeof(Library<,>), true)]
    public class LibraryPropertyDrawer : PropertyDrawer
    {
        protected ReorderableList _list;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin the property scope
            EditorGUI.BeginProperty(position, label, property);

            // Create copies to iterate through properties
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            // Initialize variables for positioning
            float y = position.y;
            float indent = EditorGUI.indentLevel * 15f;

            // Draw the foldout
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);
            y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (property.isExpanded)
            {
                // Increase indent level for child properties
                EditorGUI.indentLevel++;

                // Iterate through the properties of the object
                iterator.NextVisible(true); // Move to the first child property

                while (iterator.NextVisible(false) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    // Skip the '_items' property since we'll handle it with the ReorderableList
                    if (iterator.name == "_items")
                        continue;

                    // Calculate the height for the property
                    float propertyHeight = EditorGUI.GetPropertyHeight(iterator, true);

                    // Draw the property field
                    EditorGUI.PropertyField(new Rect(position.x, y, position.width, propertyHeight), iterator, true);

                    // Increment y position
                    y += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                // Ensure the ReorderableList is initialized
                SerializedProperty itemsProperty = property.FindPropertyRelative("_items");
                InitializeReorderableList(itemsProperty, property.serializedObject);

                // Calculate the height for the list
                float listHeight = _list.GetHeight();

                // Draw the ReorderableList
                _list.DoList(new Rect(position.x, y, position.width, listHeight));
                y += listHeight + EditorGUIUtility.standardVerticalSpacing;

                // Decrease indent level back
                EditorGUI.indentLevel--;
            }

            // End the property scope
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Start with the height of the foldout
            float totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

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
                SerializedProperty itemsProperty = property.FindPropertyRelative("_items");
                InitializeReorderableList(itemsProperty, property.serializedObject);
                totalHeight += _list.GetHeight() + EditorGUIUtility.standardVerticalSpacing;

                // Decrease indent level
                EditorGUI.indentLevel--;
            }

            return totalHeight;
        }

        private void InitializeReorderableList(SerializedProperty itemsProperty, SerializedObject serializedObject)
        {
            if (_list == null || _list.serializedProperty != itemsProperty)
            {
                _list = new ReorderableList(serializedObject, itemsProperty, false, true, true, true);
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

            itemsProperty.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnRemoveCallback(ReorderableList list)
        {
            SerializedProperty itemsProperty = list.serializedProperty;
            itemsProperty.DeleteArrayElementAtIndex(list.index);
            itemsProperty.serializedObject.ApplyModifiedProperties();
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
}