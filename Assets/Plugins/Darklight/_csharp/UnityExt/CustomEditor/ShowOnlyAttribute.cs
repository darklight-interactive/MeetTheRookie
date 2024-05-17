using System;
using UnityEngine;
using UnityEditor;

namespace Darklight.UnityExt.Editor
{
#if UNITY_EDITOR    
    public class ShowOnlyAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
    public class ShowOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            string valueStr = GetValueString(prop);

            if (prop.propertyType == SerializedPropertyType.ArraySize)
            {
                EditorGUI.LabelField(position, label.text, $"Array size: {prop.arraySize}");
                position.y += EditorGUIUtility.singleLineHeight;

                EditorGUI.indentLevel++;
                for (int i = 0; i < prop.arraySize; i++)
                {
                    SerializedProperty item = prop.GetArrayElementAtIndex(i);
                    string itemValueStr = GetValueString(item);
                    EditorGUI.LabelField(position, $"Element {i}", itemValueStr);
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                EditorGUI.indentLevel--;
            }
            else if (prop.propertyType == SerializedPropertyType.Generic && IsSerializableClass(prop))
            {
                DrawSerializableClass(position, prop, label);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, valueStr);
            }

            EditorGUI.EndProperty();
        }

        private bool IsSerializableClass(SerializedProperty prop)
        {
            return prop.hasVisibleChildren && prop.depth == 0; // Simple check to see if it's a complex type
        }

        private void DrawSerializableClass(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.LabelField(position, label.text, "Class Object");
            position.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.indentLevel++;
            SerializedProperty childProp = prop.Copy();
            bool enteredChildren = false;
            while (childProp.NextVisible(enteredChildren))
            {
                if (!enteredChildren)
                {
                    enteredChildren = true;
                }
                else if (childProp.depth == 0)
                {
                    break;
                }

                EditorGUI.LabelField(position, childProp.displayName, GetValueString(childProp));
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            EditorGUI.indentLevel--;
        }

        private string GetValueString(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return prop.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return prop.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return prop.floatValue.ToString("0.00000");
                case SerializedPropertyType.String:
                    return prop.stringValue;
                case SerializedPropertyType.Enum:
                    return prop.enumDisplayNames[prop.enumValueIndex];
                case SerializedPropertyType.ObjectReference:
                    return GetObjectReferenceString(prop);
                case SerializedPropertyType.Vector2:
                    return prop.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return prop.vector3Value.ToString();
                case SerializedPropertyType.Vector2Int:
                    return prop.vector2IntValue.ToString();
                case SerializedPropertyType.Vector3Int:
                    return prop.vector3IntValue.ToString();
                case SerializedPropertyType.Quaternion:
                    return prop.quaternionValue.ToString();
                default:
                    return "[ShowOnly] Unsupported field type";
            }
        }

        private string GetObjectReferenceString(SerializedProperty prop)
        {
            if (prop.objectReferenceValue == null)
                return "None";

            if (prop.objectReferenceValue is MonoBehaviour)
                return $"MonoBehaviour: {prop.objectReferenceValue.name}";
            if (prop.objectReferenceValue is SceneAsset)
                return $"Scene: {prop.objectReferenceValue.name}";
            if (prop.objectReferenceValue is ScriptableObject)
                return $"ScriptableObject: {prop.objectReferenceValue.name}";

            return prop.objectReferenceValue.ToString();
        }
    }
#endif
}
