using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MTRInteractableDestinationLibrary : Library<string, float>
{
    public MTRInteractableDestinationLibrary()
    {
        ReadOnlyKey = false;
        ReadOnlyValue = false;
        Reset();

        Add("DestinationA", 0.5f);
        Add("DestinationB", -0.5f);
    }

    public void DrawInEditor(Transform origin)
    {
        int i = 0;
        Color[] colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow };
        foreach (KeyValuePair<string, float> point in this)
        {
            Gizmos.color = colors[i % colors.Length];
            Vector3 pointPos = new Vector3(origin.position.x + point.Value, origin.position.y, origin.position.z);
            Gizmos.DrawSphere(pointPos, 0.025f);
            Gizmos.DrawLine(pointPos + (Vector3.down * 5), pointPos + (Vector3.up * 5));
            CustomGizmos.DrawLabel(point.Key, pointPos + (Vector3.up * 0.1f), new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                normal = new GUIStyleState()
                {
                    textColor = Gizmos.color
                },
                fontStyle = FontStyle.Bold
            });

            i++;
        }
    }
}