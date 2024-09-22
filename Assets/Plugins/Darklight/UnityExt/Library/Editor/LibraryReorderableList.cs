using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System.Collections.Generic;

namespace Darklight.UnityExt.Library.Editor
{
    public class LibraryReorderableList : ReorderableList
    {
        public readonly float ELEMENT_HEIGHT = EditorGUIUtility.singleLineHeight;
        public const int COLUMN_COUNT = 3;
        public readonly float[] COLUMN_PERCENTAGES = new float[] { 0.025f };

        private SerializedProperty _itemsProperty;
        private SerializedProperty _readOnlyKeyProperty;
        private SerializedProperty _readOnlyValueProperty;

        // GUI constants
        private readonly float ELEMENT_PADDING = 10f;
        private readonly GUIStyle LABEL_STYLE = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };

        readonly Color HEADER_COLOR_1 = Color.white * 0.4f;
        readonly Color COLUMN_COLOR_1 = Color.white * 0.4f;
        readonly Color HEADER_COLOR_2 = Color.white * 0.6f;
        readonly Color COLUMN_COLOR_2 = Color.white * 0.6f;

        private int columnCount => 3;
        private float[] columnPercentages;

        public LibraryReorderableList(SerializedObject serializedObject, SerializedProperty itemsProperty, SerializedProperty readOnlyKeyProperty, SerializedProperty readOnlyValueProperty)
            : base(serializedObject, itemsProperty, true, true, true, true)
        {
            _itemsProperty = itemsProperty;
            _readOnlyKeyProperty = readOnlyKeyProperty;
            _readOnlyValueProperty = readOnlyValueProperty;

            this.columnPercentages = ValidateAndNormalizePercentages(COLUMN_COUNT, COLUMN_PERCENTAGES);

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
            float[] columnWidths = CalculateColumnWidths(rect.width, columnCount);

            if (draggable) rect.x += ELEMENT_PADDING + 4;

            Rect idRect = CalculatePropertyRect(rect, 0, 0, columnWidths);
            Rect keyRect = CalculatePropertyRect(rect, 0, 1, columnWidths);
            Rect valueRect = CalculatePropertyRect(rect, 0, 2, columnWidths);
            if (draggable) valueRect.x -= ELEMENT_PADDING;

            EditorGUI.LabelField(idRect, "ID");
            EditorGUI.LabelField(keyRect, "Key");
            EditorGUI.LabelField(valueRect, "Value");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty itemProp = _itemsProperty.GetArrayElementAtIndex(index);
            SerializedProperty idProp = itemProp.FindPropertyRelative("_id");
            SerializedProperty keyProp = itemProp.FindPropertyRelative("_key");
            SerializedProperty valueProp = itemProp.FindPropertyRelative("_value");

            float[] columnWidths = CalculateColumnWidths(rect.width, columnCount);

            Rect idRect = CalculatePropertyRect(rect, index, 0, columnWidths);
            Rect keyRect = CalculatePropertyRect(rect, index, 1, columnWidths);
            Rect valueRect = CalculatePropertyRect(rect, index, 2, columnWidths);

            EditorGUI.LabelField(idRect, idProp.intValue.ToString());
            DrawElementProperty(keyRect, keyProp, _readOnlyKeyProperty.boolValue);
            DrawElementProperty(valueRect, valueProp, _readOnlyValueProperty.boolValue);
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.height = ELEMENT_HEIGHT;

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
                EditorGUI.PropertyField(rect, property, GUIContent.none);
            }
        }

        // Method to calculate the rect for a specific property based on its row and column
        private Rect CalculatePropertyRect(Rect baseRect, int row, int column, float[] columnWidths)
        {
            float xPosition = baseRect.x + ELEMENT_PADDING;

            for (int i = 0; i < column; i++)
            {
                xPosition += columnWidths[i] + ELEMENT_PADDING;
            }

            float yPosition = baseRect.y + row;

            return new Rect(xPosition, yPosition, columnWidths[column], ELEMENT_HEIGHT);
        }

        // Dynamic column width calculation
        private float[] CalculateColumnWidths(float totalWidth, int numColumns)
        {
            float totalPadding = ELEMENT_PADDING * (numColumns + 1);
            float availableWidth = totalWidth - totalPadding;
            return columnPercentages.Select(p => availableWidth * p).ToArray();
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
}
