using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Darklight.UnityExt.Editor
{
    public static class PropertyDrawerUtility
    {
        public static float SingleLineHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public static void DrawLabel(Rect rect, string label, GUIStyle style, out float height)
        {
            EditorGUI.LabelField(rect, label, style);
            height = rect.height;
        }

        public static void DrawButton(Rect rect, string label, out float btnHeight, Action onClick)
        {
            if (GUI.Button(rect, label))
            {
                onClick?.Invoke();
            }
            btnHeight = rect.height;
        }
    }
}
