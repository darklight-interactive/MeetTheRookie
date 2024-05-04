using System;
using UnityEngine;
using UnityEditor;

namespace Darklight.UnityExt.CustomEditor
{
    public class ShowOnlyAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
    public class ShowOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
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
            else
            {
                EditorGUI.LabelField(position, label.text, valueStr);
            }
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
                    return "(not supported)";
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
}
