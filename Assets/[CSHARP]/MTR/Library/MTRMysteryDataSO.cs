using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "MTRMysteryDataSO", menuName = "MeetTheRookie/Mystery Data")]
public class MTRMysteryDataSO : ScriptableObject
{
    [Serializable]
    public class ClueData
    {
        public MTRInteractableDataSO interactableData;
        public bool isDiscovered;
    }

    public string mysteryName = "Default Mystery";

    [Expandable]
    public List<MTRInteractableDataSO> clueDataList = new List<MTRInteractableDataSO>();

    public List<Sprite> clueBackgrounds = new List<Sprite>();

    public Sprite GetRandomClueBackground()
    {
        if (clueBackgrounds.Count == 0)
            return null;

        return clueBackgrounds[UnityEngine.Random.Range(0, clueBackgrounds.Count)];
    }
}
