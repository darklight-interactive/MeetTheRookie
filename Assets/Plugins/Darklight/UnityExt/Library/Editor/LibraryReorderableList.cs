using UnityEngine;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif


namespace Darklight.UnityExt.Library.Editor
{
#if UNITY_EDITOR
    public class LibraryReorderableList : ReorderableList
    {
        readonly float ELEMENT_PROP_HEIGHT = EditorGUIUtility.singleLineHeight;
        readonly float ELEMENT_HEIGHT = EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2);
        public const int COLUMN_COUNT = 3;
        public readonly float[] COLUMN_PERCENTAGES = new float[] { 0.025f };

        private FieldInfo _fieldInfo;  // Store the FieldInfo
        private SerializedProperty _itemsProperty;
        private SerializedProperty _readOnlyKeyProperty;
        private SerializedProperty _readOnlyValueProperty;

        // GUI constants
        private readonly float ELEMENT_PADDING = 10f;
        private readonly GUIStyle CENTERED_LABEL_STYLE = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };

        readonly Color HEADER_COLOR_1 = Color.white * 0.4f;
        readonly Color COLUMN_COLOR_1 = Color.white * 0.4f;
        readonly Color HEADER_COLOR_2 = Color.white * 0.6f;
        readonly Color COLUMN_COLOR_2 = Color.white * 0.6f;

        const bool DRAGGABLE = false;
        private int _columnCount = 3;
        private float[] _columnPercentages;

        public LibraryReorderableList(SerializedObject serializedObject, SerializedProperty itemsProperty, SerializedProperty readOnlyKeyProperty, SerializedProperty readOnlyValueProperty, FieldInfo fieldInfo)
            : base(serializedObject, itemsProperty, DRAGGABLE, true, true, true)
        {
            _fieldInfo = fieldInfo;  // Store the FieldInfo
            _itemsProperty = itemsProperty;
            _readOnlyKeyProperty = readOnlyKeyProperty;
            _readOnlyValueProperty = readOnlyValueProperty;

            this._columnPercentages = ValidateAndNormalizePercentages(COLUMN_COUNT, COLUMN_PERCENTAGES);

            headerHeight = ELEMENT_HEIGHT;
            elementHeight = ELEMENT_HEIGHT;

            drawHeaderCallback = DrawHeader;
            drawElementCallback = DrawElement;
            elementHeightCallback = GetElementHeight;
            onRemoveCallback = OnRemoveElement;
            drawElementBackgroundCallback = DrawElementBackground;
        }

        public void DrawList(Rect rect)
        {
            DoList(rect);
        }

        private void DrawHeader(Rect rect)
        {
            float[] columnWidths = CalculateColumnWidths(rect.width, _columnCount);

            if (draggable) rect.x += ELEMENT_PADDING + 5;

            Rect idRect = CalculatePropertyRect(rect, 0, columnWidths);
            Rect keyRect = CalculatePropertyRect(rect, 1, columnWidths);
            Rect valueRect = CalculatePropertyRect(rect, 2, columnWidths);
            if (draggable) valueRect.x -= ELEMENT_PADDING;

            EditorGUI.LabelField(idRect, "ID", CENTERED_LABEL_STYLE);
            EditorGUI.LabelField(keyRect, "Key");
            EditorGUI.LabelField(valueRect, "Value");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty itemProp = _itemsProperty.GetArrayElementAtIndex(index);
            SerializedProperty idProp = itemProp.FindPropertyRelative("_id");
            SerializedProperty keyProp = itemProp.FindPropertyRelative("_key");
            SerializedProperty valueProp = itemProp.FindPropertyRelative("_value");

            float[] columnWidths = CalculateColumnWidths(rect.width, _columnCount);

            Rect idRect = CalculatePropertyRect(rect, 0, columnWidths);
            Rect keyRect = CalculatePropertyRect(rect, 1, columnWidths);
            Rect valueRect = CalculatePropertyRect(rect, 2, columnWidths);

            // Begin change check for this element
            EditorGUI.BeginChangeCheck();

            EditorGUI.LabelField(idRect, idProp.intValue.ToString(), CENTERED_LABEL_STYLE);
            DrawElementProperty(keyRect, keyProp, _readOnlyKeyProperty.boolValue);
            DrawElementProperty(valueRect, valueProp, _readOnlyValueProperty.boolValue);

            // End change check and apply changes
            if (EditorGUI.EndChangeCheck())
            {
                ApplyChanges();
                Debug.Log("LibraryReorderableList: Element modified");
            }
        }

        public void ApplyChanges()
        {
            // Ensure the modified properties are serialized
            _itemsProperty.serializedObject.ApplyModifiedProperties();

            // Mark the target object dirty to ensure the changes are saved
            EditorUtility.SetDirty(_itemsProperty.serializedObject.targetObject);

            // Force the editor to repaint both the inspector and the scene
            _itemsProperty.serializedObject.UpdateIfRequiredOrScript();
            _itemsProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorApplication.QueuePlayerLoopUpdate(); // Updates Scene view if necessary
            EditorWindow.focusedWindow?.Repaint();
        }


        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isActive)
            {
                EditorGUI.DrawRect(rect, new Color(0.24f, 0.49f, 0.90f, 0.3f));
            }
            else if (isFocused)
            {
                EditorGUI.DrawRect(rect, new Color(0.24f, 0.49f, 0.90f, 0.1f));
            }
        }

        private float GetElementHeight(int index) => ELEMENT_HEIGHT;

        private void OnRemoveElement(ReorderableList list)
        {
            _itemsProperty.DeleteArrayElementAtIndex(list.index);
            _itemsProperty.serializedObject.ApplyModifiedProperties();
        }

        private void DrawElementProperty(Rect rect, SerializedProperty property, bool readOnly = false)
        {
            if (readOnly)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(rect, property, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                DrawValueField(rect, property);
            }
        }

        private void DrawValueField(Rect rect, SerializedProperty valueProperty)
        {

            if (valueProperty.propertyType == SerializedPropertyType.Integer)
            {
                valueProperty.intValue = EditorGUI.IntField(rect, valueProperty.intValue);
            }
            else if (valueProperty.propertyType == SerializedPropertyType.Float)
            {
                valueProperty.floatValue = EditorGUI.FloatField(rect, valueProperty.floatValue);
            }
            else if (valueProperty.propertyType == SerializedPropertyType.Vector2)
            {
                valueProperty.vector2Value = EditorGUI.Vector2Field(rect, GUIContent.none, valueProperty.vector2Value);
            }
            else if (valueProperty.propertyType == SerializedPropertyType.Vector3)
            {
                valueProperty.vector3Value = EditorGUI.Vector3Field(rect, GUIContent.none, valueProperty.vector3Value);
            }
            else if (valueProperty.propertyType == SerializedPropertyType.Vector4)
            {
                valueProperty.vector4Value = EditorGUI.Vector4Field(rect, GUIContent.none, valueProperty.vector4Value);
            }
            else
            {
                EditorGUI.PropertyField(rect, valueProperty, GUIContent.none);
            }
        }


        // Method to calculate the rect for a specific property based on its row and column
        private Rect CalculatePropertyRect(Rect baseElementRect, int column, float[] columnWidths)
        {
            float xPosition = baseElementRect.x + ELEMENT_PADDING;
            for (int i = 0; i < column; i++)
            {
                xPosition += columnWidths[i] + ELEMENT_PADDING;
            }

            float yPosition = baseElementRect.y + (ELEMENT_HEIGHT - ELEMENT_PROP_HEIGHT) / 2;
            return new Rect(xPosition, yPosition, columnWidths[column], ELEMENT_PROP_HEIGHT);
        }

        // Dynamic column width calculation
        private float[] CalculateColumnWidths(float totalWidth, int numColumns)
        {
            float totalPadding = ELEMENT_PADDING * (numColumns + 1);
            float availableWidth = totalWidth - totalPadding;
            return _columnPercentages.Select(p => availableWidth * p).ToArray();
        }

        // Validation and normalization of percentages
        private float[] ValidateAndNormalizePercentages(int expectedColumns, float[] percentages)
        {
            // Initialize an array to store the final percentages
            float[] finalPercentages = new float[expectedColumns];

            // Calculate the total of the provided percentages
            float providedPercentageTotal = 0f;
            int providedCount = 0;
            for (int i = 0; i < expectedColumns; i++)
            {
                if (percentages != null && i < percentages.Length && percentages[i] > 0f)
                {
                    finalPercentages[i] = percentages[i];
                    providedPercentageTotal += percentages[i];
                    providedCount++;
                }
            }

            // Calculate the remaining percentage for unset columns
            float remainingPercentage = 1f - providedPercentageTotal;
            float evenPercentage = providedCount < expectedColumns ? remainingPercentage / (expectedColumns - providedCount) : 0f;

            // Assign the even percentage to unset columns
            for (int i = 0; i < expectedColumns; i++)
            {
                if (finalPercentages[i] == 0f)
                {
                    finalPercentages[i] = evenPercentage;
                }
            }

            return finalPercentages;
        }
    }
#endif
}
