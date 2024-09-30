using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Library;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MTRInteractableDestinationLibrary : Library<string, float>
{
    // The characters currently occupying the destination
    Dictionary<string, MTRCharacterInteractable> _occupants = new Dictionary<string, MTRCharacterInteractable>();

    public MTRInteractableDestinationLibrary()
    {
        ReadOnlyKey = false;
        ReadOnlyValue = false;
        Reset();

        Add("DestinationA", 0.5f);
    }

    void RefreshOccupantDestinations()
    {
        // Add any new destinations to the occupants list
        foreach (string destKey in this.Keys)
        {
            if (!_occupants.ContainsKey(destKey))
            {
                _occupants.Add(destKey, null);
            }
        }

        // Remove any occupants that are no longer in the destination list
        List<string> toRemove = new List<string>();
        foreach (string destKey in _occupants.Keys)
        {
            if (!this.ContainsKey(destKey))
            {
                toRemove.Add(destKey);
            }
        }
        foreach (string key in toRemove)
        {
            _occupants.Remove(key);
        }

    }

    public bool TryAddOccupant(MTRCharacterInteractable character, out float destinationX)
    {
        RefreshOccupantDestinations();
        destinationX = character.transform.position.x;

        // Find the first available destination
        foreach (KeyValuePair<string, float> dest in this)
        {
            if (_occupants[dest.Key] == null)
            {
                _occupants[dest.Key] = character;
                bool result = TryGetValue(dest.Key, out destinationX);
                if (result)
                {
                    CalculateDestinationX(character, dest.Key, out destinationX);
                    Debug.Log($"Character {character.name} added to destination {dest.Key} at {destinationX}");
                }

                return result;
            }
        }
        return false;
    }

    public bool TryRemoveOccupant(MTRCharacterInteractable character)
    {
        RefreshOccupantDestinations();

        foreach (KeyValuePair<string, MTRCharacterInteractable> dest in _occupants)
        {
            if (dest.Value == character)
            {
                _occupants[dest.Key] = null;
                return true;
            }
        }
        return false;
    }

    void CalculateDestinationX(MTRInteractable origin, string destinationKey, out float destinationX)
    {
        TryGetValue(destinationKey, out float xValue);
        destinationX = origin.transform.position.x + xValue;
    }

    public override void AddDefaultItem()
    {
        InternalAdd("Destination" + Count, 0.5f);
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