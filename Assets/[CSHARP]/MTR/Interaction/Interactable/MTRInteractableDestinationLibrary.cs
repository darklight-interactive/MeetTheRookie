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
    MTRInteractable _interactableParent;
    Vector3 _lastInteractablePosition = Vector3.zero;

    public MTRInteractable InteractableParent { get => _interactableParent; set => _interactableParent = value; }

    public MTRInteractableDestinationLibrary(MTRInteractable interactableParent)
    {
        _interactableParent = interactableParent;

        ReadOnlyKey = false;
        ReadOnlyValue = false;
        Reset();
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
        destinationX = _interactableParent.transform.position.x;

        // Find the first available destination
        foreach (KeyValuePair<string, float> dest in this)
        {
            if (_occupants[dest.Key] == null)
            {
                _occupants[dest.Key] = character;
                bool result = TryGetValue(dest.Key, out destinationX);
                if (result)
                {
                    CalculateDestinationX(dest.Key, out destinationX);
                    Debug.Log($"Character {character.name} added to destination {dest.Key} at {destinationX}");
                }
                else
                {
                    Debug.Log($"Failed to get destination {dest.Key}");
                }

                return result;
            }
            else
            {
                Debug.Log($"Destination {dest.Key} is occupied by {_occupants[dest.Key].name}");
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

    public bool IsOccupant(MTRCharacterInteractable character)
    {
        return _occupants.ContainsValue(character);
    }

    void CalculateDestinationX(string destinationKey, out float destinationX)
    {
        TryGetValue(destinationKey, out float xValue);

        if (_interactableParent != null)
        {
            destinationX = _interactableParent.transform.position.x + xValue;
            _lastInteractablePosition = _interactableParent.transform.position;
        }
        else
        {
            destinationX = _lastInteractablePosition.x + xValue;
        }
    }

    void CalculateDestinationPoint(string destinationKey, out Vector3 destinationPoint)
    {
        CalculateDestinationX(destinationKey, out float destinationX);
        if (_interactableParent != null)
            destinationPoint = _interactableParent.transform.position;
        else
            destinationPoint = _lastInteractablePosition;
        destinationPoint.x = destinationX;
    }

    public override void AddDefaultItem()
    {
        InternalAdd("Destination" + Count, 0.5f);
    }

    public void DrawInEditor(MTRInteractable interactableParent)
    {
        _interactableParent = interactableParent;
        if (_interactableParent == null)
            return;

        int i = 0;
        Color[] colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow };
        foreach (KeyValuePair<string, float> point in this)
        {
            Gizmos.color = colors[i % colors.Length];
            CalculateDestinationPoint(point.Key, out Vector3 pointPos);
            Gizmos.DrawSphere(pointPos, 0.025f);
            Gizmos.DrawLine(pointPos + (Vector3.down * 5), pointPos + (Vector3.up * 5));
            CustomGizmos.DrawLabel(point.Key + "\n" + pointPos.x, pointPos + (Vector3.up * 0.1f), new GUIStyle()
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