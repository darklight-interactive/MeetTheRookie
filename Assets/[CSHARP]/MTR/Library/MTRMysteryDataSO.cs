using System;
using System.Collections.Generic;
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
    public List<MTRInteractableDataSO> clueDataList = new List<MTRInteractableDataSO>();
}
