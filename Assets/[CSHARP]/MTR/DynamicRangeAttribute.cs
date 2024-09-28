using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif

/* ==== EXAMPLE USAGE ==== 
public class DynamicRangeSlider : MonoBehaviour
{
    public Vector2 range = new Vector2(0f, 100f);

    [DynamicRange("range")]
    public float dynamicSlider;
}
*/

namespace Darklight.UnityExt.Editor
{

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class DynamicRangeAttribute : PropertyAttribute
    {
        public string Vector2FieldName { get; private set; }
        public DynamicRangeAttribute(string vector2FieldName)
        {
            Vector2FieldName = vector2FieldName;
        }
    }

    [CustomPropertyDrawer(typeof(DynamicRangeAttribute))]
    public class DynamicRangeDrawer : PropertyDrawer
    {

        const float HORZ_PADDING = 5f;
        readonly GUIStyle rangeValueStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.UpperCenter,
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0)
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DynamicRangeAttribute rangeAttribute = (DynamicRangeAttribute)attribute;

            // Get the parent object of the property
            object parentObject = GetParentObject(property.propertyPath, property.serializedObject.targetObject);
            if (parentObject != null)
            {
                // Get Vector2 field value from parent object
                Vector2 range = GetVector2Value(rangeAttribute.Vector2FieldName, parentObject);

                float nameLabelWidth = EditorGUIUtility.labelWidth;
                float rangeValueWidth = 35f; // Adjust width for range values
                float sliderPadding = 2f; // Padding between the slider and range labels
                float sliderWidth = position.width - nameLabelWidth - (rangeValueWidth * 2) - (sliderPadding * 2);

                Rect nameLabelRect = new Rect(position.x, position.y, nameLabelWidth, position.height);
                Rect minRangeRect = new Rect(nameLabelRect.x + nameLabelWidth, position.y, rangeValueWidth, position.height);
                Rect sliderRect = new Rect(minRangeRect.x + rangeValueWidth + sliderPadding, position.y, sliderWidth, position.height);
                Rect maxRangeRect = new Rect(sliderRect.x + sliderWidth + sliderPadding, position.y, rangeValueWidth, position.height);

                // Draw the property label
                EditorGUI.LabelField(nameLabelRect, label.text);

                // Draw min and max range values at the ends of the slider
                EditorGUI.LabelField(minRangeRect, $"{range.x}", rangeValueStyle);
                EditorGUI.LabelField(maxRangeRect, $"{range.y}", rangeValueStyle);

                // Draw the slider
                DrawSlider(sliderRect, property, range);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Invalid DynamicRange attribute usage.");
            }
        }

        void DrawSlider(Rect sliderRect, SerializedProperty property, Vector2 range)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = EditorGUI.Slider(sliderRect, GUIContent.none, property.floatValue, range.x, range.y);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.IntSlider(sliderRect, GUIContent.none, property.intValue, (int)range.x, (int)range.y);
            }
            else
            {
                EditorGUI.LabelField(sliderRect, "Use DynamicRange with float or int.");
                return;
            }
        }

        private Vector2 GetVector2Value(string fieldName, object targetObject)
        {
            if (targetObject == null)
            {
                Debug.LogError("DynamicRangeAttribute: Target object is null.");
                return Vector2.zero;
            }

            Type type = targetObject.GetType();
            FieldInfo field = type.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            PropertyInfo property = type.GetProperty(fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (field != null && field.FieldType == typeof(Vector2))
            {
                return (Vector2)field.GetValue(targetObject);
            }
            else if (property != null && property.PropertyType == typeof(Vector2))
            {
                return (Vector2)property.GetValue(targetObject);
            }

            Debug.LogError($"DynamicRangeAttribute: Could not find Vector2 field or property '{fieldName}' on '{targetObject.GetType()}'.");
            return Vector2.zero;
        }


        // Helper method to get the parent object of the property
        private object GetParentObject(string path, object obj)
        {
            if (obj == null)
            {
                Debug.LogError("DynamicRangeAttribute: Parent object is null.");
                return null;
            }

            string[] fields = path.Split('.');

            foreach (var field in fields.Take(fields.Length - 1))
            {
                var type = obj.GetType();
                FieldInfo fieldInfo = type.GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (fieldInfo != null)
                {
                    obj = fieldInfo.GetValue(obj);
                    if (obj == null)
                    {
                        Debug.LogError($"DynamicRangeAttribute: Field '{field}' is null on object of type '{type}'.");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"DynamicRangeAttribute: Could not find field '{field}' on object of type '{type}'.");
                    return null;
                }
            }

            return obj;
        }
    }
}
