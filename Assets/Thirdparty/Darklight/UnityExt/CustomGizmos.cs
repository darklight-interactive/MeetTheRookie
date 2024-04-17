using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Darklight.UnityExt
{
    public static class CustomGizmos
    {
        public static void DrawLabel(string label, Vector3 position, GUIStyle labelStyle)
        {
            //labelStyle.normal = new GUIStyleState { textColor = color }; // Set the text color
            Handles.Label(position, label, labelStyle);
        }

        public static void DrawWireSquare(Vector3 position, float size, Vector3 direction, Color color)
        {
            if (color == null)
            {
                Handles.color = Color.black;
            }
            else { Handles.color = color; }
            Handles.DrawSolidRectangleWithOutline(GetRectangleVertices(position, size * Vector2.one, direction), Color.clear, color);
        }

        public static void DrawWireSquare_withLabel(string label, Vector3 position, float size, Vector3 direction, Color color, GUIStyle labelStyle)
        {

            DrawWireSquare(position, size, direction, color);

            labelStyle.normal = new GUIStyleState { textColor = color }; // Set the text color
            Vector3 labelOffset = new Vector3(size / 2, size / 2, 0);
            Vector3 labelPosition = position + (size * labelOffset);
            Handles.Label(labelPosition, label, labelStyle);
        }

        public static void DrawFilledSquare(Vector3 position, float size, Vector3 direction, Color color)
        {
            Handles.color = color;
            Handles.DrawSolidRectangleWithOutline(
                GetRectangleVertices(position, size * Vector2.one, direction),
                color, Color.clear);
        }
        // Draws a Handles.Button and executes the given action when clicked.
        public static void DrawButtonHandle(Vector3 position, float size, Vector3 direction, Color color, System.Action onClick, Handles.CapFunction capFunction)
        {
            Handles.color = color;
            if (Handles.Button(position, Quaternion.LookRotation(direction), size / 2, size, capFunction))
            {
                onClick?.Invoke(); // Invoke the action if the button is clicked
            }
        }


        private static Vector3[] GetRectangleVertices(Vector3 center, Vector2 area, Vector3 normalDirection)
        {
            Vector2 halfArea = area * 0.5f;
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(-halfArea.x, 0, -halfArea.y),
                new Vector3(halfArea.x, 0, -halfArea.y),
                new Vector3(halfArea.x, 0, halfArea.y),
                new Vector3(-halfArea.x, 0, halfArea.y)
            };

            // Calculate the rotation from the up direction to the normal direction
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normalDirection);

            // Apply rotation to each vertex
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = rotation * vertices[i] + center;
            }

            return vertices;
        }



        // Function to draw an arrow in the specified direction
        public static void DrawArrow(Vector3 position, Vector3 direction, Color color, float arrowHeadLength = 1f, float arrowHeadAngle = 45.0f)
        {
            Handles.color = color; // Set the color for the arrow

            // Draw the arrow shaft
            Handles.DrawLine(position, position + direction);

            // Calculate the right and left vectors for the arrowhead
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

            // Draw the arrowhead
            Vector3 arrowTip = position + direction;
            Handles.DrawLine(arrowTip, arrowTip + right * arrowHeadLength);
            Handles.DrawLine(arrowTip, arrowTip + left * arrowHeadLength);
        }
    }
}

#endif
